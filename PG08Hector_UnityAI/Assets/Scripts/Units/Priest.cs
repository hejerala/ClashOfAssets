using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Priest : Unit {

    public GameObject healBuffParticleEffectPrefab;

    protected override void Attack() {
        anim.SetTrigger("Attack");
        GameObject healBuffParticleEffectClone= Instantiate(healBuffParticleEffectPrefab, transform.position, transform.rotation);
        ParticleSystem ps = healBuffParticleEffectClone.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule ma = ps.main;
        ma.startLifetime = attackInterval;
        Collider[] unitsToHeal = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Units"));
        foreach (Collider coll in unitsToHeal) {
            Unit unitToHeal = coll.GetComponent<Unit>();
            if (unitToHeal != null)
                unitToHeal.OnHeal(attackPower);
        }
    }

}
