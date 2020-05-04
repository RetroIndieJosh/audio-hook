using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aimer : MonoBehaviour
{
    [SerializeField]
    Color m_color = Color.red;

    [SerializeField]
    private float m_sightLength = 10.0f;

    [SerializeField]
    private float m_startWidth = 0.1f;

    [SerializeField]
    private float m_endWidth = 0.0f;

    [SerializeField]
    private Material m_material;

    public Vector3 direction { get { return m_direction; } }
    private Vector3 m_direction;
    private LineRenderer m_lineRenderer;

    public bool isReticleVisible { get; private set; }

    private void Start() {
        m_lineRenderer = gameObject.AddComponent<LineRenderer>();

        m_lineRenderer.material = m_material;
        m_material.color = m_color;

        m_lineRenderer.startWidth = m_startWidth;
        m_lineRenderer.endWidth = m_endWidth;

        isReticleVisible = false;
    }

    void determineAim() {
        var inputDirection = InputManager.instance.aimDirection;
        m_direction.x = inputDirection.x;
        m_direction.y = transform.position.y;
        m_direction.z = inputDirection.y;
        //Debug.Log( "Direction: " + m_direction );

        if ( InputManager.instance.reticleShow ) {
            isReticleVisible = true;
        }

        if ( InputManager.instance.reticleHide ) {
            isReticleVisible = false;
        }
    }

    void Update() {
        if ( GameManager.instance.isOver || GameManager.instance.isPaused ) return;

        determineAim();

        if ( !isReticleVisible ) {
            m_lineRenderer.enabled = false;
            return;
        }

        m_lineRenderer.enabled = true;
        m_lineRenderer.SetPosition( 0, transform.position );
        m_lineRenderer.SetPosition( 1, m_direction.normalized * m_sightLength );
    }
}
