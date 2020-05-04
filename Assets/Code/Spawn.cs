using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour {
    public Spawner spawner { private get; set; }

    private void OnDestroy() {
        spawner.onSpawnDestroyed(this);
    }
}
