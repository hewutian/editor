using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(CustomMap))]
public class MapInspector : Editor {
        
    public string mapname = "test";
    public CustomMap cm;
    public static e_ItemType chooseType;

    public override void OnInspectorGUI()
    {
        GUILayout.Label(string.Format("this is a custom map :{0}", mapname));
        // if (GUILayout.Button("open map designer editor"))
        //    MapDesignerWindow.Init();
        if (GUILayout.Button("GenerateBaseData"))
            GenerateBaseData();
        if (GUILayout.Button("GenerateCustomData"))
            GenerareCustomData();
        if (GUILayout.Button("FirstStage"))
            SetMapStage(1);
        if (GUILayout.Button("SecondStage"))
            SetMapStage(2);
        if (GUILayout.Button("Clear custom Data"))
           ClearCustomData();
        if (cm == null)
            cm = (CustomMap)target;
        base.OnInspectorGUI();
    }

    void GenerateBaseData()
    {
        GameObject tmp = GameObject.Instantiate(cm.scene);
        MapSceneView.Instance.CameraTop(tmp);
        MapModifier.Instance.GenerateBaseData();
    }

    void SetMapStage(int i)
    {
       MapSceneView.Instance.SetMapDesignStage(i);
    }

    void ClearCustomData()
    {
        cm.itemlist.Clear();
        cm.unreachable.Clear();
    }

    public void GenerareCustomData()
    {
        foreach(var e in cm.itemlist)
        {
            if (e != null)
                MapModifier.Instance.CreateGameObjectAndAddUnreachable(e);
        }
    }
        
    
}
