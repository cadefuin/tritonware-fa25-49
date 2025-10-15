using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D playerRb;
    public float speed;
    public float jump1;

    // This counts number of jumps the player has, only 2 if double jump (dj) eye is equipped
    public bool hasDJEye;
    private int jumpNum;
    private int jumpDefault;

<<<<<<< Updated upstream
=======
    public bool hasLBEye;

    public int playerFacingDir;

    public bool CanAttack = true;
    public bool CanFireLaser = true;

    public GameObject crowbarPrefab;
    public GameObject laserPrefab;

    public float laserCooldown;



>>>>>>> Stashed changes
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
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.RightArrow))
        {
            playerRb.linearVelocityX = speed;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            playerRb.linearVelocityX = -speed;
        }
        else
        {
            playerRb.linearVelocityX = 0;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && jumpNum > 0)
        {
            playerRb.linearVelocityY = jump1;
            jumpNum -= 1;
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CrowbarAttack.Attack();
        }    
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            LaserFire();
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // note: wall sliding should not restore jump, so ground will need to have
        // untagged colliders for walls
        if (collision.collider.gameObject.tag == "Ground")
        {
            jumpNum = jumpDefault;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // This makes sure that player cannot jump after walking off a ledge (can still use double jump if they have it)
        if (collision.collider.gameObject.tag == "Ground" && hasDJEye)
        {
            jumpNum = 1;
        }
        else if (collision.collider.gameObject.tag == "Ground")
        {
            jumpNum = 0;
        }
    }

<<<<<<< Updated upstream
=======
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
        Debug.Log("Attack Launched");
        GameObject CBHitbox = Instantiate(crowbarPrefab, gameObject.transform.position, Quaternion.identity); // Spawns the crowbar using the prefab, at the position of the player, with no rotation.
        CBHitbox.transform.SetParent(gameObject.gameObject.transform); //Set the crowbar to have the player as parent.
        if(playerFacingDir == 1){ //Setting the position of the hitbox relative to which direction the player is facing
            CBHitbox.transform.localPosition = new Vector3(1,0,0);
        } else {
            CBHitbox.transform.localPosition = new Vector3(-1,0,0);
        }
        
        yield return new WaitForSeconds(0.25f); //The hitbox will be deleted after this time, can be changed
        Destroy(CBHitbox);
        CanAttack = true;
        
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
>>>>>>> Stashed changes

}
