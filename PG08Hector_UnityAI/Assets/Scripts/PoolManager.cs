using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager> {

    private Dictionary<string, Stack<GameObject>> nameToStack = new Dictionary<string, Stack<GameObject>>();
    private string path = "PoolObjects";

    void Awake() {
        //We load all the prefabs from the pool objects folder
        GameObject[] poolObjects = Resources.LoadAll<GameObject>(path);
        foreach (GameObject obj in poolObjects) {
            //We add a new stack for each prefab
            nameToStack.Add(obj.name, new Stack<GameObject>());
            //nameToStack[obj.name] = new Stack<GameObject>();
            //We add the prefab as the first element of our stack
            nameToStack[obj.name].Push(obj);
        }
    }

    public GameObject Spawn(string objName) {
        Stack<GameObject> objectStack = nameToStack[objName];
        //If there is only one element left in the object stack, that must be the prefab
        if (objectStack.Count == 1) {
            //We create a new object based on the prefab in our stack
            GameObject newObject = Instantiate(objectStack.Peek());
            //Removes the (Clone) behind the name of the game object
            newObject.name = objName;
            return newObject;
        }
        GameObject topObject = objectStack.Pop();
        topObject.SetActive(true);
        //Setting parent to null will make the object a child of the scene root
        topObject.transform.SetParent(null);
        return topObject;
    }

    //We despawn our objects, instead of destroying them, meaning this object can now be recycled
    public void Despawn(GameObject obj) {
        obj.SetActive(false);
        obj.transform.SetParent(this.transform);
        nameToStack[obj.name].Push(obj);
    }

}
