using UnityEngine;

public class MotorSoundController : MonoBehaviour
{
    [Header("Referencias")]
    public AudioSource motorAudioSource; // El AudioSource con el clip del motor
    public Rigidbody carRigidbody;       // El Rigidbody de tu coche

    [Header("Configuración de Tono (Pitch)")]
    [Tooltip("Tono cuando el coche está totalmente quieto")]
    public float minPitch = 0.8f; 
    [Tooltip("Tono máximo al que llegará el motor")]
    public float maxPitch = 2.5f;
    [Tooltip("Qué tan rápido sube el tono con la velocidad")]
    public float pitchMultiplier = 0.04f;

    [Header("Configuración de Volumen")]
    [Range(0, 1)] public float minVolume = 0.3f; // Volumen en reposo
    [Range(0, 1)] public float maxVolume = 0.8f; // Volumen a máxima velocidad

    [Header("Suavizado")]
    public float smoothSpeed = 5f; // Qué tan rápido reacciona el sonido a los cambios

    void Start()
    {
        // Validación de componentes para evitar errores en consola
        if (motorAudioSource == null) motorAudioSource = GetComponent<AudioSource>();
        if (carRigidbody == null) carRigidbody = GetComponent<Rigidbody>();

        // Configuración inicial del audio
        if (motorAudioSource != null)
        {
            motorAudioSource.loop = true;
            motorAudioSource.playOnAwake = true;
            if (!motorAudioSource.isPlaying) motorAudioSource.Play();
        }
    }

    void Update()
    {
        if (carRigidbody == null || motorAudioSource == null) return;

        // 1. Calculamos la velocidad actual
        // magnitude nos da un número flotante (ej: 0 si está quieto, 50 si va rápido)
        float currentSpeed = carRigidbody.linearVelocity.magnitude;

        // 2. Lógica del Tono (Pitch)
        // Calculamos a qué tono "debería" llegar según la velocidad
        float targetPitch = minPitch + (currentSpeed * pitchMultiplier);
        // Lo limitamos para que no pase del máximo
        targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);
        // Aplicamos el tono suavemente
        motorAudioSource.pitch = Mathf.Lerp(motorAudioSource.pitch, targetPitch, Time.deltaTime * smoothSpeed);

        // 3. Lógica del Volumen
        // Calculamos el volumen proporcional a la velocidad (ej: dividimos entre 40 para el rango)
        float targetVolume = Mathf.Lerp(minVolume, maxVolume, currentSpeed / 40f);
        motorAudioSource.volume = Mathf.Lerp(motorAudioSource.volume, targetVolume, Time.deltaTime * smoothSpeed);
    }
}