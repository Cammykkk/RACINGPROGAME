using System.Collections;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Loader : MonoBehaviour
{

    public Image loader;
    private float progress =0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        loader.fillAmount = progress;
        StartCoroutine(LoadScene());
        SceneManager.LoadScene("Game");
    }
    public void StartProgress()
    {
        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        while (progress < 1)
        {
            progress += 0.1f;
        loader.fillAmount = progress;
        yield return new WaitForSeconds(0.2f);
        }

        if(progress >=1)
        {
           SceneManager.LoadSceneAsync("Game");
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
