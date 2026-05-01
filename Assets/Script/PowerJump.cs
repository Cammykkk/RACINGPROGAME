using UnityEngine;

public class PowerJump : MonoBehaviour
{
    [Header("Ajustes de Retroceso")]
    public float fuerzaRetroceso = 30f; 
    public float fuerzaLevante = 10f;    
    public bool resetearVelocidad = true; 

    private void OnCollisionEnter(Collision collision)
    {
        //  Tag
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // Calcular dirección opuesta
                Vector3 dirRetroceso = (collision.transform.position - transform.position).normalized;
                dirRetroceso.y = 0; 

                // Frenado en seco
                if (resetearVelocidad)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                // atrás y arriba
                rb.AddForce(dirRetroceso * fuerzaRetroceso, ForceMode.VelocityChange);
                rb.AddForce(Vector3.up * fuerzaLevante, ForceMode.VelocityChange);

                Debug.Log("Penalización aplicada.");
            }
        }
    }
}