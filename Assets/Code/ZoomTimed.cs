using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomTimed : MonoBehaviour {
    [SerializeField]
    private float m_secToFull = 1.0f;

    [SerializeField]
    [Tooltip("Y distance from camera")]
    private float m_zoomStart = 10.0f;

    [SerializeField]
    [Tooltip( "Y distance from camera" )]
    private float m_zoomEnd = 1.0f;

    private float m_totalMoveDistance = 0.0f;

    void Start () {
        if( m_zoomEnd >= m_zoomStart) {
            Debug.LogError( "Zoom Timer should have end closer to camera (lower value) than start" );
        }
        m_totalMoveDistance = m_zoomStart - m_zoomEnd;
	}

    private float m_timeElapsed = 0.0f;
	void Update () {
        m_timeElapsed += Time.deltaTime;

        var percent = 1.0f - Mathf.Min( 1.0f, m_timeElapsed / m_secToFull );
        var pos = transform.position;
        pos.y = Camera.main.transform.position.y - m_zoomEnd - m_totalMoveDistance * percent;
        transform.position = pos;

        //if ( m_timeElapsed < m_secToFull ) Debug.Log( "Zoom timed: " + percent + "% " + pos.y + "/" + ( Camera.main.transform.position.y + m_zoomEnd ) );
	}
}
