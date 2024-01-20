using UnityEngine;

public class ProjectileSmallRock : MonoBehaviour
{
    public static float rockDamage = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(gameObject);
    }
}
