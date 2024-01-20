using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallaxBG : MonoBehaviour
{
    // Containing ALL backgrounds to be PARALLAXED
    public Transform[] backgrounds;
    // Proportion of cameras movement for each background
    private float[] parallaxScales;
    // How smooth is parallax effect
    public float smoothing = 1f;

    // Reference to main cameras transform
    private Transform cam;
    private Vector3 previousCamPos;

    private void Awake()
    {
        cam = Camera.main.transform;
    }

    void Start()
    {
        //prevous frame has current frames camera position
        previousCamPos = cam.position;
        
        parallaxScales = new float[backgrounds.Length];

        for (int i = 0; i < backgrounds.Length; i++)
        {
            parallaxScales[i] = backgrounds[i].position.z * -1;
        }

    }

    void Update()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            float parallax = (previousCamPos.x - cam.position.x) * parallaxScales[i];
            float backgroundTargetPosX = backgrounds[i].position.x + parallax;
            //create target position
            Vector3 backgroundTargetPos = 
                new Vector3(backgroundTargetPosX, backgrounds[i].position.y, backgrounds[i].position.z);
            // fade between current position and target position
            backgrounds[i].position =
                Vector3.Lerp(backgrounds[i].position, backgroundTargetPos, smoothing * Time.deltaTime);

        }
        // set previous pos to current pos
        previousCamPos = cam.position;
    }
}
