using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RetractableBullet : Vehicle {
    [HideInInspector]
    public UnityEvent onDestroyed;

    [SerializeField]
    private float m_distanceMin = 1.0f;

    [SerializeField]
    private float m_distanceMax = 10.0f;

    [SerializeField]
    private float m_enemyStunTime = 0.5f;

    [SerializeField]
    private float m_retractSpeedMult = 1.5f;

    public bool isLongShot { private get; set; }

    private Vector3 m_retractVec {  get {  return -m_initialVel * m_retractSpeedMult;} }

    private bool m_isRetracting = false;
    private Vector3 m_initialVel;
    private Stem m_stem;

    private Vector3 m_prevVelocity;

    private LineRenderer m_lineRenderer;

    private float m_retractDistance;

    public PlayArea playArea { private get; set; }

    private void OnCollisionEnter( Collision collision ) {
        Debug.Log( "hookshot collided" );
        var tag = collision.gameObject.tag;
        if ( tag == "Stem" ) {
            Debug.Log( "hookshot collided with stem" );
            SoundManager.instance.playSound( SoundManager.Sound.StemHit, collision.transform.position );
            m_stem = collision.gameObject.GetComponent<Stem>();
            if ( playArea.hasRoomForMore ) {
                m_stem.startCapture();
                m_stem.GetComponent<Rigidbody>().velocity = m_retractVec;
            } else {
                m_stem.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        } else if ( tag == "Enemy" ) {
            SoundManager.instance.playSound( SoundManager.Sound.EnemyHit, collision.transform.position );
            //Debug.Log( "Push enemy" );
            collision.gameObject.GetComponent<Enemy>().kill( m_enemyStunTime );
        } else {
            Debug.Log( "destroy because unknown collision occurred" );
            Destroy( gameObject );
            Debug.LogWarning( "Destroyed hookshot because it hit something weird" );
        }

        retract();
    }

    void OnPauseGame() {
        var body = GetComponent<Rigidbody>();
        m_prevVelocity = body.velocity;
        body.velocity = Vector3.zero;

        Debug.Log( "bullet paused" );
    }

    void OnResumeGame() {
        GetComponent<Rigidbody>().velocity = m_prevVelocity;

        Debug.Log( "bullet paused" );
    }

    override protected void Start () {
        base.Start();

        m_lineRenderer = gameObject.AddComponent<LineRenderer>();
        m_lineRenderer.startWidth = m_lineRenderer.endWidth = GameManager.instance.ropeWidth;
        m_lineRenderer.material = GameManager.instance.ropeMaterial;
        m_lineRenderer.SetPosition( 0, Vector3.zero );
        m_lineRenderer.SetPosition( 1, Vector3.zero );

        m_body = GetComponent<Rigidbody>();
        m_initialVel = m_body.velocity;

        m_retractDistance = isLongShot ? m_distanceMax : m_distanceMin;
	}

    void handleRetract() {
        var distance = Vector3.Distance( Vector3.zero, transform.position );

        var retractDistance = GameManager.instance.useFullExtents ? m_distanceMax : m_retractDistance;
        if ( !m_isRetracting && distance > retractDistance ) {
            retract();
        }

        if ( m_isRetracting && distance < 0.25f ) {
            //Debug.Log( "destroy because too close" );
            Destroy( gameObject );
        }

        // HACK snap back to player if we get stuck
        if ( m_body.velocity.magnitude < m_initialVel.magnitude * 0.5f ) {
            //Debug.Log( "destroy because stuck" );
            Destroy( gameObject );
        }
    }

    override protected void Update() {
        base.Update();

        if ( GameManager.instance.isPaused ) return;

        m_lineRenderer.SetPosition( 1, transform.position );

        handleRetract();

        //Debug.LogFormat( "vel {0}", m_body.velocity );
    }

    void retract() {
        if ( m_isRetracting ) return;

        m_body.velocity = m_retractVec;
        m_isRetracting = true;

        GetComponent<Collider>().isTrigger = true;
    }

    // if we somehow leave the world, snap back
    protected override void onLeaveWorld() {
        //Debug.Log( "destroy because left world" );
        Destroy(gameObject);
    }

    private void OnDestroy() {
        onDestroyed.Invoke();
    }
}
