using UnityEngine;
using System.Collections;


public class Health : MonoBehaviour
{
    // Variables for configuring the health system
    [SerializeField] private GameOverManager gameOverManager;
    [Header("Health")]
    [SerializeField] private float startingHealth; // The initial health of the character
    public float currentHealth { get; private set; } // The current health of the character, publicly accessible but only set within this class
    private bool dead = false; // Flag to check if the character is dead
    private Animator anim; // Reference to the Animator component to trigger animations
    
    // Variables for configuring invulnerability frames (iFrames)
    [Header("iFrames")]
    [SerializeField] private float iFrameDuration; // Duration of the invulnerability period after taking damage
    [SerializeField] private int numberOfFlashes; // How many times the character should flash to indicate invulnerability
    private SpriteRenderer spriteRend; // Reference to the SpriteRenderer to change the character's sprite during invulnerability

    private AudioSource audioSource; // Variable para el componente AudioSource
    [SerializeField] private AudioClip hurtSound; // Clip de audio para cuando el personaje es herido
    [SerializeField] private AudioClip dieSound;
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
         audioSource = GetComponent<AudioSource>();
        currentHealth = startingHealth; // Set the current health to the starting value
        anim = GetComponent<Animator>(); // Get the Animator component attached to this GameObject
        spriteRend = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component attached to this GameObject
    }

    // Update is called once per frame
    private void Update()
    {
        // Debug feature: Press 'E' to simulate taking damage
        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage(1);
        }
    }

    // Method to deal damage to the character
    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth); // Decrease the health and clamp it to ensure it doesn't go below 0
        
        // If the character still has health left after taking damage...
        if (currentHealth > 0)
        {
            StartCoroutine(Invunerability()); // Start invulnerability frames
            anim.SetTrigger("hurt"); // Trigger the 'hurt' animation
            audioSource.PlayOneShot(hurtSound);
        }
        else
        {
            // If the character has no health left and is not already dead...
            if (!dead)
            {
                anim.SetTrigger("die"); // Trigger the 'die' animation
                GetComponent<Player>().enabled = false; // Disable the Player script to prevent further control
                dead = true; // Set the character as dead
                audioSource.PlayOneShot(dieSound);
                gameOverManager.ShowGameOverPanel();
            }
        }
    }

    // Method to restore health to the character
    public void AddHealth(float value)
    {
        currentHealth = Mathf.Clamp(currentHealth + value, 0, startingHealth); // Increase the health and clamp it to ensure it doesn't exceed the starting value
    }

    // Coroutine for handling the invulnerability period
    private IEnumerator Invunerability()
    {
        Physics2D.IgnoreLayerCollision(10, 11, true); // Ignore collisions between certain layers to simulate invulnerability
        
        // Loop to create a flashing effect on the character sprite
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f); // Make the character red and semi-transparent
            yield return new WaitForSeconds(iFrameDuration / (numberOfFlashes * 2)); // Wait for half the duration per flash
            spriteRend.color = Color.white; // Reset to the original sprite color
            yield return new WaitForSeconds(iFrameDuration / (numberOfFlashes * 2)); // Wait for the other half of the duration
        }

        Physics2D.IgnoreLayerCollision(10, 11, false); // Re-enable collisions between the layers
    }
}
