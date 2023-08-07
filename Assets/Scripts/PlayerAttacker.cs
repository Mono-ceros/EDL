using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{
    AnimatorHandler animatorHandler;
    InputHandler inputHandler;
    public string lastAttack;

    private void Awake()
    {
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
        inputHandler = GetComponent<InputHandler>();
    }

    public void HandleWeaponCombo(WeaponItem weapon)
    {
        if(inputHandler.comboFlag)
        {

            if (lastAttack == weapon.oh_Light_Attack_1)
            {
                animatorHandler.PlayTargetAnimation(weapon.oh_Light_Attack_2, true);
            }

            animatorHandler.anim.SetBool("canDoCombo", false);

            if (lastAttack == weapon.oh_Light_Attack_2)
            {
                animatorHandler.PlayTargetAnimation(weapon.oh_Light_Attack_3, true);
            }
        }
    }

    public void HandleLightAttack(WeaponItem weapon)
    {
        animatorHandler.PlayTargetAnimation(weapon.oh_Light_Attack_1, true);
        lastAttack = weapon.oh_Light_Attack_1;
    }

    public void HandleHeavyAttack(WeaponItem weapon)
    {
        animatorHandler.PlayTargetAnimation(weapon.oh_Heavy_Attack_1, true);
        lastAttack = weapon.oh_Heavy_Attack_1;
    }
}
