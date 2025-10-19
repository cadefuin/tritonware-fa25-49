using UnityEngine;

public class BackgroundMove : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject bg;
    public GameObject mgfar;
    public GameObject mgclose;

    public GameObject player;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(0,0.75f,0)+player.transform.position;
        bg.transform.localPosition = new Vector2(player.transform.position.x * -0.005f, player.transform.position.y * -0.002f);
        mgfar.transform.localPosition = new Vector2(player.transform.position.x * -0.05f, player.transform.position.y * -0.01f);
        mgclose.transform.localPosition = new Vector2(player.transform.position.x*-0.1f,player.transform.position.y*-0.02f);
    }
}
