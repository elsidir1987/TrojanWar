using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float jumpForce = 7f;

    [Header("Health UI")]
    public Image healthBarFill;
    public float maxHealth = 3f;
    
    [Header("Combat Settings")]
    public Transform attackPoint;    // The center of Ajax's sword hit 
    public float attackRange = 0.5f; // How big the hit area is 
    public float attackDamage = 1f;  // How much damage Ajax deals 

    [Header("Shield Settings")]
    public float detectionRadius = 3f; // How close an enemy must be for Ajax to block
    public bool isBlocking = false;
    
    private float currentHealth; 

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;

    private PlayerEnergy energyScript;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // --- PLAYER PREFS: LOAD HEALTH ---
        if (PlayerPrefs.HasKey("PlayerHealth"))
        {
            currentHealth = PlayerPrefs.GetFloat("PlayerHealth");
        }
        else
        {
            currentHealth = maxHealth;
            PlayerPrefs.SetFloat("PlayerHealth", currentHealth);
            PlayerPrefs.Save();
        }

        // --- FIND HEALTH BAR ---
        GameObject barObj = GameObject.FindWithTag("HealthBar");
        if (barObj != null)
        {
            healthBarFill = barObj.GetComponent<Image>();
        }
        
        // Corrected spelling from GetComponet to GetComponent 
        energyScript = GetComponent<PlayerEnergy>();

        UpdateHealthBar();
    }
    
    void Update()
    {
        // --- CUSTOM MOVEMENT INPUT (A and D keys) --- 
        float moveInput = 0f;
        if (Input.GetKey(KeyCode.D)) 
        {
            moveInput = 1f; // Move right 
        }
        else if (Input.GetKey(KeyCode.A)) 
        {
            moveInput = -1f; // Move left 
        }

        // --- MOVEMENT ---
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        // --- ANIMATION LOGIC ---
        anim.SetFloat("Speed", Mathf.Abs(moveInput));

        // --- FLIPPING THE SPRITE & ATTACK POINT ---
        if (moveInput > 0) 
        {
            sprite.flipX = false;
            // Move attack point to the right 
            if (attackPoint != null) attackPoint.localPosition = new Vector3(Mathf.Abs(attackPoint.localPosition.x), attackPoint.localPosition.y, 0);
        }
        else if (moveInput < 0) 
        {
            sprite.flipX = true;
            // Move attack point to the left 
            if (attackPoint != null) attackPoint.localPosition = new Vector3(-Mathf.Abs(attackPoint.localPosition.x), attackPoint.localPosition.y, 0);
        }

        // --- ATTACK LOGIC ---
        if (Input.GetKeyDown(KeyCode.Space))
        {       
            Attack();
        }

        // --- JUMPING WITH ENERGY (W key) --- 
        // Check if W is pressed and player is on the ground 
        if (Input.GetKeyDown(KeyCode.W) && Mathf.Abs(rb.linearVelocity.y) < 0.1f)
        {
            bool canJump=true;
            //if energy system exists,check and use it 
            if (energyScript!=null) 
            {
                if(energyScript.CanPerformAction())
                {
                    canJump=true;
                    energyScript.UseEnergy(); // Spend 1 slot 
                }
                else
                {
                    canJump=false;
                }
            }
            if(canJump)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            
        }

        // --- SHIELD BLOCK LOGIC ---
        if (Input.GetKey(KeyCode.LeftShift)) 
        {
            isBlocking = true;
            if(anim != null) 
            {
                anim.SetBool("Block", true);

            }
            
            // Reduce speed or stop while blocking
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); 
            return;
        }
        else 
        {
            DetectIncomingArrows();
        }
       
        // Force the UI to ALWAYS show the correct health 
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
        else 
        {
            GameObject barObj = GameObject.FindWithTag("HealthBar");
            if (barObj != null)
            {
                healthBarFill = barObj.GetComponent<Image>();
            }
        }
    }

     void DetectIncomingArrows()
    {
        // scan for colliders in a circle around Ajax with the specified detection radius
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        
        bool arrowIsClose = false;

        foreach (var obj in detectedObjects)
        {
            if (obj.CompareTag("Arrow"))
            {
                arrowIsClose = true;
                break;
            }
        }

        // inform the animator to play the block animation if an arrow is close
        isBlocking = arrowIsClose;
        anim.SetBool("Block", isBlocking); 
    }

    // Function to handle sword attack and hitting enemies 
    void Attack()
    {
        // 1. Check Energy
        if(energyScript != null)
        {
            if (!energyScript.CanPerformAction()) return; 
            energyScript.UseEnergy(); 
        }
        
        // 2. Play Animation
        if (anim != null) anim.SetTrigger("Attack1");

        // Play the sword attack sound!
        if (SFXManager.instance != null)
        {
            SFXManager.instance.PlaySound(SFXManager.instance.swordAttack);
        }

        // 3. If no attack point is assigned, return to avoid errors
        if (attackPoint == null) return;

        // 4. Find all enemies in range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);

        foreach(Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                // CASE A: Melee Enemy
                EnemyAI meleeEnemy = enemy.GetComponent<EnemyAI>();
                if (meleeEnemy != null) 
                {
                    meleeEnemy.TakeDamage(attackDamage);
                }

                // CASE B: Ranged Enemy (Archer)
                ArcherAI rangedEnemy = enemy.GetComponent<ArcherAI>();
                if (rangedEnemy != null) 
                {
                    rangedEnemy.TakeDamage(attackDamage);
                    Debug.Log("Ajax's sword hit: ArcherEnemy | Tag: Untagged");
                }
            }
        }
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            TakeDamage(1f);// Instantly kill the player if they touch water

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);// Reload the current scene to reset everything
        }
        else if (other.gameObject.CompareTag("EnemyAttack"))
        {
            // Take damage from enemy and calculate knockback direction
            float damage = 1f; // Modify if you want different damages
            TakeDamage(damage, other.transform.position);
        }
    }
    
    public void TakeDamage(float damage,Vector2 enemyPosition = default(Vector2))
    {

        // If Ajax is currently blocking, he takes no damage and we can exit the function early.
        if (isBlocking) 
        {
            Debug.Log("Ajax blocked the arrow with his shield!");
            return; 
        }
        currentHealth -= damage;
        
        PlayerPrefs.SetFloat("PlayerHealth", currentHealth);
        PlayerPrefs.Save();
        
        UpdateHealthBar();

        
        // Check if health dropped to zero or below
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Player took damage but is still alive.
            // Trigger the Hurt animation! 
            if (anim != null)
            {
                anim.SetTrigger("Hurt"); 
            }

            // Play the player hurt sound!
            if (SFXManager.instance != null)
            {
                SFXManager.instance.PlaySound(SFXManager.instance.playerHurt);
            }
            
            Debug.Log("Player took damage, but is still alive! Current Health: " + currentHealth);
        }
    }
    // Dedicated function to handle player death
    void Die()
    {
        Debug.Log("Ajax has died!");

        PlayerPrefs.SetFloat("PlayerHealth", maxHealth);
        PlayerPrefs.Save();
        PlayerPrefs.SetInt("LastLevel", SceneManager.GetActiveScene().buildIndex);
            
        // Trigger the Death animation! 
        if (anim != null) 
        {
            anim.SetTrigger("Death"); 
        }
        // Play the player hurt sound!
            if (SFXManager.instance != null)
            {
                SFXManager.instance.PlaySound(SFXManager.instance.playerHurt);
            }

        // Start the coroutine to wait a bit, then load Game Over
        StartCoroutine(WaitAndDie());
    }
    
    
    // Coroutine to handle the death delay
    System.Collections.IEnumerator WaitAndDie()
    {
        // Disable the script so the player can't move while dying
        this.enabled = false;
        
        
        // Wait for 1 second
        yield return new WaitForSeconds(1f);
        
        // Load the GameOver scene instead of reloading the active scene
        SceneManager.LoadScene("GameOver");
        
    }
    
    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    // Draw a blue circle for Ajax's sword in Unity 
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}