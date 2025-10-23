using Unity.VisualScripting;
using UnityEngine;

public class UI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject[] objs = new GameObject[5];

    public GameObject[] abilities = new GameObject[3];

    public GameObject player;
    public PlayerController pb;
    
    void Start()
    {
        pb = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 5; i++)
        {
            if (pb.HP > i)
            {
                objs[i].SetActive(true);
            }
            else
            {
                objs[i].SetActive(false);
            }
        }
        
        for(int i = 0; i < 3; i++)
        {
            if (pb.collectedAbilities[i] == 1)
            {
                abilities[i].SetActive(true);
            }
            else
            {
                abilities[i].SetActive(false);
            }
            
            if(pb.selectedAbility == i)
            {
                abilities[i].transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            } else
            {
                abilities[i].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }
        }
    }
}
