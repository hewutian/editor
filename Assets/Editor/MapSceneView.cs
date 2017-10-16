using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class MapSceneView
{
    static MapSceneView instance;
    public static MapSceneView Instance
    {
        get
        {
            if (instance == null)
                instance = new MapSceneView();
            return instance;
        }
    }
    //选择的模型预览的索引数
    static int selected = 0;
    public static int SelectedTool
    {
        get
        {
            return selected;
        }
        set
        {
            if (value == SelectedTool)
            {
                return;
            }
            else
                selected = value;
            //ResourceCenter.Instance.prefabObjects[]
        }
    }
    //设置观察的摄像机位姿
    public void CameraTop(GameObject o)
    {
        SceneView view = SceneView.lastActiveSceneView;
        if (view)
        {
            view.camera.orthographic = false;
            view.rotation = Quaternion.Euler(90f, 0f, 0f);
            Vector3 newpos = o.transform.position;
            // newpos.z = -10;
            view.pivot = o.transform.position;
            view.size = 30f;
            view.orthographic = false;
        }
    }
    //第一阶段的gui显示
    void FirstStageGuiTool(SceneView sv)
    {

    }
    //第二阶段的 gui显示
    void SecondStageGuiTool(SceneView sv)
    {
        Handles.BeginGUI();
        var re = sv.position;
        GUILayout.BeginArea(new Rect(0, re.height - 100, re.width, 100), EditorStyles.objectFieldThumb);//EditorStyles.toolbarButton);
        {
            ResourceCenter.Instance.Init("Assets/prefab/");
            var objs = ResourceCenter.Instance.prefabObjects;
            var thumbs = ResourceCenter.Instance.thumbnails;
            GUIContent[] gc = new GUIContent[objs.Length];
            for (int i = 0; i < gc.Length; ++i)
            {
                gc[i] = new GUIContent(thumbs[i]);
            }
            SelectedTool = GUILayout.SelectionGrid(
                SelectedTool,
                gc,
                gc.Length,
               EditorStyles.objectFieldThumb,//EditorStyles.toolbarButton,
               GUILayout.Width(200),
               GUILayout.Height(100));
        }
        GUILayout.EndArea();
        Handles.EndGUI();
    }
    //设置地图编辑的阶段
    public void SetMapDesignStage(int num)
    {
        switch (num)
        {
            case 1:
                SceneView.onSceneGUIDelegate -= SceneGuiDelegateSecond;
                SceneView.onSceneGUIDelegate += SceneGuiDelegateFirst;
                break;
            case 2:
                SceneView.onSceneGUIDelegate -= SceneGuiDelegateFirst;
                SceneView.onSceneGUIDelegate += SceneGuiDelegateSecond;
                break;
            default:
                SceneView.onSceneGUIDelegate -= SceneGuiDelegateFirst;
                SceneView.onSceneGUIDelegate -= SceneGuiDelegateSecond;
                break;
        }
    }
    //第一阶段中对于事件的处理和辅助显示
    void SceneGuiDelegateFirst(SceneView sv)
    {
        FirstStageGuiTool(sv);
        CustomMap cm = MapModifier.Instance.CurMap;
        Vector3 mapsize = MapModifier.Instance.MapSize;
        MapAux.DrawMapCells(new Vector3(0, 0, 0), mapsize, cm.unitlength, Color.yellow);
        foreach (var e in cm.unreachable)
        {
            Vector3 center = MapModifier.Instance.TranselateIndexToPostion(e);
            MapAux.DrawMapUnreachableArea(center, cm.unitlength, Color.red);
        }
        DealWithGUIEventFirstStage();
    }
    //处理gui事件
    void DealWithGUIEventFirstStage()
    {
        Event current = Event.current;
        //if (current == null)
        //    return;
        Vector3 collisionPos = MapModifier.Instance.CaculateCollisionPosFromGUIPoint(current.mousePosition);
        collisionPos = new Vector3(collisionPos.x, 0, collisionPos.z);
        Vector3 lefttopcenter = MapModifier.Instance.CaculateCellCenterByPos(collisionPos);
        int lefttopindex = MapModifier.Instance.CaculateIndexForPos(lefttopcenter);
        Vector3 size = new Vector3(1, 0, 1);
        var flag = MapModifier.Instance.CheckContainUnreachable(lefttopindex, size);
        if(flag)
        {
            Handles.color = Color.green;
        }
        else
        {
            Handles.color = Color.red;
        }
        //  Vector3 objectsize = 
        
        Handles.DrawSolidDisc(lefttopcenter,Vector3.up,.5f);
        switch (current.type)
        {
            case EventType.mouseDown:
                if (current.button == 0)
                {
                    if(flag == false)
                        MapModifier.Instance.AddUnreachableIndex(lefttopindex);
                    else
                        MapModifier.Instance.RemoveUnreachableIndex(lefttopindex);
                    current.Use();
                }
                else if (current.button == 1)
                {
                    // Debug.Log(current.keyCode);
                }
                break;
            default:
                break;
        }


        SceneView.RepaintAll();
    }

    //第二阶段中对于事件的处理和辅助显示
    void SceneGuiDelegateSecond(SceneView sv)
    {
        SecondStageGuiTool(sv);
        DealWithGUIEventSecondStage();
    }
    //处理gui事件
    void DealWithGUIEventSecondStage()
    {
        Event current = Event.current;
        Vector3 collisionPos = MapModifier.Instance.CaculateCollisionPosFromGUIPoint(current.mousePosition);
        collisionPos = new Vector3(collisionPos.x, 0, collisionPos.z);
        Vector3 lefttopcenter = MapModifier.Instance.CaculateCellCenterByPos(collisionPos);
        Vector3 objectsize = MapModifier.Instance.CaculateGameObjectSize(selected);
        int lefttopindex = MapModifier.Instance.CaculateIndexForPos(lefttopcenter);
        Vector3 buildcenter = MapModifier.Instance.CaculateCreateGameObjectCenter(lefttopcenter,objectsize);
        var flag = MapModifier.Instance.CheckContainUnreachable(lefttopindex, objectsize);
        if (flag == true)
            MapAux.DrawLines(lefttopcenter, objectsize, Color.red);
        else
            MapAux.DrawLines(lefttopcenter, objectsize, Color.green);

        switch (current.type)
        {
            case EventType.mouseDown:
                if (current.button == 0 && (!flag))
                {
                    MapModifier.Instance.AddObject(lefttopindex, buildcenter, selected);
                    MapModifier.Instance.AddNewItem(lefttopindex, objectsize, selected);
                    current.Use();
                }
                else if (current.button == 1)
                {
                    // Debug.Log(current.keyCode);
                }
                break;
            default:
                break;
        }
        SceneView.RepaintAll();
    }
}
