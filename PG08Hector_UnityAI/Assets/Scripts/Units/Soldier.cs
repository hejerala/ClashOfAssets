using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Unit {

    protected override void Attack() {
        //print("soldier: "+attackPower);
        anim.SetTrigger("Attack");
        currentTarget.OnHit(attackPower);
        //print("ATTACK");
        //throw new NotImplementedException();
    }

}
