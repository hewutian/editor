using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(CustomMap))]
public class MapInspector : Editor
{
    public string mapname = "test";
    public CustomMap cm;
    public static e_ItemType chooseType;
    public SerializedProperty itemlist;
    public SerializedProperty unreachable;
    public SerializedProperty designerNode;
    public SerializedProperty designerArea;

    void OnEnable()
    {
        mapname = target.name;
        //     itemlist = 
        itemlist = serializedObject.FindProperty("itemlist");
        unreachable = serializedObject.FindProperty("unreachable");
        designerNode = serializedObject.FindProperty("designerNode");
        designerArea = serializedObject.FindProperty("designerArea");

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        GUILayout.Label(string.Format("当前地图 :{0}", mapname));
        if (GUILayout.Button("显示地图"))
        {
            MapSceneView.Instance.OriginalSceneDelegate();
            //SetMapStage(0);
            //每次至多只应该有一个CustomMap处于被编辑状态
            if (existCustomMapBeGenerated())
            {
                if (UnityEditor.EditorUtility.DisplayDialog("Info", "Start A New CustomMap Edit?", "Yes", "Cancel"))
                {
                    
                    SceneView.RepaintAll();
                    Object.DestroyImmediate(GameObject.FindObjectOfType<SceneMark>().gameObject);
                    GenerateBaseData();
                    GenerareCustomData();
                }
            }
            else
            {
                GenerateBaseData();
                GenerareCustomData();
            }
        }
        //if (GUILayout.Button("GenerateCustomData"))
        //    GenerareCustomData();
        //if (GUILayout.Button("调整不可达格子"))
        //{
        //    if (target.name != GameObject.FindObjectOfType<SceneMark>().gameObject.name)
        //    {
        //        if (UnityEditor.EditorUtility.DisplayDialog("Error", "You are editing another CustomMap!\nYou can click the Button <GenerateBaseData> to Change Edit target", "Ok"))
        //        {
        //            //DoNothing
        //        }
        //    }
        //    else
        //    {
        //        SetMapStage(1);
        //        SceneView.RepaintAll();//立刻重绘不等待Delegate
        //    }
        //}
        
        //if (GUILayout.Button("绘制点与区域"))
        //{
        //    if (target.name != GameObject.FindObjectOfType<SceneMark>().gameObject.name)
        //    {
        //        if (UnityEditor.EditorUtility.DisplayDialog("Error", "You are editing another CustomMap!\nYou can click the Button <GenerateBaseData> to Change Edit target", "Ok"))
        //        {
        //            //DoNothing
        //        }
        //    }
        //    else
        //    {
        //        SetMapStage(3);
        //        SceneView.RepaintAll();//立刻重绘不等待Delegate
        //    }
        //}

        //if (GUILayout.Button("放置物体"))
        //{
        //    SetMapStage(2);
        //    SceneView.RepaintAll();//立刻重绘不等待Delegate;
        //}

        //if (GUILayout.Button("Reset Stage"))
        //{
        //    SetMapStage(0);
        //    SceneView.RepaintAll();//立刻重绘不等待Delegate
        //}
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("清除点数据"))
            ClearCustomData(1);
        if (GUILayout.Button("清除区域数据"))
            ClearCustomData(2);
        if (GUILayout.Button("清除物体数据"))
            ClearCustomData(3);
        GUILayout.EndHorizontal();
        //if (GUILayout.Button("Save CustomMap as Json"))
        //    CustomMapJsonMgr.MapDataToJson(target);
        //if (GUILayout.Button("Import Json as CustomMap"))
        //{
        //    string file_name = EditorUtility.OpenFilePanelWithFilters("Json File", Application.dataPath + "/Json", new string[2] { "JSON", "json" });
        //    if (file_name != "")
        //    {
        //        CustomMapJsonMgr.JsonToMapData((CustomMap)target, file_name);
        //    }
        //}
        // if (GUILayout.Button("导出数据到TXT"))
        // {
        //     CustomMap2DataPool.Convert2DataPool((CustomMap)target);
        // }
        //if (GUILayout.Button("Save CustomMap designer data"))
        //{
        //    MapModifier.Instance.Save();
        //}
        if (GUILayout.Button("编辑页面"))
        {
            MapSceneView.Instance.ShowMainEditPage();
        }
        if (cm == null)
            cm = (CustomMap)target;
        MapModifier.Instance.SetCustomMap(cm);
        //EditorGUILayout.PropertyField(unreachable);
        //EditorGUILayout.PropertyField(itemlist);
        //EditorGUILayout.PropertyField(designerNode);
        //EditorGUILayout.PropertyField(designerArea);
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
        MapSceneView.Instance.CameraTop(cm.center);
        MapModifier.Instance.GenerateBaseData();

        SceneMark mark = tmp.GetComponent<SceneMark>();
        if (mark == null)
        {
            mark = tmp.AddComponent<SceneMark>();
            mark.customMapName = mapname;
        }
     }
    //设置当前的地图编辑阶段
    //void SetMapStage(int i)
    //{
    //   MapSceneView.Instance.SetMapDesignStage(i);
    //}
    //清除所有后来添加的信息，
    void ClearCustomData(int index)
    {
        switch (index)
        {
            case 1:
                cm.designerNode.Clear();
                break;
            case 2:
                cm.designerArea.Clear();
                break;
            case 3:
                cm.itemlist.Clear();
                break;
            default:
                break;
        //cm.unreachable.Clear();
         }
       // cm.hasGeneratedData = false;
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
    private bool existCustomMapBeGenerated()
    {
        return GameObject.FindObjectOfType<SceneMark>() != null;
    }

}
