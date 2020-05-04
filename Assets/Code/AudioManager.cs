using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {
    public static AudioManager instance;

    [SerializeField]
    AudioMixerGroup m_mixerGroup;

    [SerializeField]
    private Stem m_stemPrefab;

    [SerializeField]
    private GameObject m_debugTextMeshPrefab;
    private TextMesh m_debugTextMesh;

    [SerializeField]
    private float m_syncTimeEvery = 1.0f;

    readonly Vector3 FAR_AWAY = Vector3.one * 500.0f;
    private int m_initialCaptureCount = 0;

    public int totalStemCount {  get { return m_stemInfoList.Count; } }

    class StemInfo
    {
        public AudioSource source;
        public Stem stem;
        public bool available = true;
    }

    List<AudioClip> m_clipList = new List<AudioClip>();
    List<StemInfo> m_stemInfoList = new List<StemInfo>();

    private bool m_isReady = false;

    private float m_elapsedTime = 0.0f;

    PlayArea m_playArea;

    public void clear() {
        StopAllCoroutines();

        foreach( var stemInfo in m_stemInfoList ) {
            if ( stemInfo.source == null ) continue;
            Destroy( stemInfo.source );
        }

        m_stemInfoList.Clear();
        m_clipList.Clear();
    }

    const float PAUSE_LERP_TIME = 0.1f;

    // resume song and fade in
    IEnumerator resumeSong() {
        float volume = 0.0f;

        foreach( var stemInfo in m_stemInfoList ) {
            stemInfo.source.UnPause();
        }

        while ( volume < 1.0f ) {
            volume += 1.0f / PAUSE_LERP_TIME * Time.deltaTime;
            volume = Mathf.Min( volume, 1.0f );
            //Debug.Log( "Volume: " + volume );
            foreach ( var stemInfo in m_stemInfoList ) {
                stemInfo.source.volume = volume;
                yield return null;
            }
        }
    }

    // fade out and pause song
    IEnumerator pauseSong() {
        float volume = 1.0f;

        while ( volume > 0.0f ) {
            volume -= 1.0f / PAUSE_LERP_TIME * Time.deltaTime;
            volume = Mathf.Max( volume, 0.0f );
            //Debug.Log( "Volume: " + volume );
            foreach ( var stemInfo in m_stemInfoList ) {
                stemInfo.source.volume = volume;
                yield return null;
            }
        }

        foreach( var stemInfo in m_stemInfoList ) {
            stemInfo.source.Pause();
        }
    }

    public void onGamePausedChanged() {
        StopAllCoroutines();

        if( GameManager.instance.isPaused ) {
            StartCoroutine( pauseSong() );
            return;
        }

        syncStems();
        StartCoroutine( resumeSong() );
    }

    public void setClips(List<AudioClip> a_clipList ) {
        clear();

        // debug output
        if ( Debug.isDebugBuild ) {
            var go = Instantiate( m_debugTextMeshPrefab );
            m_debugTextMesh = go.GetComponent<TextMesh>();
        }

        // ask the level how many to have at start
        m_initialCaptureCount = GameManager.instance.curLevel.initialStemCaptureCount;
        //Debug.Log( "starting with " + m_initialCaptureCount + " clips" );

        //Debug.Log( "loading " + a_clipList.Count + " clips" );
        foreach( var clip in a_clipList ) {
            m_clipList.Add( clip );
            initializeStem( clip );
        }

        foreach( var info in m_stemInfoList ) {
            info.source.PlayDelayed( 1.0f );
        }

        syncStems();
        m_playArea = FindObjectOfType<PlayArea>();

        m_isReady = true;
    }

    public bool stemAvailable() {
        if ( !m_isReady ) return false;

        foreach( var stem in m_stemInfoList ) {
            if ( stem.available ) return true;
        }

        return false;
    }

    public bool activateStem( int a_id, Vector3 a_pos, StemSpawner m_spawner = null ) {
        if ( a_id < 0 || a_id >= m_stemInfoList.Count ) {
            Debug.LogWarningFormat( "Attempted to activate stem but ID out of range ({0}/{1})", a_id, m_stemInfoList.Count );
            return false;
        }

        if ( !m_stemInfoList[a_id].available ) return false;

        m_stemInfoList[a_id].stem.spawner = m_spawner;
        m_stemInfoList[a_id].stem.transform.position = a_pos;
        m_stemInfoList[a_id].stem.isInGame = true;
        m_stemInfoList[a_id].available = false;
        //Debug.Log( a_id + " no longer available" );
        return true;
    }

    public void deactivateStem( int a_id ) {
        if ( a_id < 0 || a_id >= m_stemInfoList.Count ) {
            Debug.LogWarningFormat( "Attempted to deactivate stem but ID out of range ({0}/{1})", a_id, m_stemInfoList.Count );
            return;
        }

        //m_stemInfoList[a_id].stem.GetComponent<Rigidbody>().velocity = Vector3.zero;
        m_stemInfoList[a_id].stem.isInGame = false;
        m_stemInfoList[a_id].stem.transform.position = FAR_AWAY;
        m_stemInfoList[a_id].available = true;
        //Debug.Log( a_id + " now available!" );
    }

    public void deactivateStem( Stem a_stem ) {
        if( a_stem == null ) {
            Debug.Log( "Tried to deactivate null stem" );
        }

        for( int i = 0; i < m_stemInfoList.Count; ++i ) {
            if( m_stemInfoList[i].stem == a_stem ) {
                deactivateStem( i );
                return;
            }
        }

        Debug.LogError( "Could not find matching stem for " + a_stem + "in audio manager" );
    }

    public GameObject getStem( int a_id ) {
        if ( a_id < 0 || a_id >= m_stemInfoList.Count ) {
            Debug.LogWarningFormat( "Attempted to get stem but ID out of range ({0}/{1})", a_id, m_stemInfoList.Count );
            return null;
        }

        return m_stemInfoList[a_id].stem.gameObject;
    }

    public void setStem( int a_id, Stem a_stem ) {
        m_stemInfoList[a_id].stem = a_stem;
    }

    public void setVolume( int a_id, float a_volume ) {
        if ( a_id < 0 || a_id >= m_stemInfoList.Count ) {
            Debug.LogWarningFormat( "Attempted to set stem volume but ID out of range ({0}/{1})", a_id, m_stemInfoList.Count );
            return;
        }

        m_stemInfoList[a_id].source.volume = a_volume;
    }

    private void initializeStem( AudioClip a_clip ) {
        a_clip.LoadAudioData();

        var source = gameObject.AddComponent<AudioSource>();
        //Debug.Log( "create audio source " + source );
        source.clip = a_clip;
        source.volume = 0.0f;
        source.spatialBlend = 0.0f;
        source.playOnAwake = false;
        source.loop = true;
        source.outputAudioMixerGroup = m_mixerGroup;

        var stemInfo = new StemInfo();
        stemInfo.source = source;

        var stem = Instantiate( m_stemPrefab, FAR_AWAY, Quaternion.identity );
        stemInfo.stem = stem;
        stem.id = m_stemInfoList.Count;

        m_stemInfoList.Add( stemInfo );

        if ( m_stemInfoList.Count <= m_initialCaptureCount ) {
            activateStem( m_stemInfoList.Count - 1, Vector3.one * 100.0f );
            stem.startCaptured = true;
        }
    }

	void Awake () {
        if ( instance != null ) {
            Destroy( gameObject );
            return;
        }
        
        instance = this;
	}

    void syncStems() {
        if ( m_stemInfoList.Count == 0 ) return;

        var songTime = m_stemInfoList[0].source.time;
        var delta = 0.0f;
        var startTime = Time.unscaledTime;
        for( int i = 1; i < m_stemInfoList.Count; ++i ) {
            delta = Time.unscaledTime - startTime;
            startTime = Time.unscaledTime;
            m_stemInfoList[i].source.time = songTime + delta;
        }
    }

    private void trySyncStems() {
        m_elapsedTime += Time.deltaTime;
        if ( m_elapsedTime < m_syncTimeEvery ) return;
        m_elapsedTime = 0.0f;
        syncStems();
    }

	void Update () {
        if( Debug.isDebugBuild && m_debugTextMesh != null && m_playArea != null ) {

            // TODO track available stems separately so we don't have to do this loop every update
            var availableCount = 0;
            foreach( var stemInfo in m_stemInfoList ) {
                if ( stemInfo.available ) ++availableCount;
            }

            m_debugTextMesh.text = "captured/gone/total\n"
                + m_playArea.capturedStemCount + "/" + availableCount + "/" + totalStemCount;
        }

        trySyncStems();
	}
}
