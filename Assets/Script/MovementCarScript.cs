using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MovementCarScript : MonoBehaviour
{
    public InputActionAsset inputActions;
    private InputAction _moveAction, _jumpAction, _resetAction;
    private Rigidbody _rb;

    [Header("Inicio")]
    public bool carreraEmpezada = false;
    public GameObject mensajeInicioUI; 

    [Header("Físicas de manejo")]
    public float aceleracionMax = 100f;
    public float fuerzaGiroMax = 150f;
    public float suavizadoGiro = 15f; 
    [Range(0, 1)] public float agarreLateral = 0.98f; 
    public float downforce = 120f; 

    [Header("Salto e Impulso Meta")]
    public float fuerzaSalto = 22f;
    public float impulsoMeta = 70f; 
    public bool saltoInfinitoDesbloqueado = false; 
    private bool _estaEnElSuelo;

    [Header("Visuales")]
    public Transform[] llantasVisuales;
    public float radioLlanta = 0.4f;

    private float _inputAceleracionLerp;
    private float _inputGiroLerp;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        
        // Configuración de estabilidad y peso
        _rb.mass = 2800f; 
        _rb.linearDamping = 1.5f; 
        _rb.angularDamping = 8f; 
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.centerOfMass = new Vector3(0, -1.5f, 0);

        // Mapeo de controles
        var playerMap = inputActions.FindActionMap("Player");
        _moveAction = playerMap.FindAction("Move");
        _jumpAction = playerMap.FindAction("Jump");
        _resetAction = playerMap.FindAction("Reset");

        if (mensajeInicioUI != null) mensajeInicioUI.SetActive(true);
    }

    private void OnEnable() => inputActions.FindActionMap("Player").Enable();

    void Update()
    {
        //  inicio de carrera
        if (!carreraEmpezada)
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame || 
                (Gamepad.current != null && Gamepad.current.allControls[0].device.wasUpdatedThisFrame))
            {
                carreraEmpezada = true;
                if (mensajeInicioUI != null) mensajeInicioUI.SetActive(false);
            }
            return;
        }

        // reiniciar nivel
        if (_resetAction != null && _resetAction.triggered)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // salto
        if (_jumpAction != null && _jumpAction.triggered)
        {
            if (_estaEnElSuelo || saltoInfinitoDesbloqueado)
            {
                _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
                _rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
            }
        }

        RodarLlantas();
    }

    void FixedUpdate()
    {
        if (!carreraEmpezada || _moveAction == null) return;

        Vector2 inputRaw = _moveAction.ReadValue<Vector2>();
        _inputAceleracionLerp = Mathf.Lerp(_inputAceleracionLerp, inputRaw.y, Time.fixedDeltaTime * 6f);
        _inputGiroLerp = Mathf.Lerp(_inputGiroLerp, inputRaw.x, Time.fixedDeltaTime * suavizadoGiro);

        // Aceleración 
        _rb.AddRelativeForce(Vector3.forward * _inputAceleracionLerp * aceleracionMax * 100f, ForceMode.Acceleration);
        Vector3 velLateral = transform.right * Vector3.Dot(_rb.linearVelocity, transform.right);
        _rb.AddForce(-velLateral * _rb.mass * agarreLateral, ForceMode.Force);

        // downforce
        if (!saltoInfinitoDesbloqueado)
            _rb.AddForce(-transform.up * downforce * _rb.linearVelocity.magnitude);

        // drift
        float velocidadZ = transform.InverseTransformDirection(_rb.linearVelocity).z;
        if (Mathf.Abs(_inputGiroLerp) > 0.01f && Mathf.Abs(velocidadZ) > 0.2f)
        {
            float factorDir = velocidadZ > 0 ? 1 : -1;
            float rot = _inputGiroLerp * fuerzaGiroMax * factorDir * Time.fixedDeltaTime;
            _rb.MoveRotation(_rb.rotation * Quaternion.Euler(0, rot, 0));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Impulso al cruzar la meta
        if (other.CompareTag("Finish") && !saltoInfinitoDesbloqueado)
        {
            saltoInfinitoDesbloqueado = true;
            _rb.linearDamping = 0.2f; 
            _rb.AddRelativeForce(Vector3.forward * impulsoMeta, ForceMode.VelocityChange);
            _rb.AddForce(Vector3.up * impulsoMeta, ForceMode.VelocityChange);
        }
    }

    void RodarLlantas()
    {
        float vZ = transform.InverseTransformDirection(_rb.linearVelocity).z;
        float rot = (vZ / (2 * Mathf.PI * radioLlanta)) * 360f * Time.deltaTime;
        foreach (Transform llanta in llantasVisuales) 
            if (llanta != null) llanta.Rotate(Vector3.right * rot, Space.Self);
    }

    private void OnCollisionStay(Collision col) => _estaEnElSuelo = true;
    private void OnCollisionExit(Collision col) => _estaEnElSuelo = false;
}