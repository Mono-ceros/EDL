using UnityEngine;

/// <summary>
/// 데미지 입을애들 한번에 호출해서 쓰려고 만든 인터페이스
/// </summary>
public interface IDamage
{
    //데미지, 맞은곳, 맞은방향
    void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal);
}
