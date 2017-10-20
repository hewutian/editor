using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class MapSceneView
{
    static MapSceneView instance;


    Vector3 startpoint;
    Vector3 endpoint;
    bool isDrag = false;
    bool chooseObj = false;
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
        SceneView.onSceneGUIDelegate -= SceneGuiDelegateFirst;
        SceneView.onSceneGUIDelegate -= SceneGuiDelegateSecond;
        SceneView.onSceneGUIDelegate -= SceneGuiDelegatePointAndArea;
        switch (num)
        {
            case 1:
                SceneView.onSceneGUIDelegate += SceneGuiDelegateFirst;
                break;
            case 2:
                SceneView.onSceneGUIDelegate += SceneGuiDelegateSecond;
                break;
            case 3:
                SceneView.onSceneGUIDelegate += SceneGuiDelegatePointAndArea;
                break;
            default:
                
                break;
        }
    }
    //第一阶段中对于事件的处理和辅助显示
    void SceneGuiDelegateFirst(SceneView sv)
    {
        FirstStageGuiTool(sv);
        CustomMap cm = MapModifier.Instance.CurMap;
        Vector3 mapsize = MapModifier.Instance.MapSize;
        MapAux.DrawMapCells(cm.center, mapsize, cm.unitlength, Color.yellow);
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
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        HandleUtility.AddDefaultControl(controlID);
        //if (current == null)
        //    return;
        Vector3 collisionPos = MapModifier.Instance.CaculateCollisionPosFromGUIPoint(current.mousePosition);

        if (collisionPos.y == float.MaxValue)
            return;

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
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        HandleUtility.AddDefaultControl(controlID);
        Vector3 collisionPos = MapModifier.Instance.CaculateCollisionPosFromGUIPoint(current.mousePosition);
        if (collisionPos.y == float.MaxValue)
            return;
        collisionPos = new Vector3(collisionPos.x, 0, collisionPos.z);
        Vector3 lefttopcenter = MapModifier.Instance.CaculateCellCenterByPos(collisionPos);
        Vector3 objectsize = MapModifier.Instance.CaculateGameObjectSize(selected);
        int lefttopindex = MapModifier.Instance.CaculateIndexForPos(lefttopcenter);
        //Vector3 buildcenter = MapModifier.Instance.CaculateCreateGameObjectCenter(lefttopcenter, objectsize);
        //var flag = MapModifier.Instance.CheckContainUnreachable(lefttopindex, objectsize);
        //if (flag == true)
        //    MapAux.DrawLines(lefttopcenter, objectsize, Color.red);
        //else
        //    MapAux.DrawLines(lefttopcenter, objectsize, Color.green);
        int biggridindex = MapModifier.Instance.CaculatePaintedGridFromUnitIndex(lefttopindex);
        //int girdlefttopindex = MapModifier.Instance.CaculateLefttopUnitIndexOfGrid(biggridindex);
        int[] unitindexes = MapModifier.Instance.CaculateUnitIndexesOfGrid(biggridindex);
        int girdlefttopindex = unitindexes[0];
        Vector3 girdlefttopcenter = MapModifier.Instance.TranselateIndexToPostion(girdlefttopindex);
        Vector3 buildcenter = MapModifier.Instance.CaculateCreateGameObjectCenter(girdlefttopcenter, objectsize);
        var flag = MapModifier.Instance.CheckContainUnreachable(girdlefttopindex, objectsize);
        if (flag == true)
            MapAux.DrawLines(girdlefttopcenter, objectsize, Color.red);
        else
            MapAux.DrawLines(girdlefttopcenter, objectsize, Color.green);
        switch (current.type)
        {
            case EventType.mouseDown:
                if (current.button == 0 && (!flag))
                {
                    MapModifier.Instance.AddObject(girdlefttopindex, buildcenter, selected);
                    MapModifier.Instance.AddNewItem(girdlefttopindex, objectsize, selected);
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



    void SceneGuiDelegatePointAndArea(SceneView sv)
    {
        PointAndAreaGUITool(sv);

        CustomMap cm = MapModifier.Instance.CurMap;
        Vector3 mapsize = MapModifier.Instance.MapSize;
        MapAux.DrawMapCells(cm.center, mapsize, cm.unitlength, Color.yellow);
        foreach (var e in cm.unreachable)
        {
            Vector3 center = MapModifier.Instance.TranselateIndexToPostion(e);
            MapAux.DrawMapUnreachableArea(center, cm.unitlength, Color.red);
        }
        //foreach(var e in cm.designerArea)
        //{
        //    MapAux.DrawRectHandles(e.start, e.end);
        //}
        MapModifier.Instance.ShowAreaFreeMoveHandles();
        MapModifier.Instance.ShowPointFreeMoveHandles();
        //Handles.color = Color.yellow;
        //foreach (var e in cm.designerNode)
        //{
          
        //    Handles.DrawWireDisc(e.site, Vector3.up, .25f);
        //}

        DealWithGUIEventPointAndAreaStage();
    }

    void PointAndAreaGUITool(SceneView sv)
    {
        //Handles.BeginGUI();
        //var re = sv.position;
        //GUILayout.BeginArea(new Rect(0, re.height - 100, re.width, 100), EditorStyles.toolbarButton);//EditorStyles.toolbarButton);
        //{
        //    if (GUILayout.Button("edit area"))
        //        chooseObj = true;
        //    if (GUILayout.Button("draw area"))
        //        chooseObj = false;
        //}
        //GUILayout.EndArea();
        //Handles.EndGUI();
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(Screen.width - 100, Screen.height - 80, 90, 50));
        if (GUILayout.Button("edit area"))
            chooseObj = true;
        if (GUILayout.Button("draw area"))
            chooseObj = false;
        GUILayout.EndArea();
        Handles.EndGUI();
    }
    //处理gui事件
    void DealWithGUIEventPointAndAreaStage()
    {
        Event current = Event.current;
        
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        //if(chooseObj == false)
           

        Vector3 collisionPos = MapModifier.Instance.CaculateCollisionPosFromGUIPoint(current.mousePosition);
        if (collisionPos.y == float.MaxValue)
            return;
        collisionPos = new Vector3(collisionPos.x, 0, collisionPos.z);
        Vector3 lefttopcenter = MapModifier.Instance.CaculateCellCenterByPos(collisionPos);
        int lefttopindex = MapModifier.Instance.CaculateIndexForPos(lefttopcenter);
        Vector3 size = new Vector3(1, 0, 1);
        var flag = MapModifier.Instance.CheckContainUnreachable(lefttopindex, size);

  

        //if (flag)
        //{
        //    Handles.color = Color.green;
        //}
        //else
        //{
        //    Handles.color = Color.red;
        //}
        Handles.color = Color.green;
        Handles.DrawWireDisc(collisionPos, Vector3.up, .5f);
        Debug.Log(current.type);
        if (isDrag == true)
        {
            MapAux.DrawRectHandles(startpoint, endpoint);
        }
       if (chooseObj == false)
        {
            HandleUtility.AddDefaultControl(controlID);
            switch (current.type)
            {

                case EventType.mouseDown:
                    if (current.button == 0 && (!isDrag))
                    {
                        startpoint = collisionPos;
                        // current.Use();
                    }
                    else if (current.button == 0 && (!isDrag))
                    {
                        //endpoint = collisionPos;
                    }
                    break;
                case EventType.mouseUp:
                    if (current.button == 0 && (isDrag == false))
                    {
                        MapModifier.Instance.AddPoint(startpoint);

                        // current.Use();
                    }
                    else if (current.button == 0 && (isDrag == true))
                    {
                        endpoint = collisionPos;
                        MapModifier.Instance.AddArea(startpoint, endpoint);

                        isDrag = false;
                    }
                    break;
                case EventType.mouseDrag:
                    if (current.button == 0)
                    {
                        endpoint = collisionPos;
                        isDrag = true;
                        // current.Use();
                    }
                    break;
                default:
                    break;
            }
        }
        SceneView.RepaintAll();
    }
}
