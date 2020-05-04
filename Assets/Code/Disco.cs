using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// changes mesh color at random, interpolating between colors
[RequireComponent(typeof(MeshRenderer))]
public class Disco : MonoBehaviour {
    [SerializeField]
    private float m_interpolateTime = 0.5f;

    [SerializeField]
    [Tooltip("If no colors defined, will use random colors from entire RGB range")]
    private List<Color> m_colors = new List<Color>();

    [SerializeField]
    private bool m_initialColorRandom = false;

    private MeshRenderer m_renderer;

    private float m_timeElapsed = 0.0f;
    private Color m_prevColor;
    private Color m_curColor;

    private Color getNextColor() {
        if ( m_colors.Count == 0 ) {
            return new Color( Random.Range( 0.0f, 1.0f ), Random.Range( 0.0f, 1.0f ), Random.Range( 0.0f, 1.0f ) );
        }

        var i = Random.Range( 0, m_colors.Count );
        return m_colors[i];
    }

	void Start () {
        m_renderer = GetComponent<MeshRenderer>();
        var color = getNextColor();
        m_prevColor = color;

        if ( m_initialColorRandom ) m_renderer.material.color = color;

        m_curColor = m_renderer.material.color;
	}
	
	void Update () {
        if( m_timeElapsed > m_interpolateTime ) {
            m_prevColor = m_curColor;
            m_curColor = getNextColor();
            m_timeElapsed = 0.0f;
        }

        m_renderer.material.color = Color.Lerp( m_prevColor, m_curColor, m_timeElapsed / m_interpolateTime );
        m_timeElapsed += Time.deltaTime;
	}
}
