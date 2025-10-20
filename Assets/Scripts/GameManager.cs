using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] public static bool IsPaused;

    [SerializeField] public static bool DialogOn;

    public GameObject pauseUI;
    void Start()
    {
        pauseUI.SetActive(false);
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
}
