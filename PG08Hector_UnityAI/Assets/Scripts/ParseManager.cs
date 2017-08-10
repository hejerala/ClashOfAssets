using Parse;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ParseManager : Singleton<ParseManager> {

    private const string levelName = "Hector's World";
    private Action<List<ParseObject>> onFetchedLevelsSuccess;

    public void SaveWorld(string dataString) {
        StartCoroutine(SaveLevelCoroutrine(dataString));
    }

    private IEnumerator SaveLevelCoroutrine(string dataString) {
        //Before we save our level, we check if it exists. If so, delete that one
        //Usually, this would be done on the server side. Now we are calling the server 3 times to save our level. This is inefficient
        //First server call: Does the level exist already?
        //Second server call: Delete the existing level
        //Third server call: Save new level
        ParseQuery<ParseObject> query = ParseObject.GetQuery("Levels").WhereEqualTo("name", levelName);

        Task<ParseObject> getLevelTask = query.FirstAsync();
        while (!getLevelTask.IsCompleted)
            yield return null;

        if (getLevelTask.IsFaulted || getLevelTask.IsCanceled) {
            Debug.LogWarning(getLevelTask.Exception.ToString());
        } else {
            Debug.Log("Fetch Success");
            ParseObject myLevel = getLevelTask.Result;
            Task deleteTask = myLevel.DeleteAsync();
            while (!deleteTask.IsCompleted)
                yield return null;
            if (deleteTask.IsFaulted || deleteTask.IsCanceled) {
                Debug.LogWarning(deleteTask.Exception.ToString());
            } else {
                Debug.Log("Delete Success");
            }
        }

        //The level object is a class of the server side
        ParseObject levelObject = new ParseObject("Levels");
        //the data that we put in will be shown in columns (keys) and rows (values) on the server
        //Eg. This will create the name + data column, and a new row with the levelName and the dataString as value
        levelObject["name"] = levelName;
        levelObject["data"] = dataString;

        //Saving the data is done on a different thread
        Task saveTaskLevel = levelObject.SaveAsync();
        //Because the data is saved on a different thread, we cant simply set a callback, instead, we check every frame if the task has been completed
        while (!saveTaskLevel.IsCompleted)
            yield return null;

        if (saveTaskLevel.IsFaulted || saveTaskLevel.IsCanceled) {
            Debug.LogWarning(saveTaskLevel.Exception.ToString());
        } else {
            Debug.Log("Save Success");
        }
    }

    public void FetchLevels(Action<List<ParseObject>> callback) {
        //We store the method that will be called, when the server call has been completed
        onFetchedLevelsSuccess = callback;
        StartCoroutine(FetchLevelsCoroutine());
    }

    private IEnumerator FetchLevelsCoroutine() {
        ParseQuery<ParseObject> query = ParseObject.GetQuery("Levels");
        //We find all the levels in our levels class
        //We currently load all levels, both name and world. In the final version we should load only the names and after the click load only the selected level
        Task<IEnumerable<ParseObject>> fetchLevelsTask = query.FindAsync();
        while (!fetchLevelsTask.IsCompleted)
            yield return null;

        if (fetchLevelsTask.IsFaulted || fetchLevelsTask.IsCanceled) {
            Debug.LogWarning(fetchLevelsTask.Exception.ToString());
        } else {
            Debug.Log("Fetch Success");
            //The result contains all the levels in our database
            List<ParseObject> levels = new List<ParseObject>(fetchLevelsTask.Result);
            if (onFetchedLevelsSuccess != null)
                onFetchedLevelsSuccess(levels);
        }
    }

}
