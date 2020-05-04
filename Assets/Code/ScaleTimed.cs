using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleTimed : MonoBehaviour {
    [SerializeField]
    private float m_secToFull = 1.0f;

    [SerializeField]
    private float m_scaleMin = 0.1f;

    [SerializeField]
    private float m_scaleMax = 1.0f;

    private float m_scaleRange;

    void Start () {
        transform.localScale = Vector3.one * m_scaleMin;
        m_scaleRange = m_scaleMax - m_scaleMin;
	}

    private float m_timeElapsed = 0.0f;
	void Update () {
        m_timeElapsed += Time.deltaTime;
        var percent = Mathf.Min( 1.0f, m_timeElapsed / m_secToFull );
        transform.localScale = Vector3.one * m_scaleRange * percent + Vector3.one * m_scaleMin;
	}
}
