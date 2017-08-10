using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTower : Building {

    private Unit target;
    private float attackTimer = 0.0f;

    public Vector3 bombOffset = new Vector3(0.0f, 4.0f, 0.0f);
    public float attackInterval = 2.0f;
    public float attackRange = 15.0f;
    public int attackDamage = 5;
    //public Arrow arrowPrefab;

    // Use this for initialization
    //void Start () { }

    // Update is called once per frame
    void Update() {
        if (target == null)
            LookForTarget();
        attackTimer += Time.deltaTime;
        if (target != null && attackTimer >= attackInterval) {
            //Arrow arrowClone = Instantiate(arrowPrefab);
            Bomb bombClone = PoolManager.instance.Spawn("Bomb").GetComponent<Bomb>();
            bombClone.transform.position = transform.position + bombOffset;
            bombClone.Init(target, attackDamage);
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
