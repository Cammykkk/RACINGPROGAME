using UnityEngine;
using UnityEngine.Audio; // Necesario para usar AudioMixerGroup

public class CarSoundEffects : MonoBehaviour
{
    [Header("Configuración de Mezcla")]
    public AudioMixerGroup sfxGroup; // Aquí arrastraremos el canal SFX del Mixer
    [Range(0, 1)] public float volumenEfectos = 1.0f;

    [Header("Clips de Sonido")]
    public AudioClip clipMeta;
    public AudioClip clipChoqueMuro;
    public AudioClip clipCono;
    public AudioClip clipBoost;

    private AudioSource audioS;

    void Start()
    {
        audioS = gameObject.AddComponent<AudioSource>();
        
        // ENLACE AL MIXER:
        if (sfxGroup != null)
        {
            audioS.outputAudioMixerGroup = sfxGroup;
        }

        // Configuración básica para que suenen potentes
        audioS.playOnAwake = false;
        audioS.spatialBlend = 0.5f; // Mezcla entre 2D y 3D para que tengan presencia
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            audioS.PlayOneShot(clipMeta, volumenEfectos);
        }
        else if (other.CompareTag("Boost"))
        {
            audioS.PlayOneShot(clipBoost, volumenEfectos);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Muro"))
        {
            if (collision.relativeVelocity.magnitude > 2f)
            {
                audioS.PlayOneShot(clipChoqueMuro, volumenEfectos);
            }
        }
        else if (collision.gameObject.CompareTag("Cono"))
        {
            audioS.PlayOneShot(clipCono, volumenEfectos);
        }
    }
}