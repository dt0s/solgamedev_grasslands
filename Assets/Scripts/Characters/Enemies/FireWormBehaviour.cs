using System;
using System.Collections;
using System.Threading;
using UnityEngine;

public class FireWormBehaviour : MonoBehaviour
{
    // IMPLEMENT SERIALIZATION
    private string currentState = "IdleState";
    public Transform target;
    public Animator animator;
    private Rigidbody2D rb;
    public float speed;
    public float aggroRange;
    public float attackCooldown;
    public float secondCooldown;
    private float currentHealth;
    public float maxHealth;
    private bool canAttack;
    private float damageTaken;

    private bool isDead;
    private bool cycleIsRunning;
    
    private Vector2 _shootVector;
    private Vector2 _shootVectorTop;
    private Vector2 _shootVectorDown;
    
    public Transform projectileSpawnLow;
    public Transform projectileSpawnMid;
    public Transform projectileSpawnHigh;
    
    private Vector3 pSpawn_Low;
    private Vector3 pSpawn_Mid;
    private Vector3 pSpawn_High;
    
    [SerializeField] private GameObject projectile;
    
    public float projectileX;
    public float projectileY;
    public float angleCorrection;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        animator.SetBool("isIdle", true);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isDead", false);
        
        //set up
        _shootVector = new Vector2(projectileX, projectileY);
        _shootVectorTop = new Vector2(projectileX, projectileY + angleCorrection);
        _shootVectorDown = new Vector2(projectileX, projectileY - angleCorrection);
        canAttack = true;
        isDead = false;

        pSpawn_Low = projectileSpawnLow.transform.position;
        pSpawn_Mid = projectileSpawnMid.transform.position;
        pSpawn_High = projectileSpawnHigh.transform.position;

    }
    

    private void Update()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= aggroRange && canAttack && !isDead)
        {
            StartCoroutine(AttackPlayer());
        }
        if (distance > aggroRange && !animator.GetBool("isDead"))
        {
            animator.SetBool("isIdle", true);
            animator.SetBool("isAttacking", false);
        }

        if (currentHealth <= 30 && !cycleIsRunning)
        {
            cycleIsRunning = true;
            attackCooldown = secondCooldown;
        }

    }

    IEnumerator AttackPlayer()
    {
        canAttack = false;
        animator.SetBool("isIdle", false);
        animator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(0.4f);
        animator.SetBool("isIdle", true);
        animator.SetBool("isAttacking", false);
        GameObject lowBall = Instantiate(projectile);
        GameObject midBall = Instantiate(projectile);
        GameObject highBall = Instantiate(projectile);
        
        GameObject top_lowBall = Instantiate(projectile);
        GameObject top_midBall = Instantiate(projectile);
        GameObject top_highBall = Instantiate(projectile);

        GameObject bot_lowBall = Instantiate(projectile);
        GameObject bot_midBall = Instantiate(projectile);
        GameObject bot_highBall = Instantiate(projectile);

        
        // 3 normal balls
        lowBall.transform.position = pSpawn_Low;
        midBall.transform.position = pSpawn_Mid;
        highBall.transform.position = pSpawn_High;
        lowBall.GetComponent<Rigidbody2D>().velocity = _shootVector;
        midBall.GetComponent<Rigidbody2D>().velocity = _shootVector;
        highBall.GetComponent<Rigidbody2D>().velocity = _shootVector;
        // upwards spray
        top_lowBall.transform.position = pSpawn_Low;
        top_midBall.transform.position = pSpawn_Mid;
        top_highBall.transform.position = pSpawn_High;
        top_lowBall.GetComponent<Rigidbody2D>().velocity = _shootVectorTop;
        top_midBall.GetComponent<Rigidbody2D>().velocity = _shootVectorTop;
        top_highBall.GetComponent<Rigidbody2D>().velocity = _shootVectorTop;
        // downwards spray
        bot_lowBall.transform.position = pSpawn_Low;
        bot_midBall.transform.position = pSpawn_Mid;
        bot_highBall.transform.position = pSpawn_High;
        bot_lowBall.GetComponent<Rigidbody2D>().velocity = _shootVectorDown;
        bot_midBall.GetComponent<Rigidbody2D>().velocity = _shootVectorDown;
        bot_highBall.GetComponent<Rigidbody2D>().velocity = _shootVectorDown;
        
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other)
        {
            if (other.tag == "Projectile")
            {
                animator.SetTrigger("damage");
                damageTaken = ProjectileSmallRock.rockDamage;
                TakeDamage(damageTaken);
            }
        }
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log(currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        //play death animation
        animator.SetBool("isIdle", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isDead", true);
        //disable the script and the collider and enable the deadCollider
        //GetComponent<PolygonCollider2D>().isTrigger = true;
        PolygonCollider2D wormAlive = GetComponent<PolygonCollider2D>();
        wormAlive.enabled = false;
        rb.isKinematic = true;
    }
}
