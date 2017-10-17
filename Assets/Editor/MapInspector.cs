using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(CustomMap))]
public class MapInspector : Editor {
        
    public string mapname = "test";
    public CustomMap cm;
    public static e_ItemType chooseType;


    //
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
        {
            SetMapStage(1);
            SceneView.RepaintAll();//立刻重绘不等待Delegate
        }
        if (GUILayout.Button("SecondStage"))
        {
            SetMapStage(2);
            SceneView.RepaintAll();//立刻重绘不等待Delegate;
        }
        if (GUILayout.Button("Reset Stage"))
        {
            SetMapStage(0);
            SceneView.RepaintAll();//立刻重绘不等待Delegate
        }
        if (GUILayout.Button("Clear custom Data"))
           ClearCustomData();
        if (cm == null)
            cm = (CustomMap)target;
        MapModifier.Instance.SetCustomMap(cm);
        base.OnInspectorGUI();
    }
    //创建地图，并根据地图来生成不可达数据
    void GenerateBaseData()
    {
        GameObject tmp = GameObject.Find(mapname);
        if (tmp == null)
        {
            tmp = GameObject.Instantiate(cm.scene);
            tmp.name = mapname;
        }
        MapSceneView.Instance.CameraTop(tmp);
        MapModifier.Instance.GenerateBaseData();

        SceneMark mark = tmp.GetComponent<SceneMark>();
        if (mark == null)
        {
            mark = tmp.AddComponent<SceneMark>();
            mark.customMapName = mapname;
        }
    }
    //设置当前的地图编辑阶段
    void SetMapStage(int i)
    {
       MapSceneView.Instance.SetMapDesignStage(i);
    }
    //清除所有后来添加的信息，
    void ClearCustomData()
    {
        cm.itemlist.Clear();
        cm.unreachable.Clear();
        cm.hasGeneratedData = false;
    }
    //根据地图中的物体信息，来生成他们
    public void GenerareCustomData()
    {
        foreach(var e in cm.itemlist)
        {
            if (e != null)
                MapModifier.Instance.CreateGameObjectAndAddUnreachable(e);
        }
    }        
    
    void OnEnable()
    {
        mapname = target.name;
    }
}
