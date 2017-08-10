using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Pathfinding;

public class GameManager : Singleton<GameManager> {

    //The world that we are currently attacking
    public static string currentWorld;

    //Every node will store a reference to the building thats built upon it
    public Dictionary<GraphNode, Building> nodeToBuilding = new Dictionary<GraphNode, Building>();
    //public string currentObject = "Soldier";
    public string currentObject {
        get {
            return _currentObject;
        }
        set {
            _currentObject = value;
            if (GameMode.isBuilding)
                ghostBuilding = (Building)SpawnObject(Vector3.zero);
        }
    }
    public int attackBuffAmmount = 5;
    public float attackBuffDuration = 3.0f;
    public float attackBuffRange= 10.0f;
    public GameObject attackBuffParticleEffectPrefab;

    private string _currentObject;
    private Building ghostBuilding;

    //void Start() {
    //    LoadWorld();
    //}

    void Awake() {
        LoadWorld();
    }

    // Update is called once per frame
    void Update () {
        //We ignore clicks if we are hovering over the UI
        if (EventSystem.current.IsPointerOverGameObject()) {
            if (GameMode.isBuilding && ghostBuilding != null)
                ghostBuilding.gameObject.SetActive(false);
            return;
        }
        if (GameMode.isBuilding && ghostBuilding != null)
            ghostBuilding.gameObject.SetActive(true);

        //if (Input.GetMouseButtonDown(0)) {
        //    //Creates a ray from our mouse position into the 3D world
        //    Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hitInfo;
        //    if (Physics.Raycast(inputRay, out hitInfo))
        //    {
        //        //Spawn object where we hit something
        //        SpawnObject(hitInfo.point);
        //    }
        //}

        //Creates a ray from our mouse position into the 3D world
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(inputRay, out hitInfo))
        {
            if (GameMode.isBuilding && ghostBuilding != null)
                ghostBuilding.transform.position = hitInfo.point;
            if (Input.GetMouseButtonDown(0)) {
                //Spawn object where we hit something
                SpawnObject(hitInfo.point);
            }
            if (!GameMode.isBuilding)
            {
                if (Input.GetMouseButtonDown(2))
                {
                    //print("Buff");
                    //Create an attack buff (a particle effect and an overlapsphere) where we hit something
                    CreateAttackBuff(hitInfo.point);
                }
            }
        }


        //if (!GameMode.isBuilding) {
        //    if (Input.GetMouseButtonDown(2)) {
        //        //print("Buff");
        //        //Creates a ray from our mouse position into the 3D world
        //        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        //        RaycastHit hitInfo;
        //        if (Physics.Raycast(inputRay, out hitInfo)) {
        //            //Create an attack buff (a particle effect and an overlapsphere) where we hit something
        //            CreateAttackBuff(hitInfo.point);
        //        }
        //    }
        //}

    }

    BaseObject SpawnObject(Vector3 position) {
        BaseObject objectPrefab = Resources.Load<BaseObject>((GameMode.isBuilding ? "Buildings/" : "Units/") + currentObject);
        BaseObject objectClone = Instantiate(objectPrefab);
        //Im making sure the objects dont have "clone" in their name
        objectClone.name = currentObject;
        objectClone.transform.position = position;
        return objectClone;
    }

    void CreateAttackBuff(Vector3 position) {
        GameObject attackBuffParticleEffectClone = Instantiate(attackBuffParticleEffectPrefab, position, Quaternion.identity);
        ParticleSystem ps = attackBuffParticleEffectClone.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule ma = ps.main;
        ma.startLifetime = attackBuffDuration;
        Collider[] unitsToBuff = Physics.OverlapSphere(position, attackBuffRange, LayerMask.GetMask("Units"));
        foreach (Collider coll in unitsToBuff) {
            Unit unitToBuff = coll.GetComponent<Unit>();
            if (unitToBuff != null)
                unitToBuff.OnBuffAttack(attackBuffAmmount, attackBuffDuration);
        }
    }

    //IEnumerator CreateAttackBuff2() {
    //    GameObject attackBuffParticleEffectClone = Instantiate(attackBuffParticleEffectPrefab, transform.position, transform.rotation);
    //    ParticleSystem ps = attackBuffParticleEffectClone.GetComponent<ParticleSystem>();
    //    ParticleSystem.MainModule ma = ps.main;
    //    ma.startLifetime = attackBuffDuration;
    //    while (attackBuffTimer < attackBuffDuration) {
    //        attackBuffIntervalTimer += Time.deltaTime;
    //        if (attackBuffIntervalTimer > attackBuffInterval) {
    //            attackBuffIntervalTimer = 0.0f;
    //            Collider[] unitsToBuff = Physics.OverlapSphere(transform.position, attackBuffRange, LayerMask.GetMask("Units"));
    //            foreach (Collider coll in unitsToBuff)
    //            {
    //                Unit unitToBuff = coll.GetComponent<Unit>();
    //                if (unitToBuff != null)
    //                    unitToHeal.OnHeal(attackPower);
    //            }
    //        }
    //        yield return null;
    //    }
    //}

    public void SaveWorld() {
        //PlayerPrefs allows us to store data that persists after the app closes
        string worldJsonString = WorldManager.instance.GetData();
        PlayerPrefs.SetString("WorldData", worldJsonString);
        //Always remember to call PlayerPrefs.Save() or your data is lost!
        PlayerPrefs.Save();

        ParseManager.instance.SaveWorld(worldJsonString);
    }

    void LoadWorld() {
        if (GameMode.isBuilding) {
            string worldJsonString = PlayerPrefs.GetString("WorldData");
            if (!string.IsNullOrEmpty(worldJsonString))
                WorldManager.instance.SetData(worldJsonString);
        } else {
            if (!string.IsNullOrEmpty(currentWorld))
                WorldManager.instance.SetData(currentWorld);
        }
        //We scan the world after the objects have been placed
        AstarPath.active.Scan();
    }

}
