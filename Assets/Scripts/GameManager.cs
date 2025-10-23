using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] public static bool IsPaused;

    [SerializeField] public static bool DialogOn = false;

    [SerializeField] public static Vector2 respawnPos = new Vector2();

    [SerializeField] private static int[] NPCsTalkedTo = new int[99];

    [SerializeField] public static int enemiesKilled = 0;

    [SerializeField] public static int points = 0;

    public GameObject pauseUI;
    void Start()
    {
        Application.targetFrameRate = 60;
        
        pauseUI.SetActive(false);
        NPCsTalkedTo[0] = 0;
        NPCsTalkedTo[4] = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
            {
                Unpause();
            }
            else
            {
                Pause();
            }
        }

    }

    public void Pause()
    {
        Time.timeScale = 0f;
        pauseUI.SetActive(true);
        IsPaused = true;
    }

    public void Unpause()
    {
        Time.timeScale = 1f;
        pauseUI.SetActive(false);
        IsPaused = false;
    }

    public static void setRespawnPos(Vector2 pos)
    {
        respawnPos = pos;
    }

    public static Vector2 getRespawnPos()
    {
        return respawnPos;
    }
}
