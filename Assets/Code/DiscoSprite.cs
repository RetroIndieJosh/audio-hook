using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// changes mesh color at random, interpolating between colors
[RequireComponent(typeof(Sprite))]
public class DiscoSprite : MonoBehaviour {
    [SerializeField]
    private float m_interpolateTime = 0.5f;

    [SerializeField]
    [Tooltip("If no colors defined, will use random colors from entire RGB range")]
    private List<Color> m_colors = new List<Color>();

    [SerializeField]
    private bool m_initialColorRandom = false;

    public bool isPaused { private get; set; }

    private SpriteRenderer m_renderer;

    private float m_timeElapsed = 0.0f;
    private Color m_prevColor;
    private Color m_curColor;

    private Color getNextColor() {
        if ( m_colors.Count == 0 ) {
            return new Color( Random.Range( 0.0f, 1.0f ), Random.Range( 0.0f, 1.0f ), Random.Range( 0.0f, 1.0f ) );
        }

        var i = Random.Range( 0, m_colors.Count );
        //Debug.Log( "New color " + i + " :: " + m_colors[i] );
        return m_colors[i];
    }

	void Start () {
        m_renderer = GetComponent<SpriteRenderer>();
        var color = getNextColor();
        m_prevColor = color;

        if ( m_initialColorRandom ) m_renderer.color = color;

        m_curColor = m_renderer.color;

        isPaused = true;
	}

	void Update () {
        if ( isPaused ) return;

        if( m_timeElapsed > m_interpolateTime ) {
            m_prevColor = m_curColor;
            m_curColor = getNextColor();
            m_timeElapsed = 0.0f;
        }

        m_renderer.color = Color.Lerp( m_prevColor, m_curColor, m_timeElapsed / m_interpolateTime );
        //Debug.Log( "new color: " + m_renderer.color );
        m_timeElapsed += Time.deltaTime;
	}
}
