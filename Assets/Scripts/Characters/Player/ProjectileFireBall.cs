using System;
using UnityEngine;

public class ProjectileFireBall : MonoBehaviour
{
    public static float fireBallDamage = 100f;
    public float delay = 10f;

    private void Update()
    {
        if (gameObject){
            Destroy(gameObject, delay);    
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(gameObject);
    }
}