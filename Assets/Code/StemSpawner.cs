using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO make sure prefab has a Stem
public class StemSpawner : Spawner
{
    private int m_stemIndex = 0;

    // instead of creating spawn, pop it over to where we are and activate it
    public override void spawn() {
        // clamp gone - captured
        if ( !AudioManager.instance.stemAvailable() ) return;

        var count = 0;
        var success = false;
        do {
            m_stemIndex += Random.Range(3, 7);

            int countInner = 0;
            while( m_stemIndex >= AudioManager.instance.totalStemCount ) {
                m_stemIndex -= AudioManager.instance.totalStemCount;

                ++countInner;
                if ( countInner > 1000 ) {
                    Debug.LogError( "Yep this is the loop" );
                    break;
                }
            }

            //Debug.Log( "spawn stem #" + m_stemIndex + "/" + AudioManager.instance.stemCount );

            success = AudioManager.instance.activateStem( m_stemIndex, transform.position, this );

            ++count;

            if( count > 1000 ) {
                Debug.LogError( "Yep this is the loop" );
                break;
            }
        } while ( !success );

        // JTODO oncaptured callback for stems to --spawncount

        ++m_spawnCount;
        Debug.Log( "Stems spawned: " + m_spawnCount + "/" + m_spawnMax );
        return;
    }

    public void onStemCaptured() {
        --m_spawnCount;
        Debug.Log( "Stems spawned: " + m_spawnCount + "/" + m_spawnMax );
    }

    public void reset() {
        m_spawnCount = 0;
    }

    /*
    public void returnStem( GameObject a_stem ) {
        AudioManager.instance.deactivateStem( a_stem.GetComponent<Stem>() );
    }
    */
}
