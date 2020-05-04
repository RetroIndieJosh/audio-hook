using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour {
    [SerializeField]
    float m_startRotationMin = 0.0f;

    [SerializeField]
    float m_startRotationMax = 0.0f;

    [SerializeField]
    [Tooltip("If set to false, will start at startRotationMin")]
    bool m_randomizeStartRotation = false;

    [SerializeField]
    Vector3 m_center = Vector3.zero;

    [SerializeField]
    float m_speed;

    [SerializeField]
    Vector3 m_up = Vector3.up;

    private void Start() {
        var range = m_startRotationMax - m_startRotationMin;
        var rotation = m_startRotationMin;
        if( m_randomizeStartRotation ) rotation += Random.Range( 0, range );
        transform.RotateAround( m_center, m_up, rotation );
    }

    void Update () {
        if ( GameManager.instance.isPaused ) return;

        transform.RotateAround( m_center, m_up, m_speed * Time.deltaTime * Application.targetFrameRate );
	}
}
