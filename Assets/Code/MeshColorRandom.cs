using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MeshColorRandom : MonoBehaviour {
    [SerializeField]
    private Color m_color;

	void Awake() {
        GetComponent<MeshRenderer>().material.color = new Color( Random.Range( 0.0f, 1.0f ), Random.Range( 0.0f, 1.0f ), Random.Range( 0.0f, 1.0f ) );
	}
}
