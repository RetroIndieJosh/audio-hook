using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyToggle : MonoBehaviour {
    [SerializeField]
    private KeyCode m_toggleKey;

    [SerializeField]
    private bool m_enabled = true;

    virtual protected void Start() {
        if ( m_enabled ) onEnable();
        else onDisable();
    }

    virtual protected void Update () {
        if( Input.GetKeyDown(m_toggleKey ) ) {
            if ( m_enabled ) onDisable();
            else onEnable();
        }
		
	}

    virtual protected void onEnable() {
        m_enabled = true;

    }

    virtual protected void onDisable() {
        m_enabled = false;
    }
}
