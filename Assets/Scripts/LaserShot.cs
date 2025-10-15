using UnityEngine;

public class LaserShot : MonoBehaviour
{
    private int enemiesHit = 0;
    public int direction;
    public Rigidbody2D laserRB;
    public float laserSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (direction == 1)
        {
            laserRB.linearVelocityX = laserSpeed;
        }
        else
        {
            laserRB.linearVelocityX = -laserSpeed;
        }
    }

    // Set the direction the player was facing when the laser was fired
    public void SetDirection(int playerFacing)
    {
        direction = playerFacing;
    }    

    // Detects when the laser should be destroyed (currectly set to "after hitting 3 enemies" as an alternate, can be changed)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "Ground")
        {
            Destroy(gameObject);
        }
        else if (collision.collider.gameObject.tag == "Enemy")
        {
            enemiesHit++;
            if (enemiesHit == 3)
            {
                Destroy(gameObject);
            }
        }
    }
}
