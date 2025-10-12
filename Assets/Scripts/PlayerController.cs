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


}
