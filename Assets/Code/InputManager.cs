using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
    static public InputManager instance = null;

    public string inputText { get; private set; }
    public bool useGamePad { get; private set; }

    [SerializeField]
    float m_stickDeadzone = 0.25f;

    private bool m_inputTypeDetermined = false;

    public Vector2 aimDirection {
        get {
            if ( useGamePad ) {

                var stickInput = new Vector2( Input.GetAxis( "Windows Horizontal" ), Input.GetAxis( "Windows Vertical" ) );
                if ( stickInput.magnitude < m_stickDeadzone )
                    stickInput = Vector2.zero;

                return stickInput;
            }

            var x = Input.mousePosition.x - Screen.width * 0.5f;
            var y = Input.mousePosition.y - Screen.height * 0.5f;
            return new Vector2( x, y ).normalized;
        }
    }

    public bool didContinue {
        get {
            if ( useGamePad ) {
                return Input.GetButtonDown( "Windows Start" );
            }

            return Input.GetMouseButtonDown( 0 );
        }
    }

    public bool didFire {
        get {
            if ( useGamePad ) {
                return Input.GetButtonDown( "Windows A" );
            }

            return Input.GetMouseButtonDown( 0 );
        }
    }

    public bool reticleHide {
        get {
            if( useGamePad ) {
                return Input.GetAxis( "Windows LT" ) + Input.GetAxis( "Windows RT" ) == 0.0f;
            }

            //return Input.GetMouseButtonUp( 1 );
            return Input.GetKeyUp( KeyCode.Space );
        }
    }

    public bool reticleShow {
        get {
            if( useGamePad ) {
                return Input.GetAxis( "Windows LT" ) + Input.GetAxis( "Windows RT" ) > 0.0f;
            }

            //return Input.GetMouseButtonDown( 1 );
            return Input.GetKeyDown( KeyCode.Space );
        }
    }

    public bool zoomIn {
        get {
            if( useGamePad ) {
                return Input.GetButtonDown( "Windows Y" );
            }

            return Input.GetKeyDown( KeyCode.UpArrow );
        }
    }

    public bool zoomOut {
        get {
            if( useGamePad ) {
                return Input.GetButtonDown( "Windows B" );
            }

            return Input.GetKeyDown( KeyCode.DownArrow );
        }

    }

    public void redetermineInputType() {
        m_inputTypeDetermined = false;
    }

	void Awake () {
        if ( instance != null ) {
            Destroy( gameObject );
            return;
        }

        instance = this;
        useGamePad = false;
	}

    private void onInputTypeDetermined() {
        m_inputTypeDetermined = true;

        inputText = "GAME PAUSED\n\n";
        if( useGamePad ) {
            inputText +=
                "Start / B ~ Unpause\n" +
                "X ~ Restart Level\n" +
                "Y ~ Return to Title\n" +
                "Back ~ Quit Game";
        } else {
            inputText +=
                "Esc ~ Unpause\n" +
                "R ~ Restart Level\n" +
                "T ~ Return to Title\n" +
                "Q ~ Quit Game";
        }

        //Debug.Log( "Set up input text: " + inputText );
    }

    private void tryDetermineInput() {
        if ( m_inputTypeDetermined ) return;

        if( Input.GetButtonDown("Windows Start" ) ) {
            useGamePad = true;
            onInputTypeDetermined();
            Debug.Log( "using gamepad" );
        }

        if( Input.GetMouseButtonDown(0) ) {
            useGamePad = false;
            onInputTypeDetermined();
            Debug.Log( "using mouse" );
        }
    }

    private void tryHandlePauseMenu() {
        if ( !GameManager.instance.isPaused ) return;

        bool unpause = false;
        if ( useGamePad ) {
            // NOTE: unpause handled in tryPause
            // TODO handle B (to be done when refactoring input system)
            //if ( Input.GetButtonDown( "Pause" ) ) {
                //unpause = true;
            //}

            if ( Input.GetButtonDown( "Windows X" ) ) {
                GameManager.instance.startLevel();
                unpause = true;
            }

            if ( Input.GetButtonDown( "Windows Y" ) ) {
                GameManager.instance.goToTitle();
                unpause = true;
            }

            if ( Input.GetButtonDown( "Windows Back" ) ) {
                Application.Quit();
                unpause = true;
            }
        } else {
            // NOTE: unpause handled in tryPause

            if ( Input.GetKeyDown( KeyCode.R ) ) {
                GameManager.instance.startLevel();
                unpause = true;
            }

            if ( Input.GetKeyDown( KeyCode.T ) ) {
                GameManager.instance.goToTitle();
                unpause = true;
            }

            if ( Input.GetKeyDown( KeyCode.Q ) ) {
                Application.Quit();
                unpause = true;
            }

        }

        if( unpause ) {
            togglePause();
        }
    }

    private void togglePause() {
        Debug.Log( GameManager.instance.isPaused ? "Unpause " : "Pause " + " game" );
        GameManager.instance.togglePause();
        GameManager.instance.isScreenDark = GameManager.instance.isPaused;
    }

    private void tryPause() {
        if ( GameManager.instance.isOver ) return;

        if ( useGamePad ) {
            if ( Input.GetButtonDown( "Windows Start" ) ) {
                togglePause();
            }
        } else {
            if ( Input.GetKeyDown( KeyCode.Escape ) ) {
                togglePause();
            }
        }
    }
	
	void Update () {
        tryDetermineInput();

        if ( !GameManager.instance.isInGame ) return;
        tryPause();
        tryHandlePauseMenu();
	}
}
