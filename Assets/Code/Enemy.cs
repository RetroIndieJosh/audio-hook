using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Disco))]
[RequireComponent(typeof(Rigidbody))]
public class Enemy : Vehicle {
    public enum Type
    {
        Bomb,
        Ninja,
        Orbiter
    }

    float m_moveInSpeed;
    float m_fleeSpeed;
    float m_rotationSpeed;

    private float m_deathTime = 0.0f;
    private Disco m_disco;
    private Color m_initialColor;
    private MeshRenderer m_meshRenderer;

    private Type m_type;
    private PlayArea m_playArea;

    private Stem m_stem;
    private bool m_hasPlayer = false;

    // stems are colliders if they're outside playarea
    private void OnCollisionEnter( Collision collision ) {
        if ( GameManager.instance.isOver ) return;

        //Debug.Log( "enemy collision" );
        if ( collision.gameObject.tag == "Player" ) {
            if( GameManager.instance.isInBonusMode ) {
                GameManager.instance.bonusEnd();
            }

            //Debug.Log( "is player" );
            m_stem = m_playArea.getMostRecentStemAndRemove();
            if( m_stem == null ) {
                if( GameManager.instance.isGodModeEnabled ) {
                    Debug.Log( "Destroy enemy because god mode" );
                    Destroy( gameObject );
                    return;
                }

                collision.gameObject.AddComponent<Disco>().enabled = true;
                GameManager.instance.gameOver();

                // drag the player away
                collision.gameObject.transform.parent = transform;
                m_hasPlayer = true;

                return;
            }

            if( m_isDying ) {
                m_stem = null;
                Debug.LogWarning( "Tried to attach stem to dying enemy" );
                return;
            }

            var stemBody = m_stem.GetComponent<Rigidbody>();
            stemBody.isKinematic = true;
            stemBody.velocity = Vector3.zero;
            m_stem.transform.localPosition = Vector3.zero + Vector3.up * GetComponent<MeshRenderer>().bounds.extents.y;
            m_stem.uncapture();
            m_stem.kidnapped = true;
            m_stem.transform.parent = transform;
        }
    }

    bool m_isDying = false;

    public void kill( float a_deathTime ) {
        m_isDying = true;

        m_body.velocity = Vector3.zero;
        m_deathTime = a_deathTime;
        m_disco.enabled = true;

        m_body.constraints = RigidbodyConstraints.FreezeAll;
        m_body.isKinematic = true;
        GetComponent<Collider>().isTrigger = true;

        tryDropStem();

        Destroy( gameObject, a_deathTime );
    }

    override protected void Start() {
        base.Start();

        transform.localScale = Vector3.one * GameManager.instance.curLevel.getRandomEnemyScale();

        m_playArea = FindObjectOfType<PlayArea>();

        m_body = GetComponent<Rigidbody>();
        m_disco = GetComponent<Disco>();

        m_meshRenderer = GetComponent<MeshRenderer>();
        m_initialColor = m_meshRenderer.material.color;

        m_type = GameManager.instance.curArea.getRandomEnemyType();
        Debug.Log( "Enemy type: " + m_type );

        m_moveInSpeed = GameManager.instance.curLevel.spdOrbiter;
        m_rotationSpeed = GameManager.instance.curLevel.spdOrbiterRot;

        var spawnSound = SoundManager.Sound.EnemyOrbiterSpawn;
        if( m_type == Type.Ninja ) {
            m_moveInSpeed = GameManager.instance.curLevel.spdNinja;
            spawnSound = SoundManager.Sound.EnemyNinjaSpawn;
        } else if( m_type == Type.Bomb ) {
            m_moveInSpeed = GameManager.instance.curLevel.spdBomb;
            // TODO implement bomb enemy
            //spawnSound = SoundManager.Sound.EnemyBombSpawn;
        }

        m_fleeSpeed = m_moveInSpeed * 2.0f;

        SoundManager.instance.playSound( spawnSound, transform.position );
    }

    float m_dodgeTime = 0.0f;
    float m_timeSinceLastDodge = 0.0f;

    override protected void Update() {
        base.Update();

        if( m_hasPlayer ) {
            m_body.velocity = transform.position.normalized * m_fleeSpeed;
            return;
        }

        if ( GameManager.instance.isPaused || GameManager.instance.isOver ) {
            m_body.velocity = Vector3.zero;
            return;
        }

        m_deathTime -= Time.deltaTime;
        if ( m_deathTime > 0.0f ) return;

        m_disco.enabled = false;

        m_meshRenderer.material.color = m_initialColor;

        if( m_stem == null ) {

            var speedMult = 1.0f;
            if( Vector3.Distance(Vector3.zero, transform.position ) < 3.0f ) {
                speedMult = 2.0f;
            }

            // toward the center
            m_body.velocity = ( -transform.position ).normalized * m_moveInSpeed * speedMult;

            if ( m_type == Type.Orbiter ) {
                // around the outside
                //var angleVel = m_moveInSpeed * 0.1f;
                //var radiusSquared = Mathf.Pow( Vector2.Distance( transform.position, Vector2.zero ), 2.0f );
                //var rotationDegs = ( 180 * angleVel ) / ( Mathf.PI * radiusSquared );

                if( m_dodgeTime > 0.0f ) {
                    m_dodgeTime -= Time.deltaTime;
                    speedMult *= GameManager.instance.curLevel.orbiterDodgeSpdMult;
                    m_timeSinceLastDodge = 0.0f;
                } else {
                    m_timeSinceLastDodge += Time.deltaTime;
                    if( m_timeSinceLastDodge > GameManager.instance.curLevel.orbiterDodgeTimeBetween ) {
                        m_dodgeTime = GameManager.instance.curLevel.orbiterDodgeTime;
                    }
                }

                transform.RotateAround( Vector3.zero, Vector3.up, m_rotationSpeed * speedMult * Time.deltaTime * Application.targetFrameRate );
            }
        }

        // if we have stem, GTFO
        else {
            m_body.velocity = transform.position.normalized * m_fleeSpeed;
        }
    }

    private void tryDropStem() {
        if ( m_stem == null ) return;

        m_stem.transform.parent = null;
        m_stem.GetComponent<Rigidbody>().isKinematic = false;

        var x = m_stem.transform.position.x;
        var y = 0.0f;
        var z = m_stem.transform.position.z;
        m_stem.transform.position = new Vector3( x, y, z );

        m_stem.kidnapped = false;
        m_stem.GetComponent<Collider>().isTrigger = false;
    }

    protected override void onLeaveWorld() {
        if ( !m_hasPlayer ) {
            tryDropStem();
            AudioManager.instance.deactivateStem( m_stem );
            m_stem = null;
        }

        Destroy( gameObject );
        base.onLeaveWorld();
    }

    private void OnDestroy() {
        if ( m_stem == null ) return;
    }
}
