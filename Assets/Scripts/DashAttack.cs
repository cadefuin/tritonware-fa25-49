using UnityEngine;

public class DashAttack : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public bool playerSide = true;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerSide)
        {
            if (collision.CompareTag("Enemy"))
            {
                Enemy enemyScript = collision.gameObject.GetComponent<Enemy>();

                enemyScript.HitByAttack(gameObject);
            }
        } else
        {
            if (collision.CompareTag("Player"))
            {
                PlayerController pScript = collision.gameObject.GetComponent<PlayerController>();

                pScript.HitByAttack(gameObject);
            }
        }



        
    }
}
