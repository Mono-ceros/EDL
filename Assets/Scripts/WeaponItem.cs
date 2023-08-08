using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 무기 스크립터블 오브젝트
/// </summary>
[CreateAssetMenu(menuName = "Items/Weapon Item")]
public class WeaponItem : Item
{
    public GameObject modelPrefab;
    public bool isUnarmed;

    public string right_hand_idle;
    public string left_hand_idle;

    [Header("한손 공격 애니메이션")]
    public string oh_Light_Attack_1;
    public string oh_Light_Attack_2;
    public string oh_Light_Attack_3;
    public string oh_Heavy_Attack_1;

    [Header("스테미나 소모")]
    public int baseStamina;
    public float lightAttackMultiplier;
    public float heavyAttackMultiplier;
}
