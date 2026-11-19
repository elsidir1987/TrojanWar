using UnityEngine;
using System.Collections;

public class ArcherAI : MonoBehaviour
{
    [Header("Enemy Health")]
    public float maxHealth = 1f;
    private float currentHealth;
    private bool isDead = false;
    private bool isHit = false;

    [Header("Combat Settings")]
    public Transform player;
    public float speed = 4f;          // speed of the archer when chasing the player
    public float chaseRange = 20f;    // distance at which the archer starts chasing the player
    public float attackRange = 10f;
    public float fireRate = 0.8f;     // fire rate of the archer
    private float nextFireTime = 0f;

    [Header("Ranged Weapons")]
    public GameObject arrowPrefab; 
    public Transform firePoint;    

    private Animator anim;
    private Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (player == null || isDead || isHit) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 1. ATTACK (If within attack range)
        if (distanceToPlayer <= attackRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Stops to shoot
            anim.SetFloat("Speed", 0f); 
            FacePlayer();

            if (Time.time >= nextFireTime)
            {
                Attack();
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
        // 2. CHASING (If player is far but within chase range)
        else if (distanceToPlayer <= chaseRange)
        {
            ChasePlayer();
        }
        // 3. IDLE
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetFloat("Speed", 0f);
        }
    }

    void ChasePlayer()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        
        anim.SetFloat("Speed", 1f); //enter running animation
        FacePlayer();
    }

    void FacePlayer()
    {
        if (player.position.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else if (player.position.x < transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void Attack()
    {
        anim.SetTrigger("Attack");
        StartCoroutine(ShootArrowRoutine());
    }

    IEnumerator ShootArrowRoutine()
    {
        yield return new WaitForSeconds(0.45f);
        if (!isDead && !isHit && arrowPrefab != null && firePoint != null)
        {
            GameObject newArrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
            Arrow arrowScript = newArrow.GetComponent<Arrow>();
            if(arrowScript != null) arrowScript.Shoot(player); 
        }
    }

    public void TakeDamage(float damage)
    {
        // Log to check if the player's sword is actually hitting the archer
        Debug.Log("Archer took " + damage + " damage! Remaining health: " + (currentHealth - damage));
        
        if (isDead) return;
        
        currentHealth -= damage;
        StopAllCoroutines(); 

        if (currentHealth <= 0) 
        {
            if (SFXManager.instance != null)
        {
            SFXManager.instance.PlaySound(SFXManager.instance.enemyDeath);
        }
            Die();
        }
        else 
        {
            // Only trigger "Hit" if the archer survives the attack
            anim.SetTrigger("Hit"); 
            isHit = true;
            StartCoroutine(RecoverFromHit());
        }
    }
    // Coroutine to handle the hit stun duration
    IEnumerator RecoverFromHit()
    {
        // Wait for half a second before the archer can move/attack again
        yield return new WaitForSeconds(0.5f); 
        isHit = false;
    }

    void Die()
    {
        // Log to confirm the death function is triggered
        Debug.Log("Archer is dying! Playing death animation.");
        isDead = true;
        
        anim.SetTrigger("Death"); 
        
        
        rb.gravityScale = 0f; 
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        
        LevelGoalManager goal = FindFirstObjectByType<LevelGoalManager>();
        if (goal != null) goal.AddKill();

        Destroy(gameObject, 3f);
    }

    
}