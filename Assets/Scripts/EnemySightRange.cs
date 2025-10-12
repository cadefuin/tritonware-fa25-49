using UnityEngine;

public class EnemySightRange : MonoBehaviour
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

    //The enemy sight range is a trigger, so we use OnTriggerEnter instead of OnCollisionEnter.
    //Sets the Enemy's playerInSight variable to true if player is in the sight hitbox.
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Collision detected!");
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Player in sight range!");
            parentScript.playerInSight = true;
        }
    }

    //Sets the Enemy's playerInSight variable to false if player leavs the sight hitbox.
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parentScript.playerInSight = false;
        }
    }
}
