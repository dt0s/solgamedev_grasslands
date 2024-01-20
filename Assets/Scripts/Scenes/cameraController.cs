using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class cameraController : MonoBehaviour
{
    private Transform target;

    private float offset = 2;
    
    public float smoothSpeed = 0.3f;

    private string currentScene;
    
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        currentScene = SceneManager.GetActiveScene().name;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //transform.position = target.position + offset;
        Vector3 v3 = transform.position;
        //CAMERA TEST PORTAL ROOM - FIXED CAM!
        
        //Debug.Log(transform.position.y);
        if (transform.position.y > 34 || currentScene == "DesertLevel")
        {
            v3.x = Mathf.Lerp(v3.x, target.position.x, Time.deltaTime * smoothSpeed * 5);
        }
        else
        {
            v3.x = 0.5f;
        }

        if (v3.y - target.position.y + offset > 5)
        {
            smoothSpeed = 4f;
        }
        v3.y = Mathf.Lerp(v3.y, target.position.y + offset, Time.deltaTime * smoothSpeed);
        transform.position = v3;
        smoothSpeed = 0.3f;

    }
}
