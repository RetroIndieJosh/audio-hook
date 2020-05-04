using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;

public class MultitextFade : MonoBehaviour {
    [SerializeField]
    TextMesh[] m_textList;

    [SerializeField]
    Color m_fadeInColor = Color.white;

    [SerializeField]
    Color m_fadeOutColor = Color.clear;

    [SerializeField]
    float m_fadeInTime = 1.0f;

    [SerializeField]
    float m_fadeStayTime = 1.5f;

    [SerializeField]
    float m_fadeOutTime = 1.0f;

    [SerializeField]
    [Tooltip( "Change to this scene when finished. Leave blank to stay in current scene." )]
    string m_targetScene;

    [SerializeField]
    bool m_stayAtEnd = false;

    [SerializeField]
    UnityEvent m_onFinish;

    [SerializeField]
    bool m_doLoop = false;

    private int m_textMeshIndex = 0;

    IEnumerator doFade() {
        var textMesh = m_textList[m_textMeshIndex];

        Debug.Log( "do fade for #" + m_textMeshIndex );

        // fade in
        var timeElapsed = 0.0f;
        while ( timeElapsed < m_fadeInTime ) {
            var t = timeElapsed / m_fadeInTime;
            textMesh.color = Color.Lerp( Color.clear, m_fadeInColor, t );

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // don't fade out last text if stay at end
        if ( m_textMeshIndex != m_textList.Length - 1 || !m_stayAtEnd ) {
            timeElapsed = 0.0f;
            while ( timeElapsed < m_fadeStayTime ) {
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            timeElapsed = 0.0f;
            while ( timeElapsed < m_fadeOutTime ) {
                var t = timeElapsed / m_fadeInTime;
                textMesh.color = Color.Lerp( m_fadeInColor, m_fadeOutColor, t );

                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }

        // loop
        ++m_textMeshIndex;
        if ( m_textMeshIndex < m_textList.Length ) {
            StartCoroutine( doFade() );
        } else {
            if ( m_doLoop ) {
                m_textMeshIndex = 0;
                StartCoroutine( doFade() );
            } else {

                // finished
                m_onFinish.Invoke();
                if ( m_targetScene != "" ) {
                    SceneManager.LoadScene( m_targetScene );
                } else if ( !m_stayAtEnd ) {
                    Destroy( gameObject );
                }
            }
        }
    }

    void Start() {
        if ( m_textList.Length < 1 ) throw new UnityException( "MultitextFade must have at least one TextMesh defined." );

        foreach ( var text in m_textList ) text.color = m_fadeOutColor;
        StartCoroutine( doFade() );
    }
}
