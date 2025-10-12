using System.Collections;
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

    private Coroutine stunCouroutine;

    [SerializeField] public bool playerInSight = false;

    [SerializeField] public bool playerInRange = false;

    public Rigidbody2D enemyRB;

    public SpriteRenderer spriteRenderer;

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

        //Ignores the physics collisions between enemy and player, allowing the two to walk through each other.
        Physics2D.IgnoreCollision(colliderP, colliderE);
    }

    // Update is called once per frame
    void Update()
    {
        //If the HP goes to 0 or below, delete the enemy
        if (HP <= 0)
        {
            Destroy(gameObject);
        }


        //If the enemy is stunned, attacking, or has the player in its attack range, stop moving
        if (playerInSight && !IsStunned && !IsAttacking && !playerInRange)
        {
            if (player.transform.position.x > transform.position.x)
            {
                enemyRB.linearVelocityX = speed;
            }
            else
            {
                enemyRB.linearVelocityX = -1 * speed;
            }
        }
        else
        {
            enemyRB.linearVelocityX = 0;
        }


        //Change sprite color based on whether or not the enemy is stunned
        //NOTE: This is for debugging, it will be changed to an animation when art is done
        if (IsStunned)
        {
            spriteRenderer.color = Color.yellow;
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }


    //If the player is hit by the attack, stun them and deal damage if already stunned
    //NOTE: If an attack has "bypassStun", it will deal damage regardless if the enemy is stunned or not
    public void HitByAttack(GameObject attack)
    {
        PlayerAttack PA = attack.GetComponent<PlayerAttack>();
        if (PA.bypassStun || IsStunned)
        {
            HP -= PA.dmg;
        }
        if (stunCouroutine != null)
        {
            StopCoroutine(stunCouroutine);
        }
        stunCouroutine = StartCoroutine(StunTimer(PA.stunDuration));
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
    }
}
