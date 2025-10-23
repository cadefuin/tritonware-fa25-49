using System.Collections;
using System.Security.Cryptography;
using NUnit.Framework;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    //Enemy stats:

    public int HP = 5;

    public float speed = 2f;


    //Unused behavior as of now
    public bool canJump = false;

    public bool IsStunned = false;

    public bool IsAttacking = false;

    public GameObject player;

    private Coroutine stunCoroutine;

    private Coroutine attackCoroutine;
    private Coroutine laserCoroutine;
    private Coroutine shieldCoroutine;
    private Coroutine dashCoroutine;

    [SerializeField] public bool playerInSight = false;

    [SerializeField] public bool playerInRange = false;

    public Rigidbody2D enemyRB;

    public SpriteRenderer spriteRenderer;

    public GameObject attackPrefab;

    public Animator enemyAnim;

    public int dir;

    public bool CanAttack = true;

    public bool IsElite = false;

    public int shieldHP = 0;

    [SerializeField] public bool playerInAbilityRange = false;

    public GameObject shield;

    public int specialability = -1;

    public bool usingAbility = false;

    public bool abilityOnCD = false;

    public GameObject laserPrefab = null;
    public GameObject dashPrefab = null;
    public GameObject bigshieldPrefab = null;

    void Start()
    {

        // Get nessecary components

        player = GameObject.FindWithTag("Player");
        Rigidbody2D playerRB = player.GetComponent<Rigidbody2D>();
        enemyRB = gameObject.GetComponent<Rigidbody2D>();
        BoxCollider2D colliderP = playerRB.GetComponent<BoxCollider2D>();
        BoxCollider2D colliderE = enemyRB.GetComponent<BoxCollider2D>();

        Transform sight = transform.Find("Sightrange"); // Get first child, which is the sight hitbox
        GameObject sightHitbox = sight.gameObject;
        Transform attackRange = transform.Find("Attackrange"); // Get second child, which is the attack range hitbox
        GameObject attackRangeHitbox = sight.gameObject;

        if (IsElite)
        {
            Transform ability = transform.Find("Abilityrange");
            GameObject abilityRangeHitbox = ability.gameObject;
        }

        //Ignores the physics collisions between enemy and player, allowing the two to walk through each other.
        Physics2D.IgnoreCollision(colliderP, colliderE);
    }

    // Update is called once per frame
    void Update()
    {
        //If the HP goes to 0 or below, delete the enemy
        if (HP <= 0)
        {
            GameManager.enemiesKilled++;
            GameManager.points += 100;

            UnlockAbility(specialability);

            Destroy(gameObject);
        }

        if (shieldHP > 0)
        {
            shield.SetActive(true);
        }
        else
        {
            shield.SetActive(false);
        }


        //If the enemy is stunned, attacking, or has the player in its attack range, stop moving
        if (!usingAbility)
        {

            if (playerInSight && !IsStunned && !IsAttacking && !playerInRange && (abilityOnCD || !playerInAbilityRange))
            {
                if (player.transform.position.x > transform.position.x)
                {
                    enemyRB.linearVelocityX = speed;
                    dir = 1;
                    enemyAnim.SetBool("IsWalking", true);
                    spriteRenderer.flipX = false;
                }
                else
                {
                    enemyRB.linearVelocityX = -1 * speed;
                    dir = -1;
                    spriteRenderer.flipX = true;
                    enemyAnim.SetBool("IsWalking", true);
                }
            }
            else
            {
                enemyRB.linearVelocityX = 0;
                enemyAnim.SetBool("IsWalking", false);
            }

            if (IsElite && playerInAbilityRange && !IsAttacking && !IsStunned)
            {
                if (player.transform.position.x > transform.position.x)
                {
                    dir = 1;
                    SpecialAbility();
                }
                else
                {
                    dir = -1;
                    SpecialAbility();
                }
            }


            if (playerInRange && !IsAttacking && !IsStunned)
            {
                if (player.transform.position.x > transform.position.x)
                {
                    dir = 1;
                    EnemyAttack();
                }
                else
                {
                    dir = -1;
                    EnemyAttack();
                }
            }
        }


        //Change sprite color based on whether or not the enemy is stunned
        //NOTE: This is for debugging, it will be changed to an animation when art is done
        if (IsStunned)
        {
            enemyAnim.SetBool("IsStunned",true);
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);

                if (laserCoroutine != null)
                {
                    StopCoroutine(laserCoroutine);
                }

                if (dashCoroutine != null)
                {
                    StopCoroutine(dashCoroutine);
                enemyAnim.SetBool("IsDashing", false);
                enemyRB.gravityScale = 4;
                enemyRB.linearVelocityX = 0;
                }
                
                CleanUpAttack();

                abilityOnCD = false;
                usingAbility = false;
                


                
            }
        }
        else
        {
            enemyAnim.SetBool("IsStunned",false);
        }
    }
    
    public void CleanUpAttack()
    {
        if(transform.Find("EnemyCrowbar(Clone)") != null)
        {
            Transform leftoverProj = transform.Find("EnemyCrowbar(Clone)"); // Get first child, which is the sight hitbox
            GameObject leftoverObj = leftoverProj.gameObject;
            Destroy(leftoverObj);
        } else if (transform.Find("EnemySlash(Clone)") != null)
        {
            Transform leftoverProj = transform.Find("EnemySlash(Clone)"); // Get first child, which is the sight hitbox
            GameObject leftoverObj = leftoverProj.gameObject;
            Destroy(leftoverObj);
        }
        CanAttack = true;
        IsAttacking = false;
        
        if(transform.Find("EnemyDashAttack(Clone)") != null)
        {
            Transform leftoverProj = transform.Find("EnemyDashAttack(Clone)"); // Get first child, which is the sight hitbox
            GameObject leftoverObj = leftoverProj.gameObject;
            Destroy(leftoverObj);
        }
    }


    //If the player is hit by the attack, stun them and deal damage if already stunned
    //NOTE: If an attack has "bypassStun", it will deal Sdamage regardless if the enemy is stunned or not
    public void HitByAttack(GameObject attack)
    {
        PlayerAttack PA = attack.GetComponent<PlayerAttack>();
        


        if (shieldHP > 0)
        {
            shieldHP -= PA.dmg;
            if (shieldHP < 0)
            {
                HP += shieldHP;
                shieldHP = 0;
                StartCoroutine(RedFlash());

                if (stunCoroutine != null)
                {
                    StopCoroutine(stunCoroutine);
                }
                stunCoroutine = StartCoroutine(StunTimer(PA.stunDuration));
                
            }
        }
        else
        {
            if (PA.bypassStun || IsStunned)
            {
                HP -= PA.dmg;
                StartCoroutine(RedFlash());
            }
            if (stunCoroutine != null)
            {
                StopCoroutine(stunCoroutine);
            }
            stunCoroutine = StartCoroutine(StunTimer(PA.stunDuration));
        }
            
    }

    //Check collisions with player attack hitboxes
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.gameObject.tag == "PlayerAttack")
        {
            HitByAttack(collision.collider.gameObject);
        }
    }



    //Timer that changes the player's stun variable to true for a duration.
    public IEnumerator StunTimer(float stunTime)
    {
        IsStunned = true;
        yield return new WaitForSeconds(stunTime);
        IsStunned = false;

        if (player.transform.position.x > transform.position.x)
        {
            dir = 1;
            spriteRenderer.flipX = false;
        }
        else
        {
            dir = -1;
            spriteRenderer.flipX = true;
        }
                
        
    }

        public void EnemyAttack()
    {
        if (CanAttack)
        {
            CanAttack = false;
            IsAttacking = true;
            attackCoroutine = StartCoroutine(BasicAttackTimer());
        }

    }

    public IEnumerator BasicAttackTimer()
    {

        yield return new WaitForSeconds(0.5f); //Startup value, can be changed
        Debug.Log("Enemy Attack Launched");
        GameObject AttackHitbox = Instantiate(attackPrefab, gameObject.transform.position, Quaternion.identity); // Spawns the enemy attack using the prefab, at the position of the player, with no rotation.
        AttackHitbox.transform.SetParent(gameObject.gameObject.transform); //Set the attack to have the player as parent.
        SpriteRenderer attackSR = AttackHitbox.GetComponent<SpriteRenderer>();
        if (dir == 1)
        { //Setting the position of the hitbox relative to which direction the player is facing
            AttackHitbox.transform.localPosition = new Vector3(1.5f, 0, 0);
            attackSR.flipX = false;
        }
        else
        {
            AttackHitbox.transform.localPosition = new Vector3(-1.5f, 0, 0);
            attackSR.flipX = true;
        }

        yield return new WaitForSeconds(0.25f); //The hitbox will be deleted after this time, can be changed
        Destroy(AttackHitbox);
        CanAttack = true;
        IsAttacking = false;

    }

    public IEnumerator RedFlash()
    {
        for (int i = 0; i < 5; i++)
        {
            spriteRenderer.color = new Color(1, 0.5f + (i * 0.1f), 0.5f + (i * 0.1f));
            yield return new WaitForSeconds(0.05f);
        }
        spriteRenderer.color = Color.white;
    }

    public void SpecialAbility()
    {
        if (!abilityOnCD && specialability != -1)
        {
            Debug.Log("Special Ability Launched!");
            usingAbility = true;
            abilityOnCD = true;

            if (specialability == 0)
            {
                laserCoroutine = StartCoroutine(ShootLaser());
            } else if (specialability == 1)
            {
                dashCoroutine = StartCoroutine(Dash());
            } else if (specialability == 2)
            {
                shieldCoroutine = StartCoroutine(BigShield());
            } else if (specialability == 3)
            {
                
            }


        }
    }
    public IEnumerator Dash()
    {
        yield return new WaitForSeconds(1.25f);

        enemyRB.gravityScale = 0;
        enemyRB.linearVelocityY = 0;

        GameObject dashEffect = Instantiate(dashPrefab, gameObject.transform.position, Quaternion.identity);
        SpriteRenderer dashImg = dashEffect.GetComponent<SpriteRenderer>();
        dashEffect.transform.SetParent(gameObject.gameObject.transform);

        enemyAnim.SetBool("IsDashing", true);
        if (dir == 1)
        {
            enemyRB.linearVelocityX = speed * 4;
            dashImg.flipX = false;
        }
        else
        {
            enemyRB.linearVelocityX = speed * -4;
            dashImg.flipX = true;
        }
        yield return new WaitForSeconds(0.5f);

        enemyAnim.SetBool("IsDashing", false);
        enemyRB.gravityScale = 4;

        Destroy(dashEffect);

        usingAbility = false;

        yield return new WaitForSeconds(8f);
        abilityOnCD = false;

    }

    public IEnumerator BigShield()
    {
        yield return new WaitForSeconds(1);

        GameObject BigShield = Instantiate(bigshieldPrefab, gameObject.transform.position, Quaternion.identity);
        BigShield.transform.SetParent(gameObject.gameObject.transform);
        yield return new WaitForSeconds(2);
        usingAbility = false;

        yield return new WaitForSeconds(15f);
        abilityOnCD = false;
    }

    public IEnumerator ShootLaser()
    {
        yield return new WaitForSeconds(1f);
        GameObject LaserHitbox = Instantiate(laserPrefab, gameObject.transform.position, Quaternion.identity); // Spawns the enemy attack using the prefab, at the position of the player, with no rotation.
        LaserHitbox.transform.SetParent(gameObject.gameObject.transform);
        SpriteRenderer attackSR = LaserHitbox.GetComponent<SpriteRenderer>();
        LaserShot newShot = LaserHitbox.GetComponent<LaserShot>();
        newShot.SetDirection(dir);
        if (dir == 1)
        { //Setting the position of the hitbox relative to which direction the player is facing
            LaserHitbox.transform.localPosition = new Vector3(0f, 0, 0);
            attackSR.flipX = false;
        }
        else
        {
            LaserHitbox.transform.localPosition = new Vector3(0f, 0, 0);
            attackSR.flipX = true;
        }
        usingAbility = false;

        yield return new WaitForSeconds(5);
        Destroy(LaserHitbox);
        abilityOnCD = false;
    }
    
    public void UnlockAbility(int i)
    {
        if(i > -1)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if(pc.collectedAbilities[i] == 0)
            {
                pc.collectedAbilities[i] = 1;
                pc.selectedAbility = i;
            }
        }
    }
}
