using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerBar : MonoBehaviour {
    [SerializeField]
    private SpriteRenderer m_border;

    public int valueMax { private get; set; }

    public int value {
        set {
            m_value = value;
            var xScale = percent;
            var yScale = transform.localScale.y;
            var zScale = transform.localScale.z;
            transform.localScale = new Vector3( xScale, yScale, zScale );
            //Debug.Log( "Bar scale: " + xScale + " (" + ( percent * 100 ) + "%)" );

            var spriteRenderer = GetComponent<SpriteRenderer>();
            var leftBorder = m_border.transform.position.x - m_border.sprite.bounds.extents.x;
            var x = leftBorder + spriteRenderer.sprite.bounds.extents.x * transform.localScale.x;
            var y = transform.position.y;
            var z = transform.position.z;
            transform.position = new Vector3( x, y, z );

            if ( GameManager.instance.isInBonusMode || m_renderer == null ) return;

            //Debug.Log( "coloring based on value" );
            m_renderer.color = Color.Lerp( Color.red, Color.green, percent );
        }
    }

    public float percent { get { return valueMax == 0 ? 0 : (float)m_value / valueMax; } }

    private DiscoSprite m_disco;
    private SpriteRenderer m_renderer;
    private int m_value;

    public void endDisco() {
        //Debug.Log( "end disco" );
        m_disco.isPaused = true;
    }

    public void startDisco() {
        //Debug.Log( "start disco" );
        m_disco.isPaused = false;
    }

    void Start() {
        m_disco = gameObject.AddComponent<DiscoSprite>();
        m_renderer = GetComponent<SpriteRenderer>();

        m_disco.isPaused = true;

        //Debug.Log( "power bar created" );
    }

    private void OnDestroy() {
        //Debug.Log( "power bar destroyed" );
    }
}
