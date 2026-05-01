using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    [Header("Ajustes del Boost")]
    public float fuerzaBoost = 80f; 
    public Color colorDebug = Color.yellow;

    private void OnTriggerEnter(Collider other)
    {
        // Tag
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            
            if (rb != null)
            {
                // Impulso de frente del coche
                Vector3 direccionCoche = other.transform.forward;

                // cambio de velocidad 
                rb.AddForce(direccionCoche * fuerzaBoost, ForceMode.VelocityChange);

                Debug.Log("Boost aplicado.");
            }
        }
    }
}