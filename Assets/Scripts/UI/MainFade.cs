using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainFade : MonoBehaviour
{
    [SerializeField] Image Panel;
    private float time = 0f;
    [SerializeField] float F_time;
    private string sceneToLoad;

    public void Fade(string sceneName)
    {
        sceneToLoad = sceneName;
        StartCoroutine(FadeFlow());
    }

    private IEnumerator FadeFlow()
    {
        Panel.gameObject.SetActive(true);
        time = 0f;
        Color alpha = Panel.color;
        while (alpha.a < 1f)
        {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(0, 1, time);
            Panel.color = alpha;
            yield return null;
        }
        
        SceneManager.LoadScene(sceneToLoad);
    }
}
