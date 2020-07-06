using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager instance;

    public Text timeScore;
    public GameObject gameOverUI;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        timeScore.text = Time.timeSinceLevelLoad.ToString("00");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public static void GameOver(bool dead)
    {
        if (dead)
        {
            instance.gameOverUI.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
