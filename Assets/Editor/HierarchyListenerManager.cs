using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class NewBehaviourScript : HierarchyListener
{
    public static Dictionary<int, GameObject> gameDict = new Dictionary<int, GameObject>();
    public static List<int> instanceIDArray = new List<int>();

    public override void Update()
    {
        //只在Editor下处理
#if UNITY_EDITOR
        //Debug.Log ("每一帧回调一次");
        instanceIDArray.Clear();
        GameObject[] pAllObjects = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));
        foreach (GameObject obj in pAllObjects)
        {
            if (obj.GetComponent<SceneMark>() != null)
            { 
                instanceIDArray.Add(obj.GetInstanceID());
            }
        }

        List<int> toDelete = new List<int>();
        foreach (KeyValuePair<int, GameObject> dic in gameDict)
        {
            if (instanceIDArray.Contains(dic.Key))
                continue;
            else
            {
                toDelete.Add(dic.Key);
                MapSceneView.Instance.OriginalSceneDelegate();
               
            }
        }

        foreach (int key in toDelete)
        {
            gameDict.Remove(key);
        }
#endif
    }

    public override void OnPlaymodeStateChanged(PlayModeState playModeState)
    {
        //Debug.Log ("游戏运行模式发生改变， 点击 运行游戏 或者暂停游戏或者 帧运行游戏 按钮时触发: " + playModeState);
    }

    public override void OnGlobalEventHandler(Event e)
    {
        Debug.Log("全局事件回调: " + e);
    }

    public override void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        //只在Editor模式下处理
#if UNITY_EDITOR
        //Debug.Log (string.Format ("{0} : {1} - {2}", EditorUtility.InstanceIDToObject (instanceID), instanceID, selectionRect));
        GameObject instance = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (instance != null && instance.GetComponent<SceneMark>() != null)
        {
            if (!gameDict.ContainsKey(instanceID) || !gameDict.ContainsValue(instance))
            {
                gameDict.Add(instanceID, instance);
            }
        }
#endif
    }

    public override void OnHierarchyWindowChanged()
    {
        //Debug.Log("test");
    }

    public override void OnModifierKeysChanged()
    {
        Debug.Log("当触发键盘事件");
        
    }

    public override void OnProjectWindowChanged()
    {
        //	Debug.Log ("当资源视图发生变化");
    }

    public override void ProjectWindowItemOnGUI(string guid, Rect selectionRect)
    {
        //根据GUID得到资源的准确路径
        //Debug.Log (string.Format ("{0} : {1} - {2}", AssetDatabase.GUIDToAssetPath (guid), guid, selectionRect));
    }
}