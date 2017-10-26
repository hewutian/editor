using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
public class MapDesignerWindow : EditorWindow {

    public static MapDesignerWindow mapdesignerWind;
    public static CustomItemInfo sentItemInfo;



    public static Camera mainCam = null;
    public Vector3 initialCamPosition;
    public Quaternion initialCamRotation;
    public float initialFieldOfView;
    public e_ItemType chooseType;


    public static void Init()
    {
        mapdesignerWind = EditorWindow.GetWindow<MapDesignerWindow>(false, "MapDesigner", true);
        mapdesignerWind.Close();
        mapdesignerWind = EditorWindow.GetWindow<MapDesignerWindow>(false, "MapDesigner", true);
        mapdesignerWind.Show();

        EditorWindow.FocusWindowIfItsOpen<SceneView>();

        Camera sceneCam = GameObject.FindObjectOfType<Camera>();
        if(sceneCam != null)
        {
            mapdesignerWind.initialCamPosition = sceneCam.transform.position;
            mapdesignerWind.initialCamRotation = sceneCam.transform.rotation;
            mapdesignerWind.initialFieldOfView = sceneCam.fieldOfView;
        }

        mainCam = Camera.main;
        if(mainCam == null)
        {
            mainCam = new GameObject("Main Camera").AddComponent<Camera>();
            mainCam.tag = "MainCamera";
        }

        mainCam.orthographic = true;
        mainCam.transform.position = new Vector3(0f, 1f, -1.5f);
        mainCam.orthographicSize = 2.5f;
        mainCam.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0);


    }

     void Update()
    {
        //Debug.Log("window");
    }

     void OnGUI()
    {

        GUILayout.BeginVertical();
        string[] types = { "tree", "car", "stone" };
        //for(int i = 0;i<3;++i)
       // {
        //    if (GUILayout.Button(types[i]))
        //        chooseType = (e_ItemType)i;
        // }

        if (GUILayout.Button("tree"))
            chooseType = e_ItemType.Tree;
        if (GUILayout.Button("box"))
            chooseType = e_ItemType.Box;
        if (GUILayout.Button("stone"))
            chooseType = e_ItemType.Stone;
        GUILayout.EndVertical();
        GUILayout.Label("save data to asset");
        if(GUILayout.Button("save data"))
        {
            Save();
        }
    }

    //void OnSceneGUI()
    //{

    //    //if(Event.current.type == EventType.mouseDown)
    //    //{

    //    //}

    //    Event current = Event.current;
    //    // int controlID = GUIUtility.GetControlID(FocusType.Passive);
    //    switch (current.type)
    //    {
    //        case EventType.mouseDown:
    //            AddObject();
    //            break;

    //        case EventType.mouseUp:
    //            AddObject();
    //            current.Use();
    //            break;
    //        case EventType.layout:
    //            // HandleUtility.AddDefaultControl(controlID);
    //            break;
    //    }

    //}

    void AddObject()

    {
       // Debug.Log("mouse click");
    }

    public static void Save()
    {

    }
}
