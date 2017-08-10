using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //While the RMB is pressed
        if (Input.GetMouseButton(1)) {
            transform.Translate(-Input.GetAxis("Mouse X"), 0.0f, -Input.GetAxis("Mouse Y"));
        }
        //Based on the scroll wheel zoom in and out
        Camera.main.orthographicSize += Input.mouseScrollDelta.y;
		
	}
}
