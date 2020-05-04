using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayArea : MonoBehaviour { 
    public bool hasRoomForMore {
        get {
            /*
            var adjustedMax = m_stemCountMax;
            if ( m_stemCountMax > 1 && m_stemCountMax % 2 == 1 ) {
                --adjustedMax;
            }

            return m_stemList.Count < adjustedMax;
            */

            //Debug.Log( m_stemList.Count + "/" + AudioManager.instance.stemCount );
            return capturedStemCount < AudioManager.instance.totalStemCount;
        }
    }
    [SerializeField]
    Vector2 m_stemSize = Vector2.one;

    [SerializeField]
    private PowerBar m_powerBar;

    public PowerBar powerBar { get { return m_powerBar; } }

    private int m_stemCountMax;
    private int m_stemsX;
    private int m_stemsY;

    List<Stem> m_stemList = new List<Stem>();
    public int capturedStemCount {  get { return m_stemList.Count; } }

    private Vector3 m_topLeft;

    public void addStem( Stem a_stem ) {
        if ( !hasRoomForMore ) return;

        if ( m_stemCountMax == 1 ) {
            a_stem.transform.position = transform.position;
        } else {
            int curIndexAdjusted = m_stemList.Count;
            if ( m_stemCountMax % 2 == 1 ) {
                int centerIndex = Mathf.FloorToInt( m_stemCountMax * 0.5f );
                if ( curIndexAdjusted >= centerIndex ) {
                    ++curIndexAdjusted;
                }

                //Debug.Log( "index = " + curIndexAdjusted + "; center = " + centerIndex + " adjusted index = " + curIndexAdjusted );
            }

            // find location for stem
            int xPos = curIndexAdjusted % m_stemsX;
            int yPos = curIndexAdjusted / m_stemsX;
            a_stem.transform.position = m_topLeft + new Vector3( ( xPos + 0.5f ) * m_stemSize.x, 0.0f, ( yPos + 0.5f ) * m_stemSize.y );
        }

        m_stemList.Add( a_stem );

        //Debug.Log( "Placing new stem #" + m_stemList.Count + " at " + xPos + ", " + yPos );

        m_powerBar.value = m_stemList.Count;

        if( !hasRoomForMore ) {
            GameManager.instance.bonusStart();
        }
    }

    public Stem getMostRecentStemAndRemove() {
        if ( m_stemList.Count == 0 ) return null;

        //var i = Random.Range( 0, m_stemList.Count );
        var i = m_stemList.Count - 1;
        var stem = m_stemList[i];

        m_stemList.RemoveAt( i );
        m_powerBar.value = m_stemList.Count;

        return stem;
    }

    private bool isPerfectSquare( int a_num ) {
        var root = Mathf.Sqrt( m_stemCountMax );
        var rootInt = Mathf.FloorToInt( root );
        return Mathf.Pow(root, 2 ) == Mathf.Pow(rootInt, 2 );
    }

    private void Awake() {
        m_stemCountMax = GameManager.instance.curLevel.clipCount;
        if ( m_stemCountMax > 1 ) {
            if ( isPerfectSquare( m_stemCountMax ) && m_stemCountMax % 2 == 1 ) {
                Debug.LogError( "Odd perfect squares are not supported - please remove one audio clip" );
                return;
            }

            if ( !isPerfectSquare( m_stemCountMax ) ) {
                ++m_stemCountMax;
                if ( !isPerfectSquare( m_stemCountMax ) ) {
                    Debug.LogError( "Clips must be an even perfect square, or an odd perfect square minus one (" + m_stemCountMax-- + ")" );
                    return;
                }
            }
        }

        m_stemsX = m_stemsY = Mathf.FloorToInt( Mathf.Sqrt( m_stemCountMax ) );

        var scaleX = m_stemsX * m_stemSize.x * 0.1f;
        var scaleY = 1.0f;
        var scaleZ = m_stemsY * m_stemSize.y * 0.1f;
        transform.localScale = new Vector3( scaleX, scaleY, scaleZ );
        //Debug.Log( "Play area scale: " + transform.localScale );

        var meshRenderer = GetComponent<MeshRenderer>();
        var halfWidth = meshRenderer.bounds.extents.x;
        var halfHeight = meshRenderer.bounds.extents.z;

        m_topLeft = transform.position - new Vector3( halfWidth, 0, halfHeight );
        //Debug.Log( "Top left: " + m_topLeft );

        /*
        m_stemsX = Mathf.FloorToInt( halfWidth * 2.0f / m_stemSize.x );
        m_stemsY = Mathf.FloorToInt( halfHeight * 2.0f / m_stemSize.y );
        m_stemCountMax = m_stemsX * m_stemsY;
        */

        if ( m_stemCountMax > 1 && m_stemCountMax % 2 == 1 ) {
            m_powerBar.valueMax = m_stemCountMax - 1;
        } else {
            m_powerBar.valueMax = m_stemCountMax;
        }
        //Debug.Log( m_stemsX + "x" + m_stemsY + " stems = " + m_stemCountMax + " max stems" );
    }

    private void Start() {
        m_powerBar.value = 0;
    }
}
