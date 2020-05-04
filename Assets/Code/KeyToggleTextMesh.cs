using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyToggleTextMesh : KeyToggle {
    [SerializeField]
    private bool m_requireGodMode = false;

    [SerializeField]
    private Color m_visibleColor = Color.white;

    private TextMesh m_text;

	protected override void Start () {
        m_text = GetComponent<TextMesh>();

        if ( m_requireGodMode && !GameManager.instance.isGodModeEnabled ) {
            m_text.color = Color.clear;
            base.onDisable();
        }

        base.Start();
	}

    protected override void onDisable() {
        if ( m_requireGodMode && !GameManager.instance.isGodModeEnabled ) return;

        base.onDisable();
        m_text.color = Color.clear;
    }

    protected override void onEnable() {
        if ( m_requireGodMode && !GameManager.instance.isGodModeEnabled ) return;

        base.onEnable();
        m_text.color = m_visibleColor;
    }
}
