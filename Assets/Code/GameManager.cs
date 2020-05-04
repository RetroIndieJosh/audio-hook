using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [Header("Game Properties")]
    [SerializeField]
    private bool m_isGodModeEnabled = false;

    [SerializeField]
    private bool m_isInGame = false;

    [SerializeField]
    private float m_worldRadius = 8.0f;

    [Header("Rope Properties")]

    [SerializeField]
    private Material m_ropeMaterial;

    [SerializeField]
    private float m_ropeWidth;

    [Header("Prefabs")]

    [SerializeField]
    private GameObject m_bonusTimerPrefab;

    [SerializeField]
    private GameObject m_darkenMeshPrefab;

    [SerializeField]
    private GameObject m_debugCommandsPrefab;

    [SerializeField]
    private GameObject m_gameOverTextPrefab;

    [SerializeField]
    private GameObject m_levelCompleteTextPrefab;

    [SerializeField]
    TextMesh m_levelDisplayTextPrefab;

    [SerializeField]
    private GameObject m_pauseTextPrefab;

    [Header("Areas")]

    [SerializeField]
    private List<Area> m_areaList;

    [Header( "Object Links" )]

    [SerializeField]
    private Spawner m_enemySpawner;

    [SerializeField]
    private StemSpawner m_stemSpawner;

    public static GameManager instance;

    public float ropeWidth { get { return m_ropeWidth; } }
    public Material ropeMaterial { get { return m_ropeMaterial; } }

    public float worldRadius { get { return m_worldRadius; } }

    public bool isGodModeEnabled {  get { return m_isGodModeEnabled; } }

    private MeshRenderer m_darkenMeshRenderer;

    public bool isInGame {  get { return m_isInGame; } }

    public Level curLevel { get { return curArea.curLevel; } }
    public Area curArea {  get { return m_areaList[m_curAreaIndex]; } }

    private int m_curAreaIndex = 0;

    // prefab instances
    private GameObject m_bonusTimer; // why?
    private TextMesh m_pauseTextMesh;
    private TextMesh m_bonusTimerTextMesh;

    private bool m_hasBeatenLevel = false;
    private float m_bonusTimeRemaining = 0.0f;

    [SerializeField]
    private Color m_bonusTimeColor = Color.white;

    private bool m_isPaused = false;

    public bool isInBonusMode { get; private set; }
    public bool isOver { get; private set; }
    public bool isPaused {
        get {
            return m_isPaused;
        }
        private set {
            m_isPaused = value;
            if ( AudioManager.instance != null ) {
                AudioManager.instance.onGamePausedChanged();
            }

            var bodyList = FindObjectsOfType( typeof( Rigidbody ) );
            foreach( Rigidbody body in bodyList ) {
                if( m_isPaused ) body.gameObject.SendMessage ("OnPauseGame", SendMessageOptions.DontRequireReceiver);
                else body.gameObject.SendMessage ("OnResumeGame", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    private float m_timeSinceOver = 0.0f;

    public void bonusStart() {

        // TODO optimize & allow for multiple conductors
        FindObjectOfType<PowerBar>().startDisco();

        //Debug.Log( "bonus round started" );

        m_bonusTimerTextMesh.color = Color.white;

        isInBonusMode = true;

        //var pos = m_bonusTimer.transform.position;
        //pos.z = -0.5f;
        //m_bonusTimer.transform.position = pos;
    }

    public void bonusEnd() {
        if ( !isInBonusMode ) return;

        var powerBar = FindObjectOfType<PowerBar>();
        powerBar.endDisco();

        //Debug.Log( "end bonus" );
        //Destroy( m_bonusTimer );
        m_bonusTimerTextMesh.color = Color.clear;
        isInBonusMode = false;
    }

    public bool isScreenDark {
        set {
            m_darkenMeshRenderer.enabled = value;
        }
    }

    public void gameOver() {
        isScreenDark = true;

        SoundManager.instance.playSound( SoundManager.Sound.GameOver, Vector3.zero );
        isOver = true;
        bonusEnd();

        m_timeSinceOver = 0.0f;
        var obj = Instantiate( m_gameOverTextPrefab );

        string buttonText = "";
        if ( InputManager.instance.useGamePad ) {
            buttonText = "Press Start ";
        } else {
            buttonText = "Left Click ";
        }

        obj.GetComponent<TextMesh>().text = "GAME OVER\n" + buttonText + "to try again";
    }

    public void nextLevel() {
        curArea.nextLevel();

        // end of area
        if ( curLevel == null ) {
            ++m_curAreaIndex;

            if ( m_curAreaIndex >= m_areaList.Count ) {
                goToTitle();
                return;
            }

            curArea.init();
        }

        startLevel();
    }
    
    public void togglePause() {
        isPaused = !isPaused;

        if( isPaused ) {
            var obj = Instantiate( m_pauseTextPrefab );
            m_pauseTextMesh = obj.GetComponent<TextMesh>();
            m_pauseTextMesh.text = InputManager.instance.inputText;
            Debug.Log( "Pause text: " + InputManager.instance.inputText );
        } else {
            Destroy( m_pauseTextMesh.gameObject );
            m_pauseTextMesh = null;
        }
    }

    public void goToTitle() {
        AudioManager.instance.clear();
        bonusEnd();
        m_isInGame = false;

        m_curAreaIndex = 0;
        curArea.init();

        InputManager.instance.redetermineInputType();
        SceneManager.LoadScene( "title" );
    }

    public void startLevel() {
        if ( m_stemSpawner != null ) {
            m_stemSpawner.reset();
        }

        // reset game state
        bonusEnd();
        m_hasBeatenLevel = false;

        SceneManager.LoadScene( "main" );
    }

    private void SceneManager_sceneLoaded( Scene arg0, LoadSceneMode arg1 ) {
        if ( !m_isInGame ) return;

        initLevel();
        Instantiate( m_debugCommandsPrefab );

        // TODO only trigger on first enter game from title or menu
        SoundManager.instance.playSound( SoundManager.Sound.GameStart, Vector3.zero );
    }

    private void initLevel() {
        isInBonusMode = false;
        isOver = false;

        if( m_levelDisplayTextPrefab != null ) {
            var obj = Instantiate( m_levelDisplayTextPrefab );
            obj.text = curArea.displayName + " ~ " + curLevel.displayName;
            obj.transform.parent = Camera.main.transform;
        }

        m_bonusTimer = Instantiate( m_bonusTimerPrefab );
        m_bonusTimer.transform.parent = Camera.main.transform;
        m_bonusTimerTextMesh = m_bonusTimer.GetComponent<TextMesh>();
        m_bonusTimeRemaining = curLevel.bonusTime;
        m_bonusTimerTextMesh.color = m_bonusTimeColor;

        if( curArea == null ) {
            Debug.LogError( "Tried to load level from null area " + curArea.displayName );
            return;
        }

        if( curLevel == null ) {
            Debug.LogError( "Tried to load null level in area " + curArea.displayName );
            return;
        }

        var clips = curLevel.clipList;
        if( clips == null ) {
            Debug.LogError( "No clip list for level " + curLevel.displayName + " in area " + curArea.displayName );
            return;
        }

        if( clips.Count == 0) {
            Debug.LogError( "Clip list empty for level " + curLevel.displayName + " in area " + curArea.displayName );
            return;
        }

        if( AudioManager.instance == null ) {
            Debug.LogError( "No audio manager when trying to load level" );
            return;
        }

        setConductorColor();

        AudioManager.instance.setClips( clips );

        // link level to game objects to set up difficulty values
        curLevel.enemySpawner = m_enemySpawner;
        curLevel.stemSpawner = m_stemSpawner;

        m_enemySpawner.enabled = true;
        m_stemSpawner.enabled = true;
    }

    void Awake() {
        if ( instance != null ) {
            Destroy( gameObject );
            return;
        }
        instance = this;

        Application.targetFrameRate = 60;
        curArea.init();
    }

    private void Start() {
        if( m_isGodModeEnabled ) Debug.Log( "NOTE: GOD MODE ENABLED" );
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;

        var darkObj = Instantiate( m_darkenMeshPrefab );
        DontDestroyOnLoad( darkObj );
        m_darkenMeshRenderer = darkObj.GetComponent<MeshRenderer>();

        m_enemySpawner.enabled = false;
        m_stemSpawner.enabled = false;

        if ( m_isInGame ) initLevel();
	}

    private void beatLevel() {
        isScreenDark = true;

        SoundManager.instance.playSound( SoundManager.Sound.LevelComplete, Vector3.zero );
        isOver = true;
        m_hasBeatenLevel = true;
        bonusEnd();

        m_timeSinceOver = 0.0f;
        var obj = Instantiate( m_levelCompleteTextPrefab );

        string buttonText = "";
        if ( InputManager.instance.useGamePad ) {
            buttonText = "Press Start ";
        } else {
            buttonText = "Left Click ";
        }

        string text = "";
        if ( m_curAreaIndex == m_areaList.Count - 1 && curLevel == null ) {
            text = "GAME COMPLETE\n" + buttonText + "to return to title";
        } else {
            text += "LEVEL COMPLETE\n" + buttonText + "for next level";
        }

        obj.GetComponent<TextMesh>().text = text;
    }

    public bool useFullExtents {  get { return m_useFullExtents; } }
    bool m_useFullExtents = false;

    void setConductorColor() {
        var conductor = FindObjectOfType<Conductor>();
        if ( conductor != null ) {
            conductor.GetComponent<MeshRenderer>().material.color = m_isGodModeEnabled ? Color.magenta : Color.green;
        }
    }

	void Update () {
        if ( isPaused ) return;

        if ( Debug.isDebugBuild && Input.GetKeyDown( KeyCode.G ) ) {
            m_isGodModeEnabled = !m_isGodModeEnabled;
            setConductorColor();
        }

        if ( isGodModeEnabled ) {
            if( Input.GetKeyDown(KeyCode.B ) ) {
                beatLevel();
            }

            if( Input.GetKeyDown(KeyCode.N ) ) {
                nextLevel();
            }

            if( Input.GetKeyDown(KeyCode.O ) ) {
                gameOver();
            }

            if( Input.GetKeyDown(KeyCode.P ) ) {
                curArea.prevLevel();
            }

            if( Input.GetKeyDown(KeyCode.R ) ) {
                startLevel();
            }

            if ( Input.GetKeyDown( KeyCode.X ) ) {
                m_useFullExtents = true;
                Debug.Log( "use full extents" );
            }

            if ( Input.GetKeyUp( KeyCode.X ) ) {
                m_useFullExtents = false;
                Debug.Log( "stop use full extents" );
            }
        }

        if ( !m_isInGame ) {
            if( Input.GetButton("Windows Back" ) || Input.GetKeyDown(KeyCode.Escape ) ) {
                Application.Quit();
            }

            if ( InputManager.instance.didContinue ) {
                isScreenDark = false;
                //Debug.Log( "start game" );
                m_curAreaIndex = 0;
                m_isInGame = true;
                startLevel();
            }

            return;
        }

        if ( isOver ) {
            m_timeSinceOver += Time.deltaTime;
            if ( m_timeSinceOver < 1.0f ) return;

            if ( InputManager.instance.didContinue ) {
                isScreenDark = false;
                if ( m_hasBeatenLevel ) nextLevel();
                else startLevel();
            }
        }

        if ( !isInBonusMode ) return;
        m_bonusTimeRemaining -= Time.deltaTime;
        m_bonusTimerTextMesh.text = "Defend!\n" + m_bonusTimeRemaining.ToString("0.00");

        if ( m_bonusTimeRemaining <= 0.0f ) {
            beatLevel();
        }
	}
}
