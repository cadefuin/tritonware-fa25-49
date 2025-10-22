using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{

    public Rigidbody2D playerRb;

    //Player Combat Stats:

    public float HP = 5;
    public float speed;
    


    public float jump1;

    // This counts number of jumps the player has, only 2 if double jump (dj) eye is equipped
    public bool hasDJEye;
    [SerializeField] private int jumpNum;
    private int jumpDefault;

    public int playerFacingDir;

    public bool CanAttack = true;

    public GameObject crowbarPrefab;

    public bool Iframes = false;

    private Coroutine IFramesCouroutine;

    public float IFramesDuration = 1;

    public SpriteRenderer spriteRenderer;

    public bool hasLBEye;
    public bool CanFireLaser = true;
    public GameObject laserPrefab;

    public GameObject dashPrefab;

    public GameObject shield;
    public float laserCooldown;

    public int selectedAbility = -1;

    public int[] collectedAbilities = new int[3];

    [SerializeField] private Animator playerAnimator;

    public int shieldHP = 0;

    private bool CanShield = true;

    private bool CanDash = true;

    private bool IsDashing = false;


    



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Checks how many jumps player should have
        if (hasDJEye)
        {
            jumpDefault = 2;
        }
        else
        {
            jumpDefault = 1;
        }
        playerFacingDir = 1;

        collectedAbilities[0] = 1;
        collectedAbilities[1] = 1;
        collectedAbilities[2] = 1;
    }

    // Update is called once per frame
    void Update()
    {

        if (!GameManager.IsPaused && !GameManager.DialogOn)
        {
            if (!IsDashing)
            {
                    if (Input.GetKey(KeyCode.RightArrow))
                {
                    playerRb.linearVelocityX = speed;
                    playerFacingDir = 1;
                    playerAnimator.SetBool("IsWalking", true);
                    spriteRenderer.flipX = false;
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    playerRb.linearVelocityX = -speed;
                    playerFacingDir = -1;
                    playerAnimator.SetBool("IsWalking", true);
                    spriteRenderer.flipX = true;
                }
                else
                {
                    playerRb.linearVelocityX = 0;
                    playerAnimator.SetBool("IsWalking", false);
                }

                if (Input.GetKeyDown(KeyCode.Z) && jumpNum > 0)
                {
                    playerRb.linearVelocityY = jump1;
                    jumpNum -= 1;
                    playerAnimator.SetBool("IsJumping", true);
                }



                if (Input.GetKeyDown(KeyCode.X))
                {
                    BasicAttack();
                }

                if (Input.GetKeyDown(KeyCode.C))
                {
                    SpecialAbility();
                }
            }

            

            if (Input.GetKeyDown(KeyCode.S))
            {
                if (selectedAbility == -1)
                {
                    selectedAbility++;
                }
                
                    selectedAbility--;
                    if (selectedAbility <= -1)
                    {
                        selectedAbility = 2;
                    }

                while(collectedAbilities[selectedAbility] == 0)
                {

                    if (collectedAbilities[0] == 0 && collectedAbilities[1] == 0 && collectedAbilities[2] == 0)
                    {
                        selectedAbility = -1;
                        break;
                    }

                    selectedAbility--;
                    if (selectedAbility <= -1)
                    {
                        selectedAbility = 2;
                    }

                }
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                if (selectedAbility == -1)
                {
                    selectedAbility++;
                }

                selectedAbility++;
                if (selectedAbility >= 3)
                {
                    selectedAbility = 0;
                }
                    
                
                while(collectedAbilities[selectedAbility] == 0)
                {
                    if (collectedAbilities[0] == 0 && collectedAbilities[1] == 0 && collectedAbilities[2] == 0)
                    {
                        selectedAbility = -1;
                        break;
                    }
                    
                    selectedAbility++;
                    if (selectedAbility >= 3)
                    {
                        selectedAbility = 0;
                    }
                }

            }
            


        }

        if (GameManager.DialogOn)
        {
            playerRb.linearVelocityX = 0;
            playerAnimator.SetBool("IsWalking", false);
        }

        //Debug.DrawRay(transform.position, Vector2.down *(spriteRenderer.bounds.extents.y + 0.1f), Color.red);

        if (Iframes)
        {
            spriteRenderer.color = new Color(0.8f,0.8f,0.8f);
        }
        else
        {
            spriteRenderer.color = Color.white;
        }

        if (HP <= 0)
        {
            gameObject.transform.position = GameManager.getRespawnPos();
            HP = 5;
        }

        if (gameObject.transform.position.y < -100)
        {
            HP = 0;
        }
        
        if(shieldHP > 0)
        {
            shield.SetActive(true);
        } else
        {
            shield.SetActive(false);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // note: wall sliding should not restore jump, so ground will need to have
        // untagged colliders for walls
        if (collision.collider.gameObject.tag == "Ground" && IsGrounded())
        {
            jumpNum = jumpDefault;
        }
        else if (collision.collider.gameObject.tag == "EnemyAttack")
        {
            HitByAttack(collision.collider.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Checkpoint"))
        {
            GameManager.setRespawnPos(gameObject.transform.position);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "Ground" && !IsDashing && playerRb.linearVelocityY == 0)
        {
            playerAnimator.SetBool("IsJumping", false);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // This makes sure that player cannot jump after walking off a ledge (can still use double jump if they have it)
        if (collision.collider.gameObject.tag == "Ground" && hasDJEye && !IsGrounded())
        {
            if(jumpNum == 2)
            {
                jumpNum = 1;
            }
        }
        else if (collision.collider.gameObject.tag == "Ground"&& !IsGrounded())
        {
            jumpNum = 0;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, spriteRenderer.bounds.extents.y + 0.1f, LayerMask.GetMask("Ground"));
    }

    //Function that makes the player use their basic attack.
    public void BasicAttack()
    {
        if (CanAttack)
        {
            CanAttack = false;
            StartCoroutine(BasicAttackTimer());
        }

    }

    //Coroutine that deploys the attack hitbox and deletes it at the right time.
    public IEnumerator BasicAttackTimer()
    {

        yield return new WaitForSeconds(0.10f); //Startup value, can be changed
        //Debug.Log("Attack Launched");
        GameObject CBHitbox = Instantiate(crowbarPrefab, gameObject.transform.position, Quaternion.identity); // Spawns the crowbar using the prefab, at the position of the player, with no rotation.
        SpriteRenderer CBSprite = CBHitbox.GetComponent<SpriteRenderer>();
        CBHitbox.transform.SetParent(gameObject.gameObject.transform); //Set the crowbar to have the player as parent.
        if (playerFacingDir == 1)
        { //Setting the position of the hitbox relative to which direction the player is facing
            CBHitbox.transform.localPosition = new Vector3(1.25f, 0, 0);
            CBSprite.flipX = false;
        }
        else
        {
            CBHitbox.transform.localPosition = new Vector3(-1.25f, 0, 0);
            CBSprite.flipX = true;
        }

        yield return new WaitForSeconds(0.25f); //The hitbox will be deleted after this time, can be changed
        Destroy(CBHitbox);
        CanAttack = true;

    }

        public void HitByAttack(GameObject attack)
    {
        EnemyAttack EA = attack.GetComponent<EnemyAttack>();
        if (!Iframes)
        {
            if(shieldHP > 0)
            {
                shieldHP -= EA.dmg;
                if (shieldHP < 0)
                {
                    HP += shieldHP;
                    shieldHP = 0;
                    if (IFramesCouroutine == null){
                    IFramesCouroutine = StartCoroutine(IFramesTimer());
                    }
                }
            } else {
                HP -= EA.dmg;
                if (IFramesCouroutine == null){
                IFramesCouroutine = StartCoroutine(IFramesTimer());
                }
            }
            
        }
        
        
    }
    public IEnumerator IFramesTimer()
    {
        Iframes = true;
        for (float i = 0; i < IFramesDuration; i += 0.1f)
        {
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.enabled = !spriteRenderer.enabled;
        }
        spriteRenderer.enabled = true;
        Iframes = false;
        IFramesCouroutine = null;
    }

    public void LaserFire()
    {
        if (CanFireLaser)
        {
            CanFireLaser = false;
            StartCoroutine(BeamShotTimer());
        }
    }
    

    // Creates a new instance of LaserShot
    public IEnumerator BeamShotTimer()
    {
        GameObject laserHitbox = Instantiate(laserPrefab, gameObject.transform.position, Quaternion.identity);
        LaserShot newShot = laserHitbox.GetComponent<LaserShot>();

        newShot.SetDirection(playerFacingDir);

        yield return new WaitForSeconds(laserCooldown);
        // Destroys if it hasn't hit something yet
        Destroy(laserHitbox);
        CanFireLaser = true;
    }

    public void Dash()
    {
        if (CanDash)
        {
            CanDash = false;
            StartCoroutine(DashTimer());
        }
    }

    public IEnumerator DashTimer()
    {
        IsDashing = true;
        playerRb.gravityScale = 0;
        playerRb.linearVelocityY = 0;

        GameObject dashEffect = Instantiate(dashPrefab, gameObject.transform.position, Quaternion.identity);
        SpriteRenderer dashImg = dashEffect.GetComponent<SpriteRenderer>();
        dashEffect.transform.SetParent(gameObject.gameObject.transform);

        playerAnimator.SetBool("IsDashing", true);
        if (playerFacingDir == 1)
        {
            playerRb.linearVelocityX = speed * 3;
            dashImg.flipX = false;
        }
        else
        {
            playerRb.linearVelocityX = speed * -3;
            dashImg.flipX = true;
        }
        yield return new WaitForSeconds(0.2f);

        playerAnimator.SetBool("IsDashing", false);
        playerRb.gravityScale = 4;

        Destroy(dashEffect);

        IsDashing = false;

        yield return new WaitForSeconds(2.3f);
        CanDash = true;

    }

    public void Shield()
    {
        if (CanShield)
        {
            CanShield = false;
            StartCoroutine(ShieldTimer());
        }
    }
    
    public IEnumerator ShieldTimer()
    {
        shieldHP = 5;
        yield return new WaitForSeconds(5);
        shieldHP = 0;
        yield return new WaitForSeconds(10);
        CanShield = true;
    }

    public void SpecialAbility()
    {
        if (collectedAbilities[0] != 0 || collectedAbilities[1] != 0 || collectedAbilities[2] != 0)
        {
            if (selectedAbility == 0)
            {
                LaserFire();
            } else if(selectedAbility == 1)
            {
                Dash();
            } else if (selectedAbility == 2)
            {
                Shield();
            }
        }
    }
    
    public void CollectAbility(int i)
    {
        collectedAbilities[i] = 1;
    }


}
