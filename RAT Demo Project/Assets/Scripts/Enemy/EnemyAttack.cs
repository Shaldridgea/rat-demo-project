using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField]
    private float timeBetweenAttacks = 0.5f;
    [SerializeField]
    private int attackDamage = 10;

    private Animator anim;
    private GameObject player;
    private PlayerHealth playerHealth;
    private EnemyHealth enemyHealth;
    private bool playerInRange;
    private float timer;

    private void Awake ()
    {
        player = GameObject.FindGameObjectWithTag ("Player");
        playerHealth = player.GetComponent <PlayerHealth> ();
        enemyHealth = GetComponent<EnemyHealth>();
        anim = GetComponent <Animator> ();
    }


    private void OnTriggerEnter (Collider other)
    {
        if(other.gameObject == player)
        {
            playerInRange = true;
        }
    }


    private void OnTriggerExit (Collider other)
    {
        if(other.gameObject == player)
        {
            playerInRange = false;
        }
    }


    private void Update ()
    {
        timer += Time.deltaTime;

        if(timer >= timeBetweenAttacks && playerInRange && enemyHealth.CurrentHealth > 0)
        {
            Attack ();
        }

        if(playerHealth.CurrentHealth <= 0)
        {
            anim.SetTrigger ("PlayerDead");
        }
    }


    private void Attack ()
    {
        timer = 0f;

        if(playerHealth.CurrentHealth > 0)
        {
            playerHealth.TakeDamage (attackDamage);
        }
    }
}