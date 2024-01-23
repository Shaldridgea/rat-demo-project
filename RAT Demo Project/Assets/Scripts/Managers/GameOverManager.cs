using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public PlayerHealth playerHealth;


    Animator anim;


    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        GameEvents.EventBus.PlayerDied += PlayGameOver;
    }


    private void PlayGameOver()
    {
        if (playerHealth.CurrentHealth <= 0)
        {
            anim.SetTrigger("GameOver");
        }
    }

    private void OnDestroy()
    {
        GameEvents.EventBus.PlayerDied -= PlayGameOver;
    }
}
