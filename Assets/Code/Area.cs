using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour {
    [SerializeField]
    private string m_displayName;

    [SerializeField]
    List<Level> m_levelList = new List<Level>();

    [SerializeField]
    int m_weightOrbiters = 1;

    [SerializeField]
    int m_weightNinja = 0;

    [SerializeField]
    int m_weightBombs = 0;

    // returns null if area is complete
    public Level curLevel { get { return m_curLevel; } }

    public string displayName {  get { return m_displayName; } }

    // rolls up to 100 based on weights set in editor
    private int m_rollNinja = 0;
    private int m_rollBomb = 0;

    private Level m_curLevel = null;
    private int m_curLevelIndex = -1;

    public Enemy.Type getRandomEnemyType() {
        if ( curLevel.singleType ) return curLevel.enemyType;

        var roll = Random.Range( 0, 100 );
        //Debug.Log( "enemy roll: " + roll + " (" + m_rollBomb + ", " + m_rollNinja + ", 100)" );
        if ( roll < m_rollBomb ) return Enemy.Type.Bomb;
        else if ( roll < m_rollNinja ) return Enemy.Type.Ninja;
        else return Enemy.Type.Orbiter;
    }

    public void nextLevel() {
        ++m_curLevelIndex;
        updateLevel();
    }

    public void prevLevel() {
        --m_curLevelIndex;
        updateLevel();
    }

    public void init() {
        m_curLevelIndex = 0;
        updateLevel();

        var totalWeight = m_weightBombs + m_weightNinja + m_weightOrbiters;
        var chanceBomb = Mathf.FloorToInt( (float)m_weightBombs / totalWeight * 100 );
        var chanceNinja = Mathf.FloorToInt( (float)m_weightNinja / totalWeight * 100 );

        m_rollBomb = chanceBomb;
        m_rollNinja = m_rollBomb + chanceNinja;
    }

    private void updateLevel() {
        m_curLevelIndex = Mathf.Max( 0, m_curLevelIndex );
        if ( m_curLevelIndex >= m_levelList.Count ) m_curLevel = null;
        else m_curLevel = m_levelList[m_curLevelIndex];
    }
}
