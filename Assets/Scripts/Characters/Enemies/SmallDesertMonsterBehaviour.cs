using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallDesertMonsterBehaviour : MonoBehaviour
{
// IMPLEMENT SERIALIZATION
    private string currentState = "IdleState";
    public Transform target;
    public Animator animator;
    private Rigidbody2D rb;
    public float aggroRange;
    public float attackCooldown;
    private float currentHealth;
    public float maxHealth;
    private bool canAttack;
    private float damageTaken;

    private bool isDead;
    
    private Vector2 _shootVector;
    private Vector2 _shootVectorTop;
    
    public Transform projectileSpawnMid;
    
    private Vector3 pSpawn_Mid;
    
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
        canAttack = true;
        isDead = false;

        pSpawn_Mid = projectileSpawnMid.transform.position;

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
    }

    IEnumerator AttackPlayer()
    {
        canAttack = false;
        animator.SetBool("isIdle", false);
        animator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(0.4f);
        GameObject midBall = Instantiate(projectile);
        GameObject top_midBall = Instantiate(projectile);
        animator.SetBool("isIdle", true);
        animator.SetBool("isAttacking", false);
        
        // 2 normal balls
        midBall.transform.position = pSpawn_Mid;
        midBall.GetComponent<Rigidbody2D>().velocity = _shootVector;
        // upwards spray
        top_midBall.transform.position = pSpawn_Mid;
        top_midBall.GetComponent<Rigidbody2D>().velocity = _shootVectorTop;
        
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
