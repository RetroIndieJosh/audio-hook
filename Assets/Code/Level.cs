using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {
    [SerializeField]
    private string m_displayName = "Unnamed Level";

    [SerializeField]
    private bool m_singleType = false;

    [SerializeField]
    [Tooltip("The singular type of enemies if 'single type' is true.")]
    private Enemy.Type m_enemyType = Enemy.Type.Bomb;

    [Header("Clips & Stems")]

    [SerializeField]
    public List<AudioClip> clipList;

    [SerializeField]
    [Tooltip("The number of stems in the player's possession when the level begins.")]
    private int m_initialStems = 1;

    [Header("Difficulty Adjustments - Level")]

    [SerializeField]
    private float m_bonusTime = 10.0f;

    [Header("Difficulty Adjustments - Enemies")]

    [SerializeField]
    private int m_enemyCountMax = 10;

    [Tooltip("Scale of enemies will be randomly selected from this list")]
    [SerializeField]
    private List<float> m_enemyScaleList;

    [SerializeField]
    private float m_enemySpawnTime = 4.0f;

    [SerializeField]
    private float m_spdBomb = 1.0f;

    [SerializeField]
    private float m_spdNinja = 1.0f;

    [Tooltip("Orbiter speed toward the center.")]
    [SerializeField]
    private float m_spdOrbiter = 1.0f;

    [Tooltip( "Orbiter speed around the center in deg/sec." )]
    [SerializeField]
    private float m_spdOrbiterRot = 1.0f;

    [SerializeField]
    private float m_orbiterDodgeSpdMult = 0.1f;

    [SerializeField]
    private float m_orbiterDodgeTime = 0.1f;

    [SerializeField]
    private float m_orbiterDodgeTimeBetween = 0.1f;

    [Header("Difficulty Adjustments - Stems")]

    [SerializeField]
    private int m_stemCountMax = 3;

    [SerializeField]
    private float m_stemSpawnTime = 1.0f;

    // accessors
    public float bonusTime {  get { return m_bonusTime; } }
    public int clipCount {  get { return clipList.Count; } }
    public string displayName { get { return m_displayName; } }
    public int initialStemCaptureCount {  get { return m_initialStems; } }
    public float spdBomb {  get { return m_spdBomb; } }
    public float spdNinja {  get { return m_spdNinja; } }
    public float spdOrbiter {  get { return m_spdOrbiter; } }
    public float spdOrbiterRot {  get { return m_spdOrbiterRot; } }
    public float orbiterDodgeSpdMult {  get { return m_orbiterDodgeSpdMult; } }
    public float orbiterDodgeTime {  get { return m_orbiterDodgeTime; } }
    public float orbiterDodgeTimeBetween {  get { return m_orbiterDodgeTimeBetween; } }

    public Enemy.Type enemyType {  get { return m_enemyType; } }
    public bool singleType {  get { return m_singleType; } }

    public Spawner enemySpawner {
        set {
            value.spawnMax = m_enemyCountMax;
            value.timeBetweenSpawns = m_enemySpawnTime;
        }
    }

    public StemSpawner stemSpawner {
        set {
            value.spawnMax = m_stemCountMax;
            value.timeBetweenSpawns = m_stemSpawnTime;
        }
    }

    public float getRandomEnemyScale() {
        if( m_enemyScaleList.Count == 0 ) {
            Debug.LogError( "No enemy scale values for level " + displayName );
            return 1.0f;
        }

        var i = Random.Range( 0, m_enemyScaleList.Count );
        return m_enemyScaleList[i];
    }
}
