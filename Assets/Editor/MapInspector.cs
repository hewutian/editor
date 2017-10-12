using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(CustomMap))]
public class MapInspector : Editor {
        
    public string mapname = "test";
    public CustomMap cm;
    static int selected = 0;
    public static e_ItemType chooseType;
    void DrawHandlesCube(Vector3 center,Color cl)
    {
        int xlength = 1;
        int ylength = 1;
        int zlength = 1;
        if (SelectedTool != null)
        {
            var obj = ResourceCenter.Instance.prefabObjects[SelectedTool];
            //var size = obj.GetComponent<Collider>().bounds.size;
            var size = obj.GetComponent<Renderer>().bounds.size;
            xlength = (int)Mathf.Ceil(size.x / cm.unitlength);
            zlength = (int)Mathf.Ceil(size.z / cm.unitlength);
            ylength = (int)Mathf.Ceil(size.y / cm.unitlength);
        }
        Vector3 p2 = center + Vector3.up * 0f + Vector3.right * (xlength - 0.5f) + Vector3.forward * 0.5f * 1;
        Vector3 p3 = center + Vector3.up * 0f + Vector3.right * (xlength - 0.5f) - Vector3.forward * (zlength - 0.5f);
        Vector3 p4 = center + Vector3.up * 0f - Vector3.right * 0.5f * 1 - Vector3.forward * (zlength - 0.5f);

        Vector3 p1 = center + Vector3.up * 0f - Vector3.right * 0.5f * 1 + Vector3.forward * 0.5f * 1;

        //Vector3 p1 = center + Vector3.up * 0.5f + Vector3.right * 0.5f + Vector3.forward * 0.5f;
        //Vector3 p2 = center + Vector3.up * 0.5f + Vector3.right * 0.5f - Vector3.forward * 0.5f;
        //Vector3 p3 = center + Vector3.up * 0.5f - Vector3.right * 0.5f - Vector3.forward * 0.5f;
        //Vector3 p4 = center + Vector3.up * 0.5f - Vector3.right * 0.5f + Vector3.forward * 0.5f;



        //Vector3 p5 = center - Vector3.up * 0.5f + Vector3.right * 0.5f + Vector3.forward * 0.5f;
        //Vector3 p6 = center - Vector3.up * 0.5f + Vector3.right * 0.5f - Vector3.forward * 0.5f;
        //Vector3 p7 = center - Vector3.up * 0.5f - Vector3.right * 0.5f - Vector3.forward * 0.5f;
        //Vector3 p8 = center - Vector3.up * 0.5f - Vector3.right * 0.5f + Vector3.forward * 0.5f;
        Handles.color = cl;
        Handles.DrawLine(p1, p2);
        Handles.DrawLine(p2, p3);
        Handles.DrawLine(p3, p4);
        Handles.DrawLine(p4, p1);
        //Handles.DrawLine(p5, p6);
        //Handles.DrawLine(p6, p7);
        //Handles.DrawLine(p7, p8);
        //Handles.DrawLine(p8, p5);
        //Handles.DrawLine(p1, p5);
        //Handles.DrawLine(p2, p6);
        //Handles.DrawLine(p3, p7);
        //Handles.DrawLine(p4, p8);
    }

