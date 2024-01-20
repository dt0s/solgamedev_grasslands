using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInteractions : MonoBehaviour
{
    public GameObject currentInterObj = null;

    public string portalTaken;
    public string currentSceneName;
    
    // CREATE TAG FOR EVERY PORTAL, MOVE LOCATIONS DEPENDING ON TAG (STARTPORTAL, FIREPORTAL, WATERPORTAL, ...)
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && currentInterObj)
        {
            // PORTAL LOGIC
            // --------------------
            currentSceneName = SceneManager.GetActiveScene().name;
            portalTaken = currentInterObj.tag;
            // SET GLOBAL VARIABLES TO LOAD CORRECTLY
            GameData.currentScene = currentSceneName;
            GameData.PortalName = portalTaken;
            // CHECK THE TRANSITION TO LOAD APPROPRIATELY
            GameData.TransitionName = portalTaken + currentSceneName;

            Debug.Log("INTERACTION (T) WITH: " + portalTaken);
            Debug.Log("Current Scene: " + currentSceneName);
            GameData.loadDataForPortalTransition = true;
            // BasePortal
            switch (portalTaken + currentSceneName)
            {

                case "BasePortalMainGame":
                    GameData.hasBasePortal = true;
                    SceneManager.LoadScene("PortalRoom", LoadSceneMode.Single);
                    break;
                case "BasePortalPortalRoom":
                    if (GameData.hasBasePortal)
                    {
                        SceneManager.LoadScene("MainGame", LoadSceneMode.Single);
                    }
                    break;
                case "DesertPortalDesertLevel":
                    GameData.hasDesertPortal = true;
                    SceneManager.LoadScene("PortalRoom", LoadSceneMode.Single);
                    break;
                case "DesertPortalPortalRoom":
                    if (GameData.hasDesertPortal)
                    {
                        SceneManager.LoadScene("DesertLevel", LoadSceneMode.Single);
                    }
                    break;
                
            }
        }
    }

    // PORTAL
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("ENTER INTERACTABLE: " + other.gameObject.name);
        currentInterObj = other.gameObject;
        if (currentInterObj.tag == "switch_DesertLevel")
        {
            GameData.TransitionName = currentInterObj.name;
            SceneManager.LoadScene("DesertLevel", LoadSceneMode.Single);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("EXIT INTERACTABLE: " + other.gameObject.name);
        currentInterObj = null;
    }
}
