using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRespawnManager : MonoBehaviour
{
    [Header("Settings")]
    public Transform self;
    public Transform spawnPoint;
    public float deathYThreshold = -10f;

    void Update()
    {
        if (self.position.y < deathYThreshold)
        {
            self.position = spawnPoint.position;
            self.rotation = Quaternion.Euler(0, spawnPoint.eulerAngles.y, 0);
        }
    }
}
