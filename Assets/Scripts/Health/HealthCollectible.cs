using System.Collections;
using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    [SerializeField] private float healthValue;
    private AudioSource audioSource; // Variable para el componente AudioSource

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>(); // Obtener el componente AudioSource
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<Health>().AddHealth(healthValue);
            audioSource.Play(); // Reproducir el sonido

            // Desactivar el objeto después de un breve retardo para permitir que el sonido se reproduzca
            StartCoroutine(DeactivateAfterSound());
        }
    }

    private IEnumerator DeactivateAfterSound()
    {
        // Esperar a que termine el clip de audio
        yield return new WaitForSeconds(audioSource.clip.length); 
        gameObject.SetActive(false);
    }
}
