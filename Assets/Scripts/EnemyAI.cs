using UnityEngine;
using System.Collections; // Required for Coroutines 

public class EnemyAI : MonoBehaviour
{
    [Header("Enemy Health")]
    public float maxHealth = 3f;       // How many hits he can take 
    private float currentHealth;
    private bool isDead = false;       // Is he alive? 
    private bool isHit = false;        // Is he currently feeling pain? 

    [Header("Enemy Settings")]
    public float speed = 8f;
    public float chaseDistance = 150f;
    public float attackDistance = 1.5f; 

    [Header("Combat Settings")]
    public Transform attackPoint;      
    public float attackRange = 0.5f;   
    public float attackCooldown = 0.7f;
    private bool isAttacking = false;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        
        currentHealth = maxHealth; // Start with full health 

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        // If he is dead or player doesn't exist, do nothing! 
        if (player == null || isDead) return;

        // If he is attacking or getting hit, stop moving 
        if (isAttacking || isHit)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float direction = Mathf.Sign(player.position.x - transform.position.x);

        // ALWAYS face the player, regardless of distance!
        if (direction > 0) 
        {
            sprite.flipX = false;
            if (attackPoint != null) attackPoint.localPosition = new Vector3(Mathf.Abs(attackPoint.localPosition.x), attackPoint.localPosition.y, 0);
        }
        else if (direction < 0) 
        {
            sprite.flipX = true;
            if (attackPoint != null) attackPoint.localPosition = new Vector3(-Mathf.Abs(attackPoint.localPosition.x), attackPoint.localPosition.y, 0);
        }

        // State Machine: Chase, Attack, or Idle
        if (distanceToPlayer < chaseDistance && distanceToPlayer > attackDistance)
        {
            // Move towards player
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
            if (anim != null) anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        }
        else if (distanceToPlayer <= attackDistance)
        {
            // In range, stop and attack
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            if (anim != null) anim.SetFloat("Speed", 0f);
            
            if (!isAttacking)
            {
                StartCoroutine(AttackRoutine());
            }
        }
        else
        {
            // Out of range, stop completely
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            if (anim != null) anim.SetFloat("Speed", 0f);
        }
    }
    
    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        if (anim != null) anim.SetTrigger("Attack");
        
        yield return new WaitForSeconds(0.3f); 

        if (attackPoint != null && !isDead && !isHit) // Only do damage if still alive and not stunned! 
        {
            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);
            foreach(Collider2D hit in hitObjects)
            {
                if (hit.CompareTag("Player"))
                {
                    hit.GetComponent<PlayerMovement>().TakeDamage(1f);
                    break; 
                }
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    // Call this from Ajax's sword script when he hits the enemy! 
    public void TakeDamage(float damage)
    {
        Debug.Log("OUCH! Ajax hit me!");
        if (isDead || isHit) return;

        currentHealth -= damage;
        Debug.Log("Enemy Health is now: " + currentHealth);
        
        // Stop current attack if getting hit 
        StopAllCoroutines(); 
        isAttacking = false;

        if (currentHealth > 0)
        {
            StartCoroutine(HitRoutine());
        }
        else
        {
            Die();
        }
    }

    IEnumerator HitRoutine()
    {
        isHit = true;
        if (anim != null) anim.SetTrigger("Hit");
        
        // Wait for the hit animation to finish before moving again 
        yield return new WaitForSeconds(0.5f); 
        isHit = false;
    }
   void Die()
    {
        isDead = true;

        if (SFXManager.instance != null)
        {
            SFXManager.instance.PlaySound(SFXManager.instance.enemyDeath);
        }

        LevelGoalManager goal = FindFirstObjectByType<LevelGoalManager>();
        if (goal != null) goal.AddKill();
        
        if (anim != null) anim.SetTrigger("Death");
        
        // Set gravity to make him fall down and disable his collider so he doesn't block the player anymore
        rb.gravityScale = 3f;
        gameObject.tag = "Untagged";

        
        // Get the enemy's collider and tell Unity to ignore collisions between it and the player's colliders, so Ajax can walk through the dead body without getting stuck!
        Collider2D myCollider = GetComponent<Collider2D>();
        
        // Find all colliders on the player (in case he has more than one) and ignore collisions with each of them
        if (player != null && myCollider != null)
        {
            Collider2D[] playerColliders = player.GetComponentsInChildren<Collider2D>();
            foreach (Collider2D pc in playerColliders)
            {
                // Ignore the collision (true)
                Physics2D.IgnoreCollision(myCollider, pc, true);
            }
        }

        // Disappears after 3 seconds
        Destroy(gameObject, 3f);
        this.enabled = false; 
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}