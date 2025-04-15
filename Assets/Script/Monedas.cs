using UnityEngine;

public class Monedas : MonoBehaviour
{
    [SerializeField] private float velocidadRotacion = 100f;
    [SerializeField] private float amplitudMovimiento = 0.5f;
    [SerializeField] private float frecuenciaMovimiento = 1f;

    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.position;
    }

    void Update()
    {
        // Rotación sobre el eje Y
        transform.Rotate(Vector3.up, velocidadRotacion * Time.deltaTime);

        // Movimiento vertical (sube y baja)
        float nuevaPosicionY = posicionInicial.y + Mathf.Sin(Time.time * frecuenciaMovimiento) * amplitudMovimiento;
        transform.position = new Vector3(transform.position.x, nuevaPosicionY, transform.position.z);
    }
}