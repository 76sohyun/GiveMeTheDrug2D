using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    [Header("BGM")]
    public AudioClip tutorialBGM;
    public AudioClip MainBGM;
    
    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "ExplanationScene")
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                SceneManager.LoadScene("IntroScene");
            }
        }
        if (SceneManager.GetActiveScene().name == "IntroScene")
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                SceneManager.LoadScene("TutorialScene");
            }
        }
        
        if (SceneManager.GetActiveScene().name == "lastScene")
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                SceneManager.LoadScene("StartScene");
            }
        }
        
    }

    public void OnCutsceneEnd()
    {
        Debug.Log("컷신 종료 → 씬 전환 시도");
        SceneManager.LoadScene("TutorialScene");
        
    }

    public void IntroLoad()
    {
        SceneManager.LoadScene("IntroScene");
    }

    public void OnExplainEnd()
    {
        SceneManager.LoadScene("ExplanationScene");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (AudioManager.instance == null)
        {
            Debug.LogWarning("AudioManager instance is null!");
            return;
        }
        if (scene.name == "TutorialScene")
        {
            AudioManager.instance.PlayBGM(tutorialBGM);
        }
        else if (scene.name == "StartScene")
        {
            AudioManager.instance.PlayBGM(MainBGM);
        }
        else
        {
            AudioManager.instance.StopBGM();
        }
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void Ending()
    {
        SceneManager.LoadScene("EndingScene");
    }
    
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
