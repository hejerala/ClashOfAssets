using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameScreen : MonoBehaviour {

    //Rather than linking this to a prefab in the inspector we will link it to an object in our scene that we will duplicate
    public GameObject objectButtonOriginal;

	// Use this for initialization
	void Start () {
        //If GameMode.isBuilding == true, string = "Buildings", else, string ="Units"
        //This is a quick way of writing an if-else statement
        BaseObject[] baseObjects = Resources.LoadAll<BaseObject>(GameMode.isBuilding ? "Buildings" : "Units");
        foreach (BaseObject obj in baseObjects) {
            GameObject objectButtonClone = Instantiate(objectButtonOriginal);
            //We give the clone the same parent as the original (Same behaviour as duplicate in the unity editor)
            //worldPositionStays = false, to give the object a scale of 1,1,1 relative to its parent (instead of auto-adjusting)
            objectButtonClone.transform.SetParent(objectButtonOriginal.transform.parent, false);
            //We set the textfield to display the name of the unit or building
            objectButtonClone.GetComponentInChildren<Text>().text = obj.name;
        }
        //we hide the original
        objectButtonOriginal.SetActive(false);
        //We enable the first toggle
        GetComponentInChildren<Toggle>().isOn = true;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnObjectToggle(Toggle current) {
        //When the value on any toggle changes (on or off), this method is called. We need to check if the toggle was changed to on
        if (current.isOn)
            GameManager.instance.currentObject = current.GetComponentInChildren<Text>().text;
        //FindObjectOfType<GameManager>().currentObject = current.GetComponentInChildren<Text>().text;
    }

    public void OnExitButton() {
        //We only save our world if we come from building state, otherwise we would save a destroyed city
        if (GameMode.isBuilding)
            GameManager.instance.SaveWorld();
        SceneManager.LoadScene("MenuScene");
    }

}
