using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CapturePoint : MonoBehaviour
{
    private bool playerInside = false;
    private bool enemyInside = false;

    private float internalTimer = 0f;

    private int playerCounter = 0;
    private int enemyCounter = 0;
    public int counterMaxValue = 3;

    private Collider captureTrigger;
    private FirstPersonController playerController;

    private bool pointCaptured = false;

    public Light captureLight;
    public Color playerColor = Color.blue;
    public Color enemyColor = Color.red;
    public Color neutralColor = Color.white;

    public TMP_Text playerProgressText;
    public TMP_Text enemyProgressText;
    public TMP_Text endResult;
    public CinemachineVirtualCamera endCam;

    private void Awake()
    {
        captureTrigger = GetComponent<Collider>();
        if (captureTrigger == null || !captureTrigger.isTrigger)
        {
            Debug.LogError("The capture point needs a trigger collider.");
        }

        playerController = FindObjectOfType<FirstPersonController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (pointCaptured) return;

        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
        else if (other.CompareTag("Enemy"))
        {
            enemyInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (pointCaptured) return;

        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
        else if (other.CompareTag("Enemy"))
        {
            enemyInside = false;
        }
    }

    private void Update()
    {
        if (pointCaptured) return;

        //internal timer ticks up and does it's checks every in game second, like Paladins
        internalTimer += Time.deltaTime;

        if (internalTimer >= 1f)
        {
            internalTimer = 0f;

            if (playerInside && !enemyInside)
            {
                playerCounter += 3;
            }
            else if (enemyInside && !playerInside)
            {
                enemyCounter += 3;
            }
            else
            {
            }

            UpdateLightColor();

            //check for 100% capture
            if (playerCounter >= counterMaxValue)
            {
                playerCounter = counterMaxValue;
                FinalizePointCapture("Victory!");
            }
            else if (enemyCounter >= counterMaxValue)
            {
                enemyCounter = counterMaxValue;
                FinalizePointCapture("Defeat!");
            }

            playerProgressText.text = $"{playerCounter}%";
            enemyProgressText.text = $"{enemyCounter}%";
        }
    }
    private void UpdateLightColor()
    {
        if (pointCaptured || captureLight == null) return;

        if (playerInside && !enemyInside)
        {
            captureLight.color = playerColor;
        }
        else if (enemyInside && !playerInside)
        {
            captureLight.color = enemyColor;
        }
        else
        {
            captureLight.color = neutralColor;
        }
    }

    private void FinalizePointCapture(string winner)
    {
        pointCaptured = true;
        captureTrigger.enabled = false;

        playerController.enabled = false;

        playerProgressText.text = "";
        enemyProgressText.text = "";

        CameraManager.SwitchCamera(endCam);

        StartCoroutine(ShowEndResultAfterDelay(winner));
    }

    private IEnumerator ShowEndResultAfterDelay(string winner)
    {
        yield return new WaitForSeconds(5f); // wait for 5 seconds
        endResult.text = winner;
        StartCoroutine(ExitGame());
    }

    private IEnumerator ExitGame()
    {
        yield return new WaitForSeconds(5f);
        Application.Quit();
    }

}

