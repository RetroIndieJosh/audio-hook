using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Vehicle : MonoBehaviour {
    private Vector3 m_worldBounds;

    protected Rigidbody m_body;

    protected virtual void Start() {
        m_body = GetComponent<Rigidbody>();
        m_worldBounds = Vector3.one * GameManager.instance.worldRadius;
    }

    protected virtual void Update () {
        if( transform.position.x < -m_worldBounds.x || transform.position.x > m_worldBounds.x 
            || transform.position.y < -m_worldBounds.y || transform.position.y > m_worldBounds.y 
            || transform.position.z < -m_worldBounds.z || transform.position.z > m_worldBounds.z  ) {

            //Debug.Log( "Leaving world at pos " + transform.position + " (world bounds: " + m_worldBounds + ")" );
            onLeaveWorld();
        }
	}

    protected virtual void onLeaveWorld() { }

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine( transform.position, transform.position + m_body.velocity * 10.0f );
    }
}
