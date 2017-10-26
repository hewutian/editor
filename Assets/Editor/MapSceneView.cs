using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class MapSceneView
{
    static MapSceneView instance;
    EditorHandler curhandler;
    public static MapSceneView Instance
    {
        get
        {
            if (instance == null)
                instance = new MapSceneView();
            return instance;
        }
    }
     //设置观察的摄像机位姿
    public void CameraTop(Vector3 center)
    {
        SceneView view = SceneView.lastActiveSceneView;
        if (view)
        {
            view.camera.orthographic = false;
            view.rotation = Quaternion.Euler(90f, 0f, 0f);
            //Vector3 newpos = o.transform.position;
            // newpos.z = -10;
            view.pivot = center;//o.transform.position;
            view.size = 30f;
            view.orthographic = false;
        }
    }
   
    public void ShowMainEditPage()
    {
        SceneView.onSceneGUIDelegate -= MainPage;
        SceneView.onSceneGUIDelegate += MainPage;
    }

    public void OriginalSceneDelegate()
    {
        SceneView.onSceneGUIDelegate -= MainPage;
    }
    
    void MainPage(SceneView sv)
    {
        Handles.BeginGUI();
        var re = sv.position;
        //int edittype = 0;
        GUILayout.BeginArea(new Rect(0, 0, re.width, 100));//EditorStyles.toolbarButton);
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("寻路小格"))
            {
                MapEditorFSM.Instance.curState = e_Editor_State.Edit_Map | e_Editor_State.Edit_Pathfind_Cell;
                curhandler = new CellEditorHandler(MapModifier.Instance.CurMap);
            }
            if (GUILayout.Button("点和区域"))
            {
                MapEditorFSM.Instance.curState = e_Editor_State.Edit_Map | e_Editor_State.Edit_Point_and_Area;
                curhandler = new PointAndAreaEditorHandler(MapModifier.Instance.CurMap);
            }
            if (GUILayout.Button("物体"))
            {
                MapEditorFSM.Instance.curState = e_Editor_State.Edit_Map | e_Editor_State.Edit_Object;
                curhandler = new GameObjectEditorHandler(MapModifier.Instance.CurMap);
            }
            //if (GUILayout.Button("修改"))
            //    curhandler = null;
            //if (GUILayout.Button("删除"))
            //    curhandler = null;
            GUILayout.EndHorizontal();
        }
        GUILayout.EndArea();
        Handles.EndGUI();
        HandleUtility.Repaint();
        if (curhandler != null)
        {
            curhandler.ShowAuxInfo();
            curhandler.DealWithEvent();
        }

     }
}
