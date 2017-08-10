using Parse;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScreen : MonoBehaviour {

    //This refers to a button in my canvas. This button will be cloned for each level
    public Button levelButtonOriginal;
    public Text loadingLevelsText;

    void Start() {
        levelButtonOriginal.gameObject.SetActive(false);
        ParseManager.instance.FetchLevels(OnLevelsFetched);
    }

    void OnLevelsFetched(List<ParseObject> levels) {
        foreach (ParseObject level in levels) {
            Button levelButtonClone = Instantiate(levelButtonOriginal);
            levelButtonClone.gameObject.SetActive(true);
            //We set our parent to be the same as the original buttons parent
            levelButtonClone.transform.SetParent(levelButtonOriginal.transform.parent, false);
            levelButtonClone.GetComponentInChildren<Text>().text = (string)level["name"];
            //Each nutton contains the world data
            levelButtonClone.GetComponent<LevelButton>().data = (string)level["data"];
        }
        loadingLevelsText.gameObject.SetActive(false);
    }

    public void OnAttackButton(LevelButton button) {
        GameManager.currentWorld = button.data;
        GameMode.isBuilding = false;
        SceneManager.LoadScene("GameScene");
    }

    public void OnBuildButton() {
        GameMode.isBuilding = true;
        SceneManager.LoadScene("GameScene");
    }

    public void OnClearButton() {
        //PlayerPrefs.DeleteAll();
        PlayerPrefs.DeleteKey("WorldData");
    }
}
