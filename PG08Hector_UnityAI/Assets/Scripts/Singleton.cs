using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//We have to add the constraint "T : MonoBehaviour", because c# doesnt know if T would be inheriting from MonoBehaviour (and it should)
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

	public static T instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<T>();
            }
            return _instance;
        }
    }

    private static T _instance;

}
