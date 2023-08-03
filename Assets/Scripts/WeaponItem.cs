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

    [Header("한손 공격 애니메이션")]
    public string OH_Light_Attack_1;
    public string OH_Heavy_Attack_1;
}
