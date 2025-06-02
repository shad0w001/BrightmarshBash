using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapturePoint : MonoBehaviour
{
    private bool playerInside = false;
    private bool enemyInside = false;

    private float internalTimer = 0f;

    private int playerCounter = 0;
    private int enemyCounter = 0;

    private Collider captureTrigger;
    private FirstPersonController playerController;

    private bool pointCaptured = false;

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
            Debug.Log("Player entered capture point");
        }
        else if (other.CompareTag("Enemy"))
        {
            enemyInside = true;
            Debug.Log("Enemy entered capture point");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (pointCaptured) return;

        if (other.CompareTag("Player"))
        {
            playerInside = false;
            Debug.Log("Player left capture point");
        }
        else if (other.CompareTag("Enemy"))
        {
            enemyInside = false;
            Debug.Log("Enemy left capture point");
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
                Debug.Log("Player counter: " + playerCounter);
            }
            else if (enemyInside && !playerInside)
            {
                enemyCounter += 3;
                Debug.Log("Enemy counter: " + enemyCounter);
            }
            else
            {
                //Debug.Log("No capture progress this second");
            }

            //check for 100% capture
            if (playerCounter >= 100)
            {
                FinalizePointCapture("Player");

                //ShowVictoryScreen()
            }
            else if (enemyCounter >= 100)
            {
                FinalizePointCapture("Enemy");

                //ShowDefeatScreen()
            }
        }
    }

    private void FinalizePointCapture(string winner)
    {
        pointCaptured = true;
        captureTrigger.enabled = false; //disable the trigger collider so there is no more capturing
        Debug.Log(winner + " wins! Capture point locked.");

        playerController.enabled = false;

        // TODO: Add your game over logic here (e.g. show UI, stop player input, etc.)
    }
}

