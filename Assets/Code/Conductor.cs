using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AimedShooter))]
public class Conductor : MonoBehaviour {
    [SerializeField]
    private PlayArea m_playArea;

    private void OnCollisionEnter( Collision collision ) {
        //Debug.Log( "collide with conductor" );
        if( collision.gameObject.tag == "Stem" ) {
            SoundManager.instance.playSound( SoundManager.Sound.StemCollected, collision.transform.position );
            //Debug.Log( "is a stem" );
            var stem = collision.gameObject.GetComponent<Stem>();
            stem.playArea = m_playArea;
            stem.capture();
        }
    }

    private void Start() {
        GetComponent<AimedShooter>().playArea = m_playArea;

        if ( m_playArea == null ) {
            Debug.LogError( "Conductor has no play area" );
        }
    }
}
