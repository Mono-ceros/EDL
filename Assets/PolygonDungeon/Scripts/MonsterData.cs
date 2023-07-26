using UnityEngine;

// 몬스터 생성시 데이터로 사용할 스크립터블 오브젝트
[CreateAssetMenu(menuName = "Scriptable/MonsterData", fileName = "Monster Data", order = 2)]
public class MonsterData : ScriptableObject
{
    public float health = 100f; // 체력
    public float damage = 20f; // 공격력
    public float speed = 2f; // 이동 속도
    public float timeBetAttack = 1f; // 공격 속도
    public float attackDist = 5f; //사거리
}
