using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof( Aimer ) )]
public class AimedShooter : MonoBehaviour {
    [SerializeField]
    private Rigidbody m_bulletPrefab;

    [SerializeField]
    private float m_bulletSpeed = 1.0f;

    // TODO reimplement
    //[SerializeField]
    //float m_initialOffset = 0.0f;

    public PlayArea playArea { private get; set; }
    private Aimer m_aimer;
    private bool m_fired = false;

    private void Start() {
        m_aimer = GetComponent<Aimer>();
    }

    void handleFire() {
        if ( !InputManager.instance.didFire || m_fired ) return;

        SoundManager.instance.playSound( SoundManager.Sound.HookshotFire, transform.position );

        var bullet = Instantiate( m_bulletPrefab, transform.position, Quaternion.LookRotation( m_aimer.direction ) );
        bullet.velocity = m_aimer.direction * m_bulletSpeed;

        var bulletComp = bullet.GetComponent<RetractableBullet>();
        bulletComp.onDestroyed.AddListener( onBulletDestroyed );
        bulletComp.playArea = playArea;
        bulletComp.isLongShot = m_aimer.isReticleVisible;

        m_fired = true;
    }

    void Update () {
        if ( GameManager.instance.isOver || GameManager.instance.isPaused ) return;

        handleFire();
	}

    void onBulletDestroyed() {
        m_fired = false;
    }
}
