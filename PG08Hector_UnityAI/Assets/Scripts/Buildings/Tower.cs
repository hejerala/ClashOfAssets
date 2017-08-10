using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Building {

    private Unit target;
    private float attackTimer = 0.0f;

    public Vector3 arrowOffset = new Vector3(0.0f, 4.0f, 0.0f);
    public float attackInterval = 1.0f;
    public float attackRange = 15.0f;
    public int attackDamage = 5;
    //public Arrow arrowPrefab;

    //void Start() { attackTimer = 0.0f; }

    // Update is called once per frame
    void Update () {
        if (target == null)
            LookForTarget();
        attackTimer += Time.deltaTime;
        if (target != null && attackTimer >= attackInterval) {
            //Arrow arrowClone = Instantiate(arrowPrefab);
            Arrow arrowClone = PoolManager.instance.Spawn("Arrow").GetComponent<Arrow>();
            arrowClone.transform.position = transform.position + arrowOffset;
            arrowClone.Init(target, attackDamage);
            attackTimer = 0.0f;
        }
	}

    void LookForTarget() {
        Collider[] surroundingColliders = Physics.OverlapSphere(transform.position, attackRange);
        foreach (Collider c in surroundingColliders) {
            Unit unit = c.GetComponent<Unit>();
            if (unit != null) {
                target = unit;
                return;
            }
        }
    }
}
