using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using RAT;

public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public Slider healthSlider;
    public Image damageImage;
    public AudioClip deathClip;
    public float flashSpeed = 5f;
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);


    Animator anim;
    AudioSource playerAudio;
    PlayerMovement playerMovement;
    PlayerShooting playerShooting;
    bool isDead;
    bool isLowHealth;
    bool damaged;
    int currentHealth;

    public int CurrentHealth { get { return currentHealth; } }

    void Awake ()
    {
        anim = GetComponent <Animator> ();
        playerAudio = GetComponent <AudioSource> ();
        playerMovement = GetComponent <PlayerMovement> ();
        playerShooting = GetComponentInChildren <PlayerShooting> ();
        currentHealth = startingHealth;
    }


    void Update ()
    {
        if(damaged)
        {
            damageImage.color = flashColour;
        }
        else
        {
            damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        damaged = false;
    }


    public void TakeDamage (int amount)
    {
        if (amount <= 0)
            return;

        damaged = true;

        currentHealth -= amount;

        healthSlider.value = currentHealth;

        playerAudio.Play ();

        RAT_Controller.Instance.Hearing.ShowCaption("Player_Hurt", playerAudio.clip.length, transform);

        if(currentHealth <= 0 && !isDead)
        {
            Death ();
        }
        else if (currentHealth <= startingHealth * 0.4 && !isLowHealth)
        {
            isLowHealth = true;
            GameEvents.EventBus.TriggerPlayerLowHealth();
        }
    }


    void Death ()
    {
        isDead = true;

        playerShooting.DisableEffects ();

        anim.SetTrigger ("Die");

        playerAudio.clip = deathClip;
        playerAudio.Play ();

        playerMovement.enabled = false;
        playerShooting.enabled = false;
        GameEvents.EventBus.TriggerPlayerDied();
    }


    public void RestartLevel ()
    {
        SceneManager.LoadScene (0);
    }
}
