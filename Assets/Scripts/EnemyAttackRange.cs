using UnityEngine;

public class EnemyAttackRange : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] public Enemy parentScript;
    void Start()
    {
        parentScript = transform.parent.GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    //The enemy attack range is a trigger, so we use OnTriggerEnter instead of OnCollisionEnter.
    //Sets the Enemy's playerInRange variable to true if player is in the range hitbox.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Player in attack range!");
            parentScript.playerInRange = true;
        }
    }

    //Sets the Enemy's playerInRange variable to false if player leavs the range hitbox.
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parentScript.playerInRange = false;
        }
    }
}
