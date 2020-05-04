using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// input-controlled, discrete movement of an object on y-plane
public class ZoomChanger : MonoBehaviour {
    [Tooltip("List of zoom positions - must be in order from closest to furthest")]
    [SerializeField]
    private List<float> m_posList = new List<float>();

    [Tooltip("Index from the list above which is the inital zoom level")]
    [SerializeField]
    private int m_defaultPosIndex = 0;

    private int m_posIndex = 0;

    private void Awake() {
        m_posIndex = m_defaultPosIndex;
        updatePos();
    }

    void Update () {
        if( InputManager.instance.zoomIn ) {
            --m_posIndex;
            updatePos();
        }

        if( InputManager.instance.zoomOut ) {
            ++m_posIndex;
            updatePos();
        }
	}

    void updatePos() {
        m_posIndex = Mathf.Clamp( m_posIndex, 0, m_posList.Count - 1 );

        var pos = transform.position;
        pos.y = m_posList[m_posIndex];
        transform.position = pos;
    }
}
