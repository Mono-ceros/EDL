using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerStats : MonoBehaviour
{
    public float maxHealth = 200;
    [Range(0, 200)]
    public float currentHealth;

    public float maxStamina = 200;
    [Range(0, 200)]
    public float currentStamina;

    public float restTime;

    public HealthBar healthBar;
    public StaminaBar staminaBar;
    AnimatorHandler animatorHandler;
    PlayerManager playerManager;
    InputHandler inputHandler;

    private void Awake()
    {
        healthBar = FindAnyObjectByType<HealthBar>();
        staminaBar = FindAnyObjectByType<StaminaBar>();
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
        playerManager = GetComponent<PlayerManager>();
        inputHandler = GetComponent<InputHandler>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetCurrentHealth(currentHealth);

        currentStamina = maxStamina;
        staminaBar.SetMaxStamina(maxStamina);
        staminaBar.SetCurrentStamina(currentStamina);

        StartCoroutine(HealStamina());
    }


    IEnumerator HealStamina()
    {
        while (true)
        {
            restTime += Time.deltaTime;

            if (playerManager.isInteracting)
            {
                restTime = 0;
            }

            if (restTime >= 0.5 && currentStamina <= 200)
            {
                currentStamina = Mathf.Min(200, currentStamina + 10);
                staminaBar.SetCurrentStamina(currentStamina);
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                yield return null;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        //피보다 0이 크면 0됨
        currentHealth = Mathf.Max(0, currentHealth - damage);
        healthBar.SetCurrentHealth(currentHealth);

        animatorHandler.PlayTargetAnimation("Player_Hit", true);

        if(currentHealth == 0)
        {
            animatorHandler.PlayTargetAnimation("Player_Die", true);
        }
    }

    public void TakeStaminaDamage(int damage)
    {
        currentStamina = Mathf.Max(0, currentStamina - damage);
        staminaBar.SetCurrentStamina(currentStamina);
    }
}
