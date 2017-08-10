using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Linq;

public abstract class Unit : BaseObject {

    public float minWaypointDistance = 1.0f;
    public float rotationSpeed = 5.0f;
    public float movementSpeed = 5.0f;
    public int attackPower = 20;
    public float attackInterval = 1.0f;
    public float attackRange = 0.0f;

    protected Building currentTarget;
    private IEnumerator currentState;

    private Path currentPath;
    private int waypointIndex;

    private Seeker seekerObj;

    protected Animator anim;
    private Vector3 lastPosition;

    private int maxHealth;

    private float attackBuffTimer = 0.0f;
    private bool hasAttackBuff = false;

    private float speedDebuffTimer = 0.0f;
    private bool hasSpeedDebuff = false;

    // Use this for initialization
    void Start () {
        maxHealth = health;
        waypointIndex = 0;
        currentTarget = null;
        currentPath = null;
        seekerObj = GetComponent<Seeker>();
        anim = GetComponentInChildren<Animator>();
        SetState(OnIdle());
	}

    void Update() {
        //The distance between current and last position gives us the speed of the last frame
        float movement = Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;
        anim.SetFloat("Speed", movement);
    }

    public void OnHeal(int healedAmmount) {
        health += healedAmmount;
        if (health > maxHealth){
            health = maxHealth;
        }
        //print(health);
    }

    public void OnBuffAttack(int attackBuffAmmount, float attackBuffDuration) {
        if (hasAttackBuff)
            return;
        StopCoroutine(BuffAttackUnit(attackBuffAmmount, attackBuffDuration));
        StartCoroutine(BuffAttackUnit(attackBuffAmmount, attackBuffDuration));
    }

    IEnumerator BuffAttackUnit(int attackBuffAmmount, float attackBuffDuration) {
        hasAttackBuff = true;
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.GetComponent<Renderer>().material.color = Color.red;
        sphere.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        sphere.transform.position = new Vector3(transform.position.x, transform.position.y + GetComponent<Collider>().bounds.size.y, transform.position.z);
        sphere.transform.SetParent(transform);
        Destroy(sphere, attackBuffDuration);
        attackPower += attackBuffAmmount;
        while (attackBuffTimer < attackBuffDuration) {
            attackBuffTimer += Time.deltaTime;
            yield return null;
        }
        attackBuffTimer = 0.0f;
        attackPower -= attackBuffAmmount;
        hasAttackBuff = false;
    }

    public void OnDebuffSpeed(float speedDebuffAmmount, float speedDebuffDuration) {
        if (hasSpeedDebuff)
            return;
        StopCoroutine(DebuffSpeedUnit(speedDebuffAmmount, speedDebuffDuration));
        StartCoroutine(DebuffSpeedUnit(speedDebuffAmmount, speedDebuffDuration));
    }

    IEnumerator DebuffSpeedUnit(float speedDebuffAmmount, float speedDebuffDuration) {
        hasSpeedDebuff = true;
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.GetComponent<Renderer>().material.color = Color.blue;
        cube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        cube.transform.position = new Vector3(transform.position.x, transform.position.y + GetComponent<Collider>().bounds.size.y, transform.position.z);
        cube.transform.SetParent(transform);
        Destroy(cube, speedDebuffDuration);
        movementSpeed -= speedDebuffAmmount;
        while (speedDebuffTimer < speedDebuffDuration) {
            speedDebuffTimer += Time.deltaTime;
            yield return null;
        }
        speedDebuffTimer = 0.0f;
        movementSpeed += speedDebuffAmmount;
        hasSpeedDebuff = false;
    }

    void SetState(IEnumerator newState) {
        //We ensure the previous coroutine is no longer running
        if (currentState != null)
            StopCoroutine(currentState);
        //We store the currently running coroutine as a variable
        currentState = newState;
        //We start the coroutine
        StartCoroutine(currentState);
    }

    IEnumerator OnIdle() {
        while (currentTarget == null) {
            //You can yield return another couroutine
            //This means that this couroutine wont continue until the FindClosestBuilding couroutine has completed
            //Its a coroutine inside a coroutine
            yield return StartCoroutine(FindClosestBuilding());
            //FindClosestBuilding();
            yield return null;
        }
        //The target may have been set, but sometimes it takes a while for the path to calculate
        while (currentPath == null) {
            yield return null;
        }
        SetState(OnMoving());
    }

