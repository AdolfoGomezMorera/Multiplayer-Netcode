using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PrimerScript : MonoBehaviour
{
    [SerializeField] private float velocidad;
    private Camera camara;
    private float distanciaOpacidad = 5f;

    private TMP_Text textoEstrellas;
    private TMP_Text textoVictoria;
    private TMP_Text textoTiempo;

    private AudioSource audioSource;
    [SerializeField] private AudioClip Colector1;
    [SerializeField] private AudioClip Colector2;
    [SerializeField] private AudioClip musicaAmbiental;

    private Vector3 offset;
    private Rigidbody rb;
    private int estrellas = 0;
    private int totalEstrellas;
    private float tiempoPartida = 0f;
    private bool juegoTerminado = false;

    public void AsignarReferencias(Camera cam, TMP_Text estrellasText, TMP_Text victoriaText, TMP_Text tiempoText)
    {
        camara = cam;
        textoEstrellas = estrellasText;
        textoVictoria = victoriaText;
        textoTiempo = tiempoText;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        if (camara != null)
            offset = camara.transform.position - transform.position;

        totalEstrellas = GameObject.FindGameObjectsWithTag("Estrella").Length;

        if (musicaAmbiental != null)
        {
            audioSource.clip = musicaAmbiental;
            audioSource.loop = true;
            audioSource.Play();
        }

        if (textoVictoria != null)
            textoVictoria.text = "Recoge todas las monedas";
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Estrella"))
        {
            ParticleSystem particulas = other.GetComponent<ParticleSystem>();
            if (particulas != null)
                particulas.Play();

            other.gameObject.SetActive(false);
            estrellas++;

            if (textoEstrellas != null)
                textoEstrellas.text = "   " + estrellas + "/13";

            if (estrellas == totalEstrellas)
            {
                if (textoVictoria != null)
                    textoVictoria.text = "!Has Ganado!";

                audioSource.Stop();
                audioSource.PlayOneShot(Colector2);
                TerminarJuego();
            }
            else
            {
                audioSource.PlayOneShot(Colector1);
            }
        }
    }

    void Update()
    {
        if (!juegoTerminado)
        {
            tiempoPartida += Time.deltaTime;
            int horas = Mathf.FloorToInt(tiempoPartida / 3600);
            int minutos = Mathf.FloorToInt((tiempoPartida % 3600) / 60);
            int segundos = Mathf.FloorToInt(tiempoPartida % 60);

            if (textoTiempo != null)
                textoTiempo.text = string.Format("{0:00}:{1:00}:{2:00}", horas, minutos, segundos);
        }

        Vector3 input = Vector3.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) input += Vector3.forward;
            if (Keyboard.current.sKey.isPressed) input += Vector3.back;
            if (Keyboard.current.aKey.isPressed) input += Vector3.left;
            if (Keyboard.current.dKey.isPressed) input += Vector3.right;
        }

        if (input != Vector3.zero)
        {
            if (textoVictoria != null)
                textoVictoria.text = "";

            rb.AddForce(input.normalized * velocidad);
        }

        if (camara != null)
        {
            camara.transform.position = transform.position + offset;

            GameObject[] objetos = GameObject.FindGameObjectsWithTag("Objetos Opacos");
            foreach (GameObject objeto in objetos)
            {
                float distancia = Vector3.Distance(camara.transform.position, objeto.transform.position);
                Renderer renderer = objeto.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Color color = renderer.material.color;
                    color.a = distancia < distanciaOpacidad ? 0.5f : 1f;
                    renderer.material.color = color;
                }
            }
        }
    }

    void TerminarJuego()
    {
        juegoTerminado = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
