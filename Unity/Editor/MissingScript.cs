using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissingScript
{
    public static void CheckSelectGameObjects()
    {
        Check(new List<GameObject>(Selection.gameObjects));
    }

    public static void CheckScene()
    {
        //Get the current scene and all top-level GameObjects in the scene hierarchy
        Scene currentScene = SceneManager.GetActiveScene();
        var list = new List<GameObject>();
        currentScene.GetRootGameObjects(list);
        Check(list);
    }

    private static void Check(List<GameObject> goList)
    {
        List<Transform> objectsWithDeadLinks = new List<Transform>();
        foreach (GameObject g in goList)
        {
            Check(g.transform, objectsWithDeadLinks);
        }
        if (objectsWithDeadLinks.Count == 0)
        {
            Debug.Log("no missing scripts found");
        }
    }

    private static void Check(Transform trans, List<Transform> deadList)
    {
        foreach (var currentComponent in trans.GetComponents<Component>())
        {
            //If the component is null, that means it's a missing script!
            if (currentComponent == null)
            {
                //Add the sinner to our naughty-list
                deadList.Add(trans);
                Debug.Log(trans.gameObject + " has a missing script!", trans);
                break;
            }
        }
        
        for (int i = 0; i < trans.childCount; i++)
        {
            Check(trans.GetChild(i), deadList);
        }
    }
}