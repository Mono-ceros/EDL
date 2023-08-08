using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int maxHealth = 200;
    public int currentHealth;

    public int maxStamina = 20;
    public int currentStamina;

    public HealthBar healthBar;
    public StaminaBar staminaBar;
    AnimatorHandler animatorHandler;

    private void Awake()
    {
        healthBar = FindAnyObjectByType<HealthBar>();
        staminaBar = FindAnyObjectByType<StaminaBar>();
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetCurrentHealth(currentHealth);

        currentStamina = maxStamina;
        staminaBar.SetMaxStamina(maxStamina);
        staminaBar.SetCurrentStamina(currentStamina);
    }

    public void TakeDamage(int damage)
    {
        currentHealth = currentHealth - damage;
        healthBar.SetCurrentHealth(currentHealth);

        animatorHandler.PlayTargetAnimation("Damage_01", true);

        if(currentHealth <= 0)
        {
            currentHealth = 0;
            animatorHandler.PlayTargetAnimation("Die_01", true);
        }
    }

    public void TakeStaminaDamage(int damage)
    {
        currentStamina = currentStamina - damage;
        staminaBar.SetCurrentStamina(currentStamina);
    }
}
