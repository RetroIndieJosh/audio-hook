using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour {
    public static SoundManager instance = null;

    [SerializeField]
    AudioMixerGroup m_mixerGroup;
    
    [SerializeField]
    List<AudioClip> m_gameOverSounds;

    [SerializeField]
    List<AudioClip> m_gameStartSounds;

    [SerializeField]
    AudioClip m_enemyHitSound;

    [SerializeField]
    AudioClip m_enemyNinjaSpawnSound;

    [SerializeField]
    AudioClip m_enemyOrbiterSpawnSound;

    [SerializeField]
    AudioClip m_hookshotFireSound;

    [SerializeField]
    AudioClip m_levelCompleteSound;

    [SerializeField]
    AudioClip m_stemCollectedSound;

    [SerializeField]
    AudioClip m_stemHitSound;

    public enum Sound
    {
        EnemyHit,
        EnemyNinjaSpawn,
        EnemyOrbiterSpawn,
        HookshotFire,
        GameOver,
        GameStart,
        LevelComplete,
        StemCollected,
        StemHit
    }

    public void playSoundRandom( List<AudioClip> a_clipList, Vector3 a_pos ) {
        var i = Random.Range( 0, a_clipList.Count );
        playSound( a_clipList[i], a_pos );
    }

    public  void playSound( AudioClip a_clip, Vector3 a_pos ) {
        var tempObj = new GameObject();
        tempObj.transform.position = a_pos;
        var source = tempObj.AddComponent<AudioSource>();
        source.clip = a_clip;
        source.spatialBlend = 0.0f;
        source.outputAudioMixerGroup = m_mixerGroup;
        source.Play();
        Destroy( tempObj, source.clip.length );
    }

    public void playSound( Sound a_sound, Vector3 a_pos ) {
        //Debug.Log( "Play sound " + a_sound );
        switch( a_sound ) {
            case Sound.EnemyHit: playSound( m_enemyHitSound, a_pos ); return;
            case Sound.EnemyNinjaSpawn: playSound( m_enemyNinjaSpawnSound, a_pos ); return;
            case Sound.EnemyOrbiterSpawn: playSound( m_enemyOrbiterSpawnSound, a_pos ); return;
            case Sound.GameOver: playSoundRandom( m_gameOverSounds, a_pos ); return;
            case Sound.GameStart: playSoundRandom( m_gameStartSounds, a_pos ); return;
            case Sound.HookshotFire: playSound( m_hookshotFireSound, a_pos ); return;
            case Sound.LevelComplete: playSound( m_levelCompleteSound, a_pos ); return;
            case Sound.StemCollected: playSound( m_stemCollectedSound, a_pos ); return;
            case Sound.StemHit: playSound( m_stemHitSound, a_pos ); return;
        }
    }

	void Awake () {
        if( instance != null ) {
            Destroy ( gameObject );
            return;
        }

        instance = this;
	}

    IEnumerator testSounds() {
        for( int i = 0; i < 7; ++i ) {
            playSound( (Sound)i, Vector3.one * Random.Range(-10.0f, 10.0f ) );
            yield return new WaitForSeconds( 2 );
        }
    }

    private void Start() {
        //StartCoroutine( testSounds() );
    }
}
