using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ��ũ���ͺ� ������Ʈ
/// </summary>
[CreateAssetMenu(menuName = "Items/Weapon Item")]
public class WeaponItem : Item
{
    public GameObject modelPrefab;
    public bool isUnarmed;

    [Header("�Ѽ� ���� �ִϸ��̼�")]
    public string OH_Light_Attack_1;
    public string OH_Heavy_Attack_1;
}
