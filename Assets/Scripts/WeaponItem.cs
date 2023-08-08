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

    public string right_hand_idle;
    public string left_hand_idle;

    [Header("�Ѽ� ���� �ִϸ��̼�")]
    public string oh_Light_Attack_1;
    public string oh_Light_Attack_2;
    public string oh_Light_Attack_3;
    public string oh_Heavy_Attack_1;

    [Header("���׹̳� �Ҹ�")]
    public int baseStamina;
    public float lightAttackMultiplier;
    public float heavyAttackMultiplier;
}
