using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinRobber : Monster
{
    public override void Attack()
    {
        onAttack += () => 
        base.Attack();
    }


}
