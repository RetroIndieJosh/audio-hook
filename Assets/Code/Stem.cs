using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Stem : Vehicle {
    public bool isInGame = false;

    private bool m_startCaptured = false;
    public bool startCaptured { set { m_startCaptured = value; } }

    [SerializeField]
    private float m_rotateSpeed = 1.0f;

    [SerializeField]
    private float m_minDistanceFromCenter = 3.0f;

    [SerializeField]
    private float m_maxDistanceFromCenter = 7.5f;

    [SerializeField]
    private float m_moveInSpeed = 0.5f;

    public StemSpawner spawner { private get; set; }
    public PlayArea playArea { private get; set; }

    private bool m_isCaptured = false;
    private bool m_isBeingCaptured = false;

    private bool m_movingIn = true;

    public bool kidnapped {  set { m_kidnapped = value; } }
    private bool m_kidnapped = false;

    public int id { private get; set; }

    public void startCapture() {
        m_isBeingCaptured = true;
    }

    override protected void Start() {
        base.Start();

        if ( m_startCaptured ) {
            transform.position = Vector3.zero;
        } else {
            uncapture();
        }
    }

    override protected void Update() {
        base.Update();

        if ( GameManager.instance.isPaused ) return;

        if ( m_isCaptured || m_isBeingCaptured || m_kidnapped || !isInGame ) {
            return;
        }

        transform.RotateAround( Vector3.zero, Vector3.up, m_rotateSpeed * Time.deltaTime * Application.targetFrameRate );

        if ( Vector3.Distance( transform.position, Vector3.zero ) < m_minDistanceFromCenter ) {
            m_movingIn = false;
        }

        if ( Vector3.Distance( transform.position, Vector3.zero ) > m_maxDistanceFromCenter ) {
            m_movingIn = true;
        }

        m_body.velocity = transform.position * m_moveInSpeed;

        if( m_movingIn) {
            m_body.velocity = -m_body.velocity;
        }
    }

    public void capture() {
        if( spawner != null ) spawner.onStemCaptured();

        m_isBeingCaptured = false;
        m_isCaptured = true;
        GetComponent<Collider>().isTrigger = true;
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        transform.rotation = Quaternion.identity;

        AudioManager.instance.setVolume( id, 1.0f );

        if( playArea == null ) {
            Debug.LogError( "Trying to capture stem, but play area doesn't exist" );
            return;
        }
        playArea.addStem( this );
    }

    public void uncapture() {
        AudioManager.instance.setVolume( id, 0.0f );
        m_isCaptured = false;
    }

    protected override void onLeaveWorld() {
        if ( !isInGame ) return;
        //Debug.Log( "left world,deactivate" );
        uncapture();
        AudioManager.instance.deactivateStem( this );
    }

    bool m_isQuitting = false;

    private void OnDestroy() {
        if ( m_isQuitting ) return;
        Debug.LogWarning( "Stem destroyed " + this + " at " + Time.time );
    }

    private void OnApplicationQuit() {
        m_isQuitting = true;
    }
}
