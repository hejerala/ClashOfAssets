using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : MonoBehaviour {

    public int health = 100;
    public float radius = 0.0f;

    public void OnHit(int damage) {
        health -= damage;
        if (health <= 0) {
            //health = 0;
            Destroy(gameObject);
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