    void DrawFrameByItemType(e_ItemType it,Vector3 center)
    {
    }
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
            chooseType = (e_ItemType)value;
            //ResourceCenter.Instance.prefabObjects[]
            switch (value)
            {
                case 0:
            
                    break;
                case 1:
                  
                    break;
                case 2:
                  
                    break;
                default:
                  
                    break;
            }
        }
    }
    private void OnDestroy()
    {
        //SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }
      void TestTool(SceneView sv)
    {
        Handles.BeginGUI();
        var re = sv.position;
        //var newstyle = new GUIStyle();
        //newstyle.fixedHeight = 80;
        //newstyle.fixedHeight = 80;
        GUILayout.BeginArea(new Rect(0, re.height - 100, re.width, 100), EditorStyles.objectFieldThumb);//EditorStyles.toolbarButton);
        {
            // string[] buttonLabels = new string[] { "1", "2", "3" };
            ResourceCenter.Instance.Init("Assets/prefab/");
            var objs = ResourceCenter.Instance.prefabObjects;
            var thumbs = ResourceCenter.Instance.thumbnails;
            GUIContent[] gc = new GUIContent[objs.Length];
            for(int i = 0;i< gc.Length;++i)
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
        void OnSceneGUI(SceneView sv)
    {
        TestTool(sv);
        Event current = Event.current;
        if (current == null)
            return;
        var collisionPos = CaculateCollisionPos();
        collisionPos = new Vector3(collisionPos.x, 0, collisionPos.z);
        e_ItemType itemtype = e_ItemType.Box ;
        var pos = CaculateBuildPos(collisionPos, itemtype);
        DrawHandlesCube(pos,Color.green);
        // int controlID = GUIUtility.GetControlID(FocusType.Passive);
        switch (current.type)
        {
            case EventType.mouseDown:
                //
                if (current.button ==0)
                {
                    AddObject();
                    current.Use();
                }
                else
                {
                   // Debug.Log(current.keyCode);
                }
                break;
            default:
                break;
        }
        //current.Use();
        SceneView.RepaintAll();
    }
    void Update()
    {
        Debug.Log(" update ");
    }
    Vector3 CaculateCollisionPos()
    {
        var mousepos = Event.current.mousePosition;
        Ray screenTo = HandleUtility.GUIPointToWorldRay(mousepos);// SceneView.lastActiveSceneView.camera.ScreenPointToRay(mousepos);
        //Debug.Log(mousepos + "mouse");
        //Debug.Log(screenTo.origin + "origin");
       // Debug.Log(screenTo.direction + "direction");
        Vector3 pos = new Vector3();
        RaycastHit hitinfo;
        if( Physics.Raycast(screenTo, out hitinfo))
        {
            pos = hitinfo.point;
        }
        //if (Event.current.type == EventType.mouseDown)
        //{
        //    //Debug.Log(Input.mousePosition);
        //    //var p = SceneView.lastActiveSceneView.camera.ScreenToWorldPoint(new Vector3(mousepos.x, mousepos.y, SceneView.lastActiveSceneView.camera.nearClipPlane));
        //    // Gizmos.color = Color.yellow;
        //    //  Gizmos.DrawSphere(p, 0.1f);
        //    Debug.Log("----------------");
        //    Debug.Log(pos);
        //    // Debug.DrawLine(SceneView.lastActiveSceneView.camera.transform.position, pos);
        //    Debug.DrawLine(screenTo.origin, pos);
        //}
        return pos;
    }


    int CaculateIndexForPos(Vector3 pos)
    {
        Vector3 lefttop = new Vector3(cm.center.x - cm.mapwidth / 2.0f, 0, cm.center.z + cm.mapheight / 2.0f);
        int rank = (int)Mathf.Ceil((pos.x - lefttop.x) / (float)cm.unitlength);
        int row = (int)Mathf.Ceil(Mathf.Abs(pos.z - lefttop.z) / (float)cm.unitlength);
        int index = (row - 1) * cm.mapwidth / cm.unitlength + (rank - 1);
        return index;
    }

    bool CheckIfBuildItem( Vector3 pos,e_ItemType type)
    {

        Vector3 lefttop = new Vector3(cm.center.x - cm.mapwidth / 2.0f, 0, cm.center.z + cm.mapheight / 2.0f);
        int rank = (int)Mathf.Ceil((pos.x - lefttop.x) / (float)cm.unitlength);
        int row = (int)Mathf.Ceil(Mathf.Abs(pos.z - lefttop.z) / (float)cm.unitlength);
        int index = (row - 1) * cm.mapwidth / cm.unitlength + (rank - 1);
        return CheckIsUnreachable(index);
    }
    
    bool CheckIsUnreachable(int valueindex)
    {
        foreach(var i in cm.unreachable) 
        {
            if (valueindex == i)
                return false;
        }
        return true;
    }

    Vector3 CaculateBuildPos(Vector3 pos, e_ItemType type)
    {
        Vector3 lefttop = new Vector3(cm.center.x - cm.mapwidth / 2.0f, 0, cm.center.z + cm.mapheight / 2.0f);
        int rank = (int)Mathf.Ceil((pos.x - lefttop.x) / (float)cm.unitlength);
        int row = (int)Mathf.Ceil(Mathf.Abs(pos.z - lefttop.z) / (float)cm.unitlength);
        // int index = (row - 1) * cm.mapwidth / cm.unitlength + (rank);
        var indexUnitpos = new Vector3(rank*cm.unitlength + lefttop.x - cm.unitlength/2.0f, pos.y,lefttop.z - row*cm.unitlength + cm.unitlength/2.0f);
        Debug.Log(indexUnitpos + "index");
        return indexUnitpos;
    }

    void BuildOrNot(e_ItemType itemtype)
    {
        var collisionPos = CaculateCollisionPos();
        if (collisionPos == Vector3.zero)
            return;
        if (CheckIfBuildItem(collisionPos, itemtype))
        {
            GameObject objTarget;
            switch (itemtype)
            {
                case e_ItemType.Tree:
                     objTarget = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

                    break;
                case e_ItemType.Box:
                     objTarget = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    break;
                case e_ItemType.Stone:
                     objTarget = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    break;
                default:
                    return;
                    break;

            }
            //if (objTarget)
           objTarget.transform.position = CaculateBuildPos(collisionPos, itemtype);
           int index = CaculateIndexForPos(collisionPos);
            cm.unreachable.Add(index);
            CustomItemInfo newitem = new CustomItemInfo();
            newitem.type = itemtype;
            newitem.lefttopsite = index;
            newitem.prefab = objTarget;
            cm.itemlist.Add(newitem);
        }
    }

    void AddObject()
    {
        e_ItemType itemtype = MapInspector.chooseType;// MapDesignerWindow.mapdesignerWind.chooseType;
        //GameObject  objTarget;
        switch (itemtype)
        {
            case e_ItemType.Tree:
            case e_ItemType.Box:
            case e_ItemType.Stone:
                BuildOrNot(itemtype);
                break;
            default:
               // AttempToSelect();
                break;
        }  
         Debug.Log("mouse click");
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Label(string.Format("this is a custom map :{0}", mapname));


        if (GUILayout.Button("open map designer editor"))
            MapDesignerWindow.Init();

        if (GUILayout.Button("save designed map data"))
        // MapDesignerWindow.Save();
        {
            Save();
        }
        if (GUILayout.Button("GenerateFirstData"))
            GenerateFirstData();
        if (GUILayout.Button("GenerateSecondData"))
            GenerareSecondData();
        if (GUILayout.Button("Clear custom Data"))
           ClearCustomData();
        if (cm == null)
            cm = (CustomMap)target;

        base.OnInspectorGUI();
    }

    void ClearCustomData()
    {
        cm.itemlist.Clear();
        cm.unreachable.Clear();
    }

    public void GenerateFirstData()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        SceneView.onSceneGUIDelegate += OnSceneGUI;
        GameObject tmp = GameObject.Instantiate(cm.scene);
        CameraTop(tmp);
        List<Vector3> positions = new List<Vector3>();
     
        for (int i = 0; i < cm.mapwidth/ cm.tilelength; ++i)
            for (int j = 0; j < cm.mapheight/ cm.tilelength; ++j)
            {
                Vector3 pos = new Vector3((float)(i - cm.mapwidth/2 + 0.5) * cm.tilelength, 1000, (float)(j - cm.mapheight/2 + 0.5) * cm.tilelength);
                positions.Add(pos);
            }
       // AssetDatabase.Refresh();
        cm.unreachable =Detect(positions, cm.dir, cm.max);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    void CreateGameObjectAndAddUnreachable(CustomItemInfo iteminfo)
    {
        int index = iteminfo.lefttopsite;
        Vector3 lefttop = new Vector3(cm.center.x - cm.mapwidth / 2.0f, 0, cm.center.z + cm.mapheight / 2.0f);
        int rank = index % (cm.mapwidth / cm.unitlength) + 1;//(int)Mathf.Ceil((pos.x - lefttop.x) / (float)cm.unitlength);
        int row = index / (cm.mapwidth / cm.unitlength) + 1;// (int)Mathf.Ceil(Mathf.Abs(pos.z - lefttop.z) / (float)cm.unitlength);
        // int index = (row - 1) * cm.mapwidth / cm.unitlength + (rank);
        var centerpos = new Vector3(rank * cm.unitlength + lefttop.x - cm.unitlength / 2.0f, 0, lefttop.z - row * cm.unitlength + cm.unitlength / 2.0f);

        GameObject objTarget;
        switch (iteminfo.type )
        {
            case e_ItemType.Tree:
                objTarget = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

                break;
            case e_ItemType.Box:
                objTarget = GameObject.CreatePrimitive(PrimitiveType.Cube);
                break;
            case e_ItemType.Stone:
                objTarget = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                break;
            default:
                return;
                break;

        }
        //if (objTarget)
        //objTarget.transform.position = centerpos;
        objTarget.transform.position = new Vector3(centerpos.x,iteminfo.posy,centerpos.z);
        cm.unreachable.Add(index);

        //return centerpos;
    }

    public void GenerareSecondData()
    {
        foreach(var e in cm.itemlist)
        {
            if (e != null)
            CreateGameObjectAndAddUnreachable(e);
        }
    }

    private void CameraTop(GameObject o)
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
    public  List<int> Detect(List<Vector3> pos, Vector3 dir, float max)
    {
        int i = 0;
        List<int> unreachable = new List<int>();
        foreach (var p in pos)
        {
            bool res = CastLine(p, dir, max);
            if (res == false)
            {
                unreachable.Add(i); //保存的这个i是地图的小格子 从0开始计数
            }
            i++;
        }
        return unreachable;
    }

    public  bool CastLine(Vector3 pos, Vector3 dir, float max)
    {
        RaycastHit hit;
        bool flag = false;
       // Debug.DrawLine(pos, dir);
        if (Physics.Raycast(pos, dir, out hit))
        {
            float depth = hit.point.y;
         //   Debug.Log(pos);
         //   Debug.Log(depth);
            //float depth = hit.collider.gameObject.transform.position.y;
            if (depth <= max)
            {
                flag = true;
            }
        }
        return flag;
    }

    void UpdateItemInfo()
    {
        foreach(var i in cm.itemlist)
        {
            if (i != null)
            i.posy = i.prefab.transform.position.y;
        }
    }


    public void Save()
    {
        // SceneView.onSceneGUIDelegate -= OnSceneGUI;
        UpdateItemInfo();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
