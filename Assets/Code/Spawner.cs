using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    [SerializeField]
    private int m_initialSpawnCount = 1;

    [SerializeField]
    private float m_timeBetweenSpawns = 10.0f;

    [SerializeField]
    protected int m_spawnMax = 5;

    [SerializeField]
    protected GameObject m_spawnPrefab;

    [SerializeField]
    [Tooltip("If false, only spawn initial amount (ignore time between spawns)")]
    private bool m_continuousSpawn = true;

    public int spawnMax {  set { m_spawnMax = value; } }
    public float timeBetweenSpawns {  set { m_timeBetweenSpawns = value; } }

    private float m_timeToNextSpawn = 10.0f;
    protected int m_spawnCount = 0;

    virtual public void onSpawnDestroyed( Spawn spawn) {
        --m_spawnCount;
    }

    virtual public void spawn() {
        var obj = Instantiate( m_spawnPrefab.gameObject, transform.position, Quaternion.identity );
        var spawn = obj.AddComponent<Spawn>();
        spawn.spawner = this;

        //Debug.Log( "spawn at " + spawn.transform.position );
        ++m_spawnCount;
    }

    private void updateSpawnTime() {
        m_timeToNextSpawn = m_timeBetweenSpawns;
    }

    virtual protected void Start() {
        updateSpawnTime();
        for( int i = 0; i < m_initialSpawnCount; ++i ) {
            spawn();
        }
    }

    void Update() {
        if ( GameManager.instance.isOver || GameManager.instance.isPaused ) return;
        if ( !m_continuousSpawn ) return;
        if ( m_spawnCount >= m_spawnMax ) return;

        m_timeToNextSpawn -= Time.deltaTime;
        if ( m_timeToNextSpawn <= 0 ) {
            spawn();
            updateSpawnTime();
        }
    }

    protected void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere( transform.position, 1.0f );
    }
}
