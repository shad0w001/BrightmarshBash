using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [Header("Settings")]
    public Transform player;
    public Transform spawnPoint;
    public float deathYThreshold = -10f;
    public GameObject mainVirtualCamera; // Player's main camera (Cinemachine Virtual Camera)
    public GameObject tempVCamPrefab;    // Temporary Cinemachine Virtual Camera prefab
    public float respawnTime;

    private bool isRespawning = false;

    void Update()
    {
        if (!isRespawning && player.position.y < deathYThreshold)
        {
            StartCoroutine(HandleRespawn());
        }
    }

    private IEnumerator HandleRespawn()
    {
        isRespawning = true;

        // Disable main gameplay camera
        mainVirtualCamera.SetActive(false);

        // Spawn temp camera at a fixed offset behind the player
        GameObject tempCam = Instantiate(tempVCamPrefab, player.position + new Vector3(0, 2f, -5f), Quaternion.identity);
        CinemachineVirtualCamera vcam = tempCam.GetComponent<CinemachineVirtualCamera>();
        if (vcam != null)
        {
            vcam.Follow = player;    // Follow the player as they fall
            vcam.LookAt = player;    // Keep looking at the player
        }

        // Wait before respawning
        yield return new WaitForSeconds(respawnTime);

        // Teleport player to spawn point
        player.position = spawnPoint.position;
        player.rotation = Quaternion.Euler(0, spawnPoint.eulerAngles.y, 0);

        // Enable main camera and destroy temporary one
        mainVirtualCamera.SetActive(true);
        Destroy(tempCam);

        isRespawning = false;
    }
}
