using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public Rigidbody2D playerRb;

    //Player Combat Stats:

    public float HP = 10;
    public float speed;
    


    public float jump1;

    // This counts number of jumps the player has, only 2 if double jump (dj) eye is equipped
    public bool hasDJEye;
    private int jumpNum;
    private int jumpDefault;

    public int playerFacingDir;

    public bool CanAttack = true;

    public GameObject crowbarPrefab;


    



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
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.RightArrow))
        {
            playerRb.linearVelocityX = speed;
            playerFacingDir = 1;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            playerRb.linearVelocityX = -speed;
            playerFacingDir = -1;
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
            BasicAttack();
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


}