    IEnumerator OnMoving() {
        waypointIndex = 0;
        //As long as the target is not null (it could be destroyed), keep moving towards it
        while (currentTarget != null) {
            if (waypointIndex < currentPath.vectorPath.Count) {
                Vector3 targetPos = currentPath.vectorPath[waypointIndex];
                //We have to normalize the distance vector (giving it a length of 1), because otherwise the unit will move faster on long distances
                Vector3 direction = (targetPos - transform.position).normalized;
                //We multiply our direction by Time.deltaTime to ensure it runs framerate independant
                transform.Translate(direction * Time.deltaTime * movementSpeed, Space.World);
                //We cant calculate a lookRotation on Vector3.zero. Just ensuring that the warning message doesnt popup
                if (direction != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
                }
                if (Vector3.Distance(transform.position, targetPos) < minWaypointDistance)
                    waypointIndex++;

                //If I am closer to the building than the sum of my attack range and the radius of the building I attack
                if (Vector3.Distance(transform.position, currentTarget.transform.position) < attackRange + currentTarget.radius)
                    SetState(OnAttacking());

            }
            //else {
            //    SetState(OnAttacking());
            //}
            yield return null;
        }
        SetState(OnIdle());
    }

    IEnumerator OnAttacking() {
        float timer = 0.0f;
        while (currentTarget.health > 0) {
            timer += Time.deltaTime;
            if (timer >= attackInterval) {
                Attack();
                timer = 0.0f;
            }
            yield return null;
        }
        currentTarget = null;
        SetState(OnIdle());
    }

    protected abstract void Attack();

    IEnumerator FindClosestBuilding() {
        //TODO make sure it finds the closest building
        Building[] allBuildings = FindObjectsOfType<Building>().Where(b => b.GetType() != typeof(Wall)).ToArray();

        if (allBuildings.Length == 0)
            yield break;

        //New way - Linq
        //Using Linq here is slightly inefficient, because it orders the ENTIRE array, we on;y need the closest building        
        Building closestBuilding = allBuildings.OrderBy(b => Vector3.Distance(transform.position, b.transform.position)).First();

        currentTarget = closestBuilding;

        //We create a path from the unit to its current target
        //We set the old path to null before we calculate a new one
        currentPath = null;
        //We calculate a new path towards the building
        Path newPath = seekerObj.StartPath(transform.position, currentTarget.transform.position);
        //This coroutine will wait for the path to be calculated
        yield return StartCoroutine(newPath.WaitForPath());
        //Now the path has been calculated

        foreach (GraphNode pathnode in newPath.path) {
            //We check if there is any penalty in our path. A node will penalty, must mean that there is a building placed on top of that node
            if (pathnode.Penalty > 0) {
                Building newBuilding = GameManager.instance.nodeToBuilding[pathnode];
                //The building can be null, when it was destroyed
                //We ignore buildings that are not walls
                if (newBuilding == null || newBuilding.GetType() != typeof(Wall))
                    continue;

                //If there is a wall in our path, that will be our new target
                currentTarget = newBuilding;
                newPath = seekerObj.StartPath(transform.position, currentTarget.transform.position);
                yield return StartCoroutine(newPath.WaitForPath());
                //Breaking out of the foreach loop
                break;
            }
        }
        OnPathComplete(newPath);
    }

    void FindClosestBuildingOld() {
        //TODO make sure it finds the closest building
        Building[] allBuildings = FindObjectsOfType<Building>().Where(b => b.GetType() != typeof(Wall)).ToArray();

        if (allBuildings.Length == 0)
            return;

        //New way - Linq
        //Using Linq here is slightly inefficient, because it orders the ENTIRE array, we on;y need the closest building        
        Building closestBuilding = allBuildings.OrderBy(b => Vector3.Distance(transform.position, b.transform.position)).First();

        //Old Way
        //float closestDistance = Mathf.Infinity;
        //Building closestBuilding = null;
        //foreach (Building b in allBuildings) {
        //    float distanceToBuilding = Vector3.Distance(transform.position, b.transform.position);
        //    if (distanceToBuilding < closestDistance) {
        //        closestDistance = distanceToBuilding;
        //        closestBuilding = b;
        //    }
        //}


        currentTarget = closestBuilding;

        //We create a path from the unit to its current target
        //We set the old path to null before we calculate a new one
        currentPath = null;
        seekerObj.StartPath(transform.position, currentTarget.transform.position, OnPathComplete);
    }

    void OnPathComplete(Path newPath) {
        currentPath = newPath;
    }

}
