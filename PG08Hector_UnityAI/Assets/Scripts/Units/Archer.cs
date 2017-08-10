using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Unit {

    //public Arrow arrowPrefab;
    public Vector3 arrowOffset = new Vector3(0.0f, 0.5f, 0.0f);

    protected override void Attack() {
        //print("archer: " + attackPower);
        anim.SetTrigger("Attack");
        //Arrow arrowClone = Instantiate(arrowPrefab);
        Arrow arrowClone = PoolManager.instance.Spawn("Arrow").GetComponent<Arrow>();
        arrowClone.transform.position = transform.position + arrowOffset;
        //We provide our target and attack power to the arrow
        //Now its independent from the archer
        arrowClone.Init(currentTarget, attackPower);
        //currentTarget.OnHit(attackPower);
        //print("ATTACK");
        //throw new NotImplementedException();
    }

}
