using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loader : MonoBehaviour
{
    public Image loader;
    private float progress = 0;

    [Header("Configuración de Sonido")]
    public AudioSource musicaCarga; // Arrastra aquí el clip de música de la pantalla de carga

    void Start()
    {
        loader.fillAmount = 0;
        // Iniciamos la música si no está sonando ya
        if (musicaCarga != null && !musicaCarga.isPlaying)
        {
            musicaCarga.loop = true;
            musicaCarga.Play();
        }
        
        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        // 1. Simulación de la barra de carga
        while (progress < 1)
        {
            progress += 0.1f;
            loader.fillAmount = progress;
            yield return new WaitForSeconds(0.2f);
        }

        // 2. Iniciamos el Fade Out de la música antes de cambiar de escena
        if (musicaCarga != null)
        {
            yield return StartCoroutine(FadeOutMusica(1.5f)); 
        }

        // 3. Carga de la escena real
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game");

        // Esperamos a que la escena termine de cargar
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    // Corrutina para desvanecer el sonido suavemente
    private IEnumerator FadeOutMusica(float duration)
    {
        float startVolume = musicaCarga.volume;

        while (musicaCarga.volume > 0)
        {
            musicaCarga.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        musicaCarga.Stop();
    }
}