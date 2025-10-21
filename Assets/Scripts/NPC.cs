using UnityEngine;

public class NPC : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public bool playerInRange;

    public Dialog dialog;

    public int dialogNum;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && playerInRange)
        {
            dialog.StartDialog(dialogNum);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Player in attack range!");
            playerInRange = true;
        }
    }

    //Sets the Enemy's playerInRange variable to false if player leavs the range hitbox.
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
