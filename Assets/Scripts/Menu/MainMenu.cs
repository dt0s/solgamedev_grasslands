using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string sceneToLoad;
    
    private string _savePath;
    
    public void PlayGame()
    {
        sceneToLoad = GameData.currentScene;
        Debug.Log("LOAD DATA()");
        string path = Application.persistentDataPath;
        string saveFileName = "aaaSave.data";
        _savePath = Path.Combine(path, saveFileName);
        if (File.Exists(_savePath))
        {
            Debug.Log("FILE EXISTS");
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(_savePath, FileMode.Open);
            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
            sceneToLoad = data.currentScene;
            Debug.Log("SCENE TO LOAD: " + sceneToLoad);
        }
        if (sceneToLoad == null || sceneToLoad == "MainGame")
        {
            SceneManager.LoadScene("MainGame", LoadSceneMode.Single);    
        } else if (sceneToLoad == "PortalRoom")
        {
            SceneManager.LoadScene("PortalRoom", LoadSceneMode.Single);
        }
        else if (sceneToLoad == "DesertLevel")
        {
            SceneManager.LoadScene("DesertLevel", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("PortalRoom", LoadSceneMode.Single);
        }
    }
    public void EnterSettings()
    {
        SceneManager.LoadScene("SettingsMenu", LoadSceneMode.Additive);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }
}
