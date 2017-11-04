using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System;
public class MapModifier
{
    CustomMap cm;
    Vector3 mapsize;
    List<int> unreachableDontRecord = new List<int>();
    List<GameObject> buildings;
    public Dictionary<object, GameObject> infoAndGameObject = new Dictionary<object, GameObject>();
    bool isObjectsActive = false;
    public GameObject root;
    public List<int> UnreachableDontRecord
    {
        get
        {
            return unreachableDontRecord;
        }
    }
    //int curControlID;
    [SerializeField]
    Vector3 maplefttopcenter;
     static MapModifier instance;
    public static MapModifier Instance
    {
        get
        {
            if (instance == null)
                instance = new MapModifier();
            return instance;
        }
    }
    public CustomMap CurMap
    {
        get
        {
            return cm;
        }
    }
    public Vector3 MapSize
    {
        get
        {
            return mapsize;
        }
    }  
    public void SetCustomMap(CustomMap cmap)
    {
        cm = cmap;
        //allunreachable = cm.unreachable;
    }

    public void AddUnreachableIndex(int index)
    {
        if(!cm.unreachable.Contains(index))
        {
            cm.unreachable.Add(index);
        }
    }

    public void AddUnreachableIndexs(List<int> index)
    {

    }

    public void RemoveUnreachableIndex(int index)
    {
        if(cm.unreachable.Contains(index))
        {
            cm.unreachable.Remove(index);
        }
    }

    public void RemoveInfo(object info)
    {
        Type infotype = info.GetType();
        if(infotype == typeof(NodeInfo))
        {
            if(cm.designerNode.Contains((NodeInfo)info))
            {
                cm.designerNode.Remove((NodeInfo)info);
            }
        }
        else if(infotype == typeof(AreaInfo))
        {
            if (cm.designerArea.Contains((AreaInfo)info))
            {
                cm.designerArea.Remove((AreaInfo)info);
            }
        }
        else if(infotype == typeof(CustomItemInfo))
        {
            if (cm.itemlist.Contains((CustomItemInfo)info))
            {
                cm.itemlist.Remove((CustomItemInfo)info);
            }
        }
    }

    public void AddItem(CustomItemInfo iteminfo)
    { }

    public void RemoveItemInfo()
    { }
    //从gui上一点发射线求与世界物体碰撞的位置
   public Vector3 CaculateCollisionPosFromGUIPoint(Vector2 guipoint)
    {
        var mousepos = Event.current.mousePosition;
        Ray screenTo = HandleUtility.GUIPointToWorldRay(mousepos);// SceneView.lastActiveSceneView.camera.ScreenPointToRay(mousepos);
        RaycastHit hitinfo;
        if (Physics.Raycast(screenTo, out hitinfo))
            return hitinfo.point;
        else
            return new Vector3(0f, float.MaxValue, 0f);
    }

    public string GetCollisionNameFromGUIPoint(Vector2 guipoint)
    {
        var mousepos = Event.current.mousePosition;
        Ray screenTo = HandleUtility.GUIPointToWorldRay(mousepos);// SceneView.lastActiveSceneView.camera.ScreenPointToRay(mousepos);
        RaycastHit hitinfo;
        if (Physics.Raycast(screenTo, out hitinfo))
            return hitinfo.transform.name;
        else
            return "null";
    }
    //计算选择的模型的轮廓大小，主要看x,z
    public Vector3 CaculateGameObjectSize(int selected)
    {
        int xlength = 1;
        int ylength = 1;
        int zlength = 1;
        if (selected != null)
        {
            var obj = ResourceCenter.Instance.prefabObjects[selected];
            //var size = obj.GetComponent<Collider>().bounds.size;
            var size = obj.GetComponent<Renderer>().bounds.size;
            xlength = (int)Mathf.Ceil(size.x / cm.unitlength);
            zlength = (int)Mathf.Ceil(size.z / cm.unitlength);
            ylength = (int)Mathf.Ceil(size.y / cm.unitlength);
        }
        return new Vector3(xlength, ylength, zlength);
    }
    //根据左上的中心坐标和物体轮廓来求轮廓的中心，
    public Vector3 CaculateCreateGameObjectCenter(Vector3 pos, Vector3 size)
    {
        float xlength = ((int)size.x)*cm.paintedgridlength;
        float zlength = ((int)size.z) * cm.paintedgridlength;
        Vector3 center = new Vector3();
        center.x = pos.x + (float)xlength / 2f - 0.5f * cm.unitlength;
        center.y = pos.y;
        center.z = pos.z - (float)zlength / 2f + 0.5f * cm.unitlength;
        return center;
    }
    //根据位置来求对应的在地图中的小格子的中心坐标
    public Vector3 CaculateCellCenterByPos(Vector3 pos)
    {
        Vector3 lefttop = new Vector3(cm.center.x - cm.mapwidth / 2.0f, 0, cm.center.z + cm.mapheight / 2.0f);
        int rank = (int)Mathf.Ceil((pos.x - lefttop.x) / (float)cm.unitlength);
        int row = (int)Mathf.Ceil(Mathf.Abs(pos.z - lefttop.z) / (float)cm.unitlength);
        // int index = (row - 1) * cm.mapwidth / cm.unitlength + (rank);
        var indexUnitpos = new Vector3(rank * cm.unitlength + lefttop.x - cm.unitlength / 2.0f, pos.y, lefttop.z - row * cm.unitlength + cm.unitlength / 2.0f);
         Debug.Log(indexUnitpos + "-------indexUnitpos");
        return indexUnitpos;
    }
    //根据位置来求对应的在地图中的小格子索引
    public int CaculateIndexForPos(Vector3 pos)
    {
        Vector3 lefttop = new Vector3(cm.center.x - cm.mapwidth / 2.0f, 0, cm.center.z + cm.mapheight / 2.0f);
        int rank = (int)Mathf.Ceil((pos.x - lefttop.x) / (float)cm.unitlength);
        int row = (int)Mathf.Ceil(Mathf.Abs(pos.z - lefttop.z) / (float)cm.unitlength);
        int index = (row - 1) *(int)Math.Round(( cm.mapwidth / cm.unitlength)) + (rank - 1);
        // Debug.Log((rank - 1) + "----(rank - 1)");
        return index;
    }
    //根据左上索引和轮廓来看是否包含了不可达的位置
    public bool CheckContainUnreachable(int siteindex, Vector3 size)
    {
        int xlength = (int)Math.Round((size.x / cm.unitlength));
        int zlength = (int)Math.Round((size.z / cm.unitlength));
        int num = xlength * zlength;
        //格子越界
        if (siteindex % (int)Math.Round((cm.mapwidth / cm.unitlength)) + xlength > (int)Math.Round((cm.mapwidth / cm.unitlength)))
            return true;
        //var unreachable = cm.unreachable;
        for (int i = 0; i < num; ++i)
        {
            int index = siteindex + (i / xlength) * (int)Math.Round((cm.mapwidth / cm.unitlength)) + i % xlength;
            if (cm.unreachable.Contains(index))
            {
                return true;
            }
        }
        return false;
    }
    
   public  void CreateGameObject(Vector3 center, int index)
    {
        GameObject objTarget;
        objTarget = GameObject.Instantiate(ResourceCenter.Instance.prefabObjects[index]);
        if (objTarget)
        {
            objTarget.transform.position = center;
            objTarget.transform.parent = GameObject.FindObjectOfType<SceneMark>().gameObject.transform;
            if (objTarget.GetComponent<ItemMark>() == null)
            {
                ItemMark itemMark = objTarget.AddComponent<ItemMark>();
                itemMark.sceneMark = GameObject.FindObjectOfType<SceneMark>();
            }
        }
    }
    
    public CustomItemInfo AddNewItem(int posindex, Vector3 size,int dir , int tid)
    {
        CustomItemInfo newitem = new CustomItemInfo();
        newitem.dir = dir;
        newitem.lefttopsite = posindex;
        newitem.tid = tid;
        cm.itemlist.Add(newitem);
        return newitem;
    }

    public void AddObject(int posindex, Vector3 center,CustomItemInfo info)
    {
        var tmp = DPM.FindByKey(DPM.Instance.BuildingsDP, "build", info.tid);
        if (tmp == null)
            return;
        int type = int.Parse(tmp.MemberOf("cat").ToString());
        GameObject target = ResourceCenter.Instance.GetPrefabInstance(type);
        if (target == null)
            return;
       // int[] unitindexes = MapModifier.Instance.CaculateUnitIndexesOfGrid(info.lefttopsite);
      //  int girdlefttopindex = unitindexes[0];
      //  Vector3 girdlefttopcenter = MapModifier.Instance.TranselateIndexToPostion(girdlefttopindex);
        int width = int.Parse(tmp.MemberOf("space_long").ToString());
        int height = int.Parse(tmp.MemberOf("space_short").ToString());
        Vector3 objectsize;
        objectsize = new Vector3(width, 0, height);
        target.transform.position = center;
        {
            target.transform.parent = root.transform;
            if (target.GetComponent<ItemMark>() == null)
            {
                ItemMark itemMark = target.AddComponent<ItemMark>();
                itemMark.sceneMark = GameObject.FindObjectOfType<SceneMark>();
            }
        }
        buildings.Add(target);
        CheckInfoAndObject(info, target);
    }

    public  void AddObject(CustomItemInfo iteminfo)// da ge index
    {
        int index = iteminfo.lefttopsite;
        Vector3 lefttop = new Vector3(cm.center.x - cm.mapwidth / 2.0f, 0, cm.center.z + cm.mapheight / 2.0f);
        int rank = index % (int)Math.Round((cm.mapwidth /(float) cm.paintedgridlength)) + 1;//(int)Mathf.Ceil((pos.x - lefttop.x) / (float)cm.unitlength);
        int row = index / (int)Math.Round((cm.mapwidth / (float)cm.paintedgridlength)) + 1;// (int)Mathf.Ceil(Mathf.Abs(pos.z - lefttop.z) / (float)cm.unitlength);
        // int index = (row - 1) * cm.mapwidth / cm.unitlength + (rank);
        var lefttopcenterpos = new Vector3(rank * cm.paintedgridlength + lefttop.x - cm.paintedgridlength / 2.0f, 0, lefttop.z - row * cm.paintedgridlength + cm.paintedgridlength / 2.0f);
        Vector3 start, end;
        string name;
        var tmp = DPM.FindByKey(DPM.Instance.BuildingsDP, "build", iteminfo.tid);
        if (tmp == null)
            return;
        name = tmp.MemberOf("name").ToString();
        int width = int.Parse(tmp.MemberOf("space_long").ToString());// * cm.paintedgridlength;
        int height = int.Parse(tmp.MemberOf("space_short").ToString());//* cm.paintedgridlength;
        if (width == 0)
            return;
        int step;
        int[] startIndexes = new int[(int)width];
        int g = (int)Math.Round((cm.mapwidth / cm.paintedgridlength));//(int)(cm.mapwidth / cm.paintedgridlength);
        if (iteminfo.dir == 0)
        {
            start = new Vector3(lefttopcenterpos.x - cm.paintedgridlength / 2f, 0, lefttopcenterpos.z + cm.paintedgridlength / 2f);
            end = new Vector3(start.x + width * cm.paintedgridlength, 0, start.z - height * cm.paintedgridlength);
             step = 1;
            for(int i =0;i<width;++i)
            {
                startIndexes[i] = iteminfo.lefttopsite + i*g;
            }
        }
        else if(iteminfo.dir == 1)
        {
            start = new Vector3(lefttopcenterpos.x - cm.paintedgridlength / 2f, 0, lefttopcenterpos.z + cm.paintedgridlength / 2f);
            end = new Vector3(start.x +height * cm.paintedgridlength, 0, start.z - width * cm.paintedgridlength);
             step = (int)Math.Round((cm.mapwidth / cm.paintedgridlength)); //(int)cm.mapwidth / cm.paintedgridlength;
            for (int i = 0; i < width; ++i)
            {
                startIndexes[i] = iteminfo.lefttopsite + i *(-1);
            }
        }
        else if(iteminfo.dir == 2)
        {
            start = new Vector3(lefttopcenterpos.x + cm.paintedgridlength / 2f, 0, lefttopcenterpos.z - cm.paintedgridlength / 2f);
            end = new Vector3(start.x - height * cm.paintedgridlength, 0, start.z + width * cm.paintedgridlength);
             step = -1;
            for (int i = 0; i < width; ++i)
            {
                startIndexes[i] = iteminfo.lefttopsite + i * (-g);
            }
        }
        else
        {
            start = new Vector3(lefttopcenterpos.x + cm.paintedgridlength / 2f, 0, lefttopcenterpos.z - cm.paintedgridlength / 2f);
            end = new Vector3(start.x - width * cm.paintedgridlength, 0, start.z +height * cm.paintedgridlength);
             step = -1 * (int)Math.Round((cm.mapwidth / cm.paintedgridlength)); //(int)cm.mapwidth / cm.paintedgridlength;
            for (int i = 0; i < width; ++i)
            {
                startIndexes[i] = iteminfo.lefttopsite + i * (1);
            }
        }
        for (int i =0;i< startIndexes.Length;++i)
        {
            for(int j=0;j<height;++j)
            {
                int curindex = startIndexes[i] + j * step;
                int after = (i * (int)width + j);//.ToString();
                // int masknum = int.Parse(tmp.MemberOf("col").ToString());
                DataPoolVariable dpv = tmp.MemberOf("col");
                var masknums = DPM.GetArray(dpv);
                int masknum = int.Parse(masknums[after].ToString());
                var indexes = GetUnreachableUnitIndex(curindex, masknum);
                unreachableDontRecord.AddRange(indexes);
            }
        }
        MapAux.DrawRectHandles(start, end);
        Handles.color = Color.green;
        Handles.CapFunction customfunc = Handles.SphereHandleCap;
        customfunc += HandleRecorder.RecordHandles;
        Vector3 center = (start + end) / 2f;
        center = Handles.FreeMoveHandle(center, Quaternion.identity, .25f, Vector3.zero, customfunc);
        HandleRecorder.CheckID(iteminfo);
        var newstyle = new GUIStyle();
        newstyle.fontSize = 10;
        Handles.Label((start + end) / 2, name, newstyle);
    }

    public void GenerateBaseData()
    {
        ResourceCenter.Instance.Init("Assets/prefab/");
        mapsize = new Vector3();
        mapsize.x = (int)Math.Round((cm.mapwidth / cm.unitlength));
        mapsize.y = 0;
        mapsize.z = (int)Math.Round((cm.mapheight / cm.unitlength));//cm.mapheight / cm.unitlength;
        maplefttopcenter = new Vector3(cm.center.x - cm.mapwidth / 2.0f + cm.unitlength/2f, 0, cm.center.z + cm.mapheight / 2.0f - cm.unitlength/2f);
        if (cm.hasGeneratedData == false)
        {
            GenerateBaseUnreachableData();
            cm.hasGeneratedData = true;
        }
        HandleRecorder.handleIDAndTarget.Clear();

    }
    //检测不可达点
    List<int> Detect(List<Vector3> pos, Vector3 dir, float max)
    {
        int i = 0;
        List<int> unreachable = new List<int>();
        foreach (var p in pos)
        {
            bool res = CastLine(p, dir, max);
            if (res == false)
            {
                
                int num = CaculateIndexForPos(new Vector3(p.x,0,p.z));
                unreachable.Add(num); //保存的这个i是地图的小格子 从0开始计数
            }
            i++;
        }
        return unreachable;
    }

    bool CastLine(Vector3 pos, Vector3 dir, float max)
    {
        RaycastHit hit;
        bool flag = false;
        if (Physics.Raycast(pos, dir, out hit))
        {
            float depth = hit.point.y;
            if (depth <= max)
            {
                flag = true;
            }
            else
            {
               // Debug.Log(hit.point);
            }
        }
        return flag;
    }
    //仅仅根据模板地图生成不可达点
    void GenerateBaseUnreachableData()
    {
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < (int)Math.Round((cm.mapwidth / cm.unitlength)); ++i)
            for (int j = 0; j < (int)Math.Round((cm.mapheight / cm.unitlength)); ++j)
            {
                Vector3 pos = new Vector3(cm.unitlength*i + maplefttopcenter.x, 1000, maplefttopcenter.z - cm.unitlength*j);
                positions.Add(pos);
            }
        cm.unreachable = Detect(positions, cm.dir, cm.max);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    //把地图的小格索引转成对应的格子中心位置
    public Vector3 TranselateIndexToPostion(int index)
    {
        int xlength = (int)Math.Round((cm.mapwidth / cm.unitlength));
        //int zlength = (int)cm.mapheight / cm.unitlength;
        int xdelta = index % xlength;
        int zdelta = index / xlength;
        Vector3 pos = new Vector3(maplefttopcenter.x + xdelta * cm.unitlength, 0, maplefttopcenter.z - zdelta * cm.unitlength);
        return pos;
    }
    //主要是更新物体的y值
      void UpdateItemInfo()
    {
        foreach (var i in cm.itemlist)
        {
        }
    }

    public void Save()
    {
        UpdateItemInfo();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    // 根据信息来创建物体，并且计算该物体新产生的不可达点，并添加进去
   public  void CreateGameObjectAndAddUnreachable(CustomItemInfo iteminfo)
    {
        int index = iteminfo.lefttopsite;
        Vector3 lefttop = new Vector3(cm.center.x - cm.mapwidth / 2.0f, 0, cm.center.z + cm.mapheight / 2.0f);
        int rank = index % (int)Math.Round((cm.mapwidth / cm.unitlength)) + 1;//(int)Mathf.Ceil((pos.x - lefttop.x) / (float)cm.unitlength);
        int row = index / (int)Math.Round((cm.mapwidth / cm.unitlength)) + 1;// (int)Mathf.Ceil(Mathf.Abs(pos.z - lefttop.z) / (float)cm.unitlength);
        // int index = (row - 1) * cm.mapwidth / cm.unitlength + (rank);
        var lefttopcenterpos = new Vector3(rank * cm.unitlength + lefttop.x - cm.unitlength / 2.0f, 0, lefttop.z - row * cm.unitlength + cm.unitlength / 2.0f);
    }

    public int CaculatePaintedGridFromUnitIndex(int lefttopindex)
    {
        int scale = (int)Math.Round((cm.paintedgridlength / cm.unitlength));
        int rank = lefttopindex % (int)Math.Round((cm.mapwidth / cm.unitlength)) + 1;
        int row = lefttopindex / (int)Math.Round((cm.mapwidth / cm.unitlength)) + 1;
        int gridrow = (int)Mathf.Ceil(row /(float) scale);
        int gridrank = (int)Mathf.Ceil(rank / (float)scale);
        int gridindex = (gridrow - 1) * (int)Math.Round((cm.mapwidth / cm.paintedgridlength))  + (gridrank - 1);
        return gridindex;
    }

    public Vector3 CaculatePaintedGridCenter(int index)
    {
        int xlength = (int)Math.Round((cm.mapwidth / cm.paintedgridlength));
        //int zlength = (int)cm.mapheight / cm.unitlength;
        int xdelta = index % xlength;
        int zdelta = index / xlength; 
        Vector3 maplefttop = new Vector3(cm.center.x - cm.mapwidth / 2.0f , 0, cm.center.z + cm.mapheight / 2.0f );
        Vector3 pos = new Vector3(maplefttop.x + xdelta * cm.paintedgridlength + cm.paintedgridlength/2f, 0, maplefttop.z - zdelta * cm.paintedgridlength - cm.paintedgridlength/2f);
        return pos;
    }

    public int CaculateLefttopUnitIndexOfGrid(int gridindex)
    {
        return 1;
    }

    public int[] CaculateUnitIndexesOfGrid(int gridindex)
    {
        int scale = (int)Math.Round((cm.paintedgridlength / cm.unitlength));
        int[] indexes = new int[scale * scale];
        int rank = gridindex % ((int)Math.Round((cm.mapwidth / cm.paintedgridlength))) ;
        int row = gridindex / ((int)Math.Round((cm.mapwidth / cm.paintedgridlength))) ;

        int startnum = row * ((int)Math.Round((cm.mapwidth / cm.paintedgridlength))) * scale * scale + rank * scale;
        for(int i = 0;i<scale;++i)
            for(int j=0;j<scale;++j)
            {
                indexes[scale * i +  j] = startnum + (int)Math.Round((cm.mapwidth / cm.unitlength)) * i + j;
            }
        return indexes;
    }

    public void AddPoint(Vector3 site)
    {
        NodeInfo tmp = new NodeInfo();
        tmp.site = new Vector2(site.x,site.z);
        if(!cm.designerNode.Contains(tmp))
        {
            int idnext;
            if (cm.designerNode.Count > 0)
                idnext = cm.designerNode[cm.designerNode.Count - 1].id + 1;
            else
                idnext = 0;
            tmp.id = idnext;
            cm.designerNode.Add(tmp);
        }
    }

    public void AddArea(Vector3 start,Vector3 end)
    {
        AreaInfo tmp = new AreaInfo();
        tmp.start = new Vector2(start.x, start.z);
        tmp.end = new Vector2(end.x, end.z);
        if (!cm.designerArea.Contains(tmp))
        {
            int idnext;
            if (cm.designerArea.Count > 0)
                idnext = cm.designerArea[cm.designerArea.Count - 1].id + 1;
            else
                idnext = 0;
            tmp.id = idnext;
            cm.designerArea.Add(tmp);
        }
    }

    public void ShowAreaFreeMoveHandles()
    {
        foreach( var e in cm.designerArea)
        {
            Handles.color = Color.green;
            Handles.CapFunction customfunc = Handles.CubeHandleCap;
            customfunc += HandleRecorder.RecordHandles;
            Vector3 tmp = new Vector3(e.start.x, 0, e.start.y);
            tmp = Handles.FreeMoveHandle(tmp, Quaternion.identity, .5f, Vector3.zero, customfunc);
            e.start = new Vector2(tmp.x, tmp.z);
            HandleRecorder.CheckID(e);
            Vector3 tmp2 = new Vector3(e.end.x, 0, e.end.y);
            tmp2 = Handles.FreeMoveHandle(tmp2, Quaternion.identity, .5f, Vector3.zero, customfunc);
            e.end = new Vector2(tmp2.x, tmp2.z);
            HandleRecorder.CheckID(e);
            MapAux.DrawRectHandles(tmp, tmp2);
            var newstyle = new GUIStyle();
            newstyle.fontSize = 10;
             Handles.Label((tmp + tmp2) / 2, e.id.ToString(),newstyle);
        }
    }

    public void ShowPointFreeMoveHandles()
    {
        foreach (var e in cm.designerNode)
        {
            Handles.color = Color.green;
            Handles.CapFunction customfunc = Handles.SphereHandleCap;
            customfunc += HandleRecorder.RecordHandles;
            Vector3 tmp = new Vector3(e.site.x,0, e.site.y);
            tmp = Handles.FreeMoveHandle(tmp, Quaternion.identity, .25f, Vector3.zero, customfunc);
            e.site = new Vector2(tmp.x, tmp.z);
            HandleRecorder.CheckID(e);
            Handles.DrawWireDisc(tmp, Vector3.up, .5f);
            var newstyle = new GUIStyle();
            newstyle.fontSize = 10;
            Handles.Label(tmp, e.id.ToString(),newstyle);
        }
    }
    public void WallHandles()
    {
        foreach (var e in cm.walllist)
        {
            Handles.color = Color.green;
            Handles.CapFunction customfunc = Handles.ConeHandleCap;
            customfunc += HandleRecorder.RecordHandles;
            bool isHorizon = false;
            int gridlefttopindex = e.grids[0].onegrid;
            if(Mathf.Abs((e.grids[0].another - e.grids[0].onegrid)) == (int)Math.Round((cm.mapwidth / cm.paintedgridlength)))
            {
                isHorizon = true;
            }
            Vector3 start, end;
            Vector3 record;
            if(isHorizon == true)
            {

                int[] unitindexes = MapModifier.Instance.CaculateUnitIndexesOfGrid(gridlefttopindex);
                int targetIndex = unitindexes[0];
                Vector3 lefttopcenter = MapModifier.Instance.TranselateIndexToPostion(targetIndex);
                start = new Vector3(lefttopcenter.x - cm.unitlength / 2f, 0, lefttopcenter.z + cm.unitlength/2f);
                end = start + new Vector3(e.grids.Count * cm.paintedgridlength, 0, 0);
                record = lefttopcenter;
            }
            else
            {
                int[] unitindexes = MapModifier.Instance.CaculateUnitIndexesOfGrid(gridlefttopindex);
                int scale = (int)Math.Round((cm.paintedgridlength / cm.unitlength));//(int) (cm.paintedgridlength / cm.unitlength);
                int targetIndex = unitindexes[scale - 1];
                Vector3 lefttopcenter = MapModifier.Instance.TranselateIndexToPostion(targetIndex);
                start = new Vector3(lefttopcenter.x + cm.unitlength / 2f, 0, lefttopcenter.z + cm.unitlength/2f);
                end = start + new Vector3(0, 0, -1*e.grids.Count * cm.paintedgridlength);
                record = lefttopcenter;
            }
            Handles.FreeMoveHandle(start, Quaternion.identity, .5f, Vector3.zero, customfunc);
            MapAux.ShowLabel(new Vector3(start.x + cm.unitlength , start.y, start.z + cm.unitlength*2), start.ToString(), Color.black);
            MapAux.ShowLabel(new Vector3(start.x + cm.unitlength , start.y, start.z - cm.unitlength*2), record.ToString(), Color.black);
            HandleRecorder.CheckID(e);
            Handles.FreeMoveHandle(end, Quaternion.identity, .5f, Vector3.zero, customfunc);
            HandleRecorder.CheckID(e);
            Handles.color = new Color(128 / 255f, 0, 128 / 255f);
            Handles.DrawLine(start, end);
            var newstyle = new GUIStyle();
            newstyle.fontSize = 10;
            Handles.Label((start + end) / 2, e.tid.ToString(), newstyle);
        }
    }

    public void DoorHandles()
    {
        foreach (var e in cm.doorlist)
        {
            Handles.color = Color.green;
            Handles.CapFunction customfunc = Handles.ConeHandleCap;
            customfunc += HandleRecorder.RecordHandles;
            bool isHorizon = false;
            //  Vector3 tmp = new Vector3(e.site.x, 0, e.site.y);
            int gridlefttopindex = e.grids[0].onegrid;
            if (Mathf.Abs((e.grids[0].another - e.grids[0].onegrid)) == (int)Math.Round((cm.mapwidth / cm.paintedgridlength)))
            {
                isHorizon = true;
            }
            Vector3 start, end;
            if (isHorizon == true)
            {

                int[] unitindexes = MapModifier.Instance.CaculateUnitIndexesOfGrid(gridlefttopindex);
                int targetIndex = unitindexes[0];
                Vector3 lefttopcenter = MapModifier.Instance.TranselateIndexToPostion(targetIndex);
                start = new Vector3(lefttopcenter.x - cm.unitlength / 2f, 0, lefttopcenter.z + cm.unitlength/2f);
                end = start + new Vector3(e.grids.Count * cm.paintedgridlength, 0, 0);
            }
            else
            {
                int[] unitindexes = MapModifier.Instance.CaculateUnitIndexesOfGrid(gridlefttopindex);
                int scale = (int)Math.Round((cm.paintedgridlength / cm.unitlength));//(int)(cm.paintedgridlength / cm.unitlength);
                int targetIndex = unitindexes[scale - 1];
                Vector3 lefttopcenter = MapModifier.Instance.TranselateIndexToPostion(targetIndex);
                start = new Vector3(lefttopcenter.x - cm.unitlength / 2f, 0, lefttopcenter.z + cm.unitlength/2f);
                end = start + new Vector3(e.grids.Count * cm.paintedgridlength, 0, 0);
            }
            Handles.FreeMoveHandle(start, Quaternion.identity, .5f, Vector3.zero, customfunc);
            HandleRecorder.CheckID(e);
           Handles.FreeMoveHandle(end, Quaternion.identity, .5f, Vector3.zero, customfunc);
            HandleRecorder.CheckID(e);
            Handles.color = new Color(128 / 255f, 0, 128 / 255f);
            Handles.DrawLine(start, end);
            var newstyle = new GUIStyle();
            newstyle.fontSize = 10;
            Handles.Label((start + end) / 2, e.tid.ToString(), newstyle);
        }

    }

    public void ShowGameObjectIndexInfo()
    {
        unreachableDontRecord.Clear();
        foreach(var e in cm.itemlist)
        {
            AddObject(e);
        }
  
    }

    public void AddWall(int gridindex, Vector3 center,WallInfo winfo)
    {

        var tmp = DPM.FindByKey(DPM.Instance.BuildingsDP, "build", winfo.tid);
        if (tmp == null)
            return;
        int type = int.Parse(tmp.MemberOf("cat").ToString());
        GameObject target = ResourceCenter.Instance.GetPrefabInstance(type);
        if (target == null)
            return;
        target.transform.position = center;
        target.transform.Rotate( new Vector3(0, 180, 0));
        {
            target.transform.parent = root.transform;
            if (target.GetComponent<ItemMark>() == null)
            {
                ItemMark itemMark = target.AddComponent<ItemMark>();
                itemMark.sceneMark = GameObject.FindObjectOfType<SceneMark>();
            }
        }
        buildings.Add(target);
        CheckInfoAndObject(winfo, target);
    }
    
    public WallInfo AddWallInfo(int lefttopindex, int length, int tid)
    {
        int curIndex = lefttopindex;
        WallInfo wall = new WallInfo();
        wall.grids = new List<Grids>();
        wall.tid = tid;
        for (int i =0;i<length;++i)
        {
            Grids grids = new Grids();
            grids.onegrid = curIndex + i;
            grids.another = grids.onegrid - ((int)Math.Round((cm.mapwidth / cm.paintedgridlength)));
            wall.grids.Add(grids);
        }
        if(!cm.walllist.Contains(wall))
        {
            cm.walllist.Add(wall);
        }
        return wall;
     }

    public void AddDoor(int gridindex, Vector3 center, DoorInfo dinfo)
    {

        var tmp = DPM.FindByKey(DPM.Instance.BuildingsDP, "build", dinfo.tid);
        if (tmp == null)
            return;
        int type = int.Parse(tmp.MemberOf("cat").ToString());
        GameObject target = ResourceCenter.Instance.GetPrefabInstance(type);
        if (target == null)
            return;
        target.transform.position = center;
        target.transform.Rotate(new Vector3(0, 180, 0));
        {
            target.transform.parent = root.transform;
            if (target.GetComponent<ItemMark>() == null)
            {
                ItemMark itemMark = target.AddComponent<ItemMark>();
                itemMark.sceneMark = GameObject.FindObjectOfType<SceneMark>();
            }
        }
        buildings.Add(target);
        CheckInfoAndObject(dinfo, target);
    }

    public DoorInfo AddDoorInfo(int lefttopindex, int length, int tid)
    {
        int curIndex = lefttopindex;
        DoorInfo door = new DoorInfo();
        door.openstate = 0;
        door.grids = new List<Grids>();
        door.tid = tid;
        for (int i = 0; i < length; ++i)
        {
            Grids grids = new Grids();
            grids.onegrid = curIndex + i;
            grids.another = grids.onegrid - (int)Math.Round((cm.mapwidth / cm.paintedgridlength));
            door.grids.Add(grids);
        }
        if (!cm.doorlist.Contains(door))
        {
            cm.doorlist.Add(door);
        }
        return door;
    }

    List<int> GetUnreachableUnitIndex(int gridindex, int masknum)
    {
        List<int> indexes = new List<int>();
        if (masknum > 0)
        {
            int[] unitindexes = MapModifier.Instance.CaculateUnitIndexesOfGrid(gridindex);
            int scale = (int)Math.Round((cm.paintedgridlength / cm.unitlength));
            for (int i = 0; i < scale * scale; ++i)
            {
                int op = 1 << i;
                bool flag = (op & masknum) == op ? true : false;
                if (flag == true)
                    indexes.Add(unitindexes[i]);
            }
        }
        return indexes;
    }

    void InstantiateObject(CustomItemInfo info)
    {
        var tmp = DPM.FindByKey(DPM.Instance.BuildingsDP, "build", info.tid);
        if (tmp == null)
            return ;
        int type = int.Parse(tmp.MemberOf("cat").ToString());
        GameObject target = ResourceCenter.Instance.GetPrefabInstance(type);
        if (target == null)
            return;
        int[] unitindexes = MapModifier.Instance.CaculateUnitIndexesOfGrid(info.lefttopsite);
        int girdlefttopindex = unitindexes[0];
        Vector3 girdlefttopcenter = MapModifier.Instance.TranselateIndexToPostion(girdlefttopindex);
        int width = int.Parse(tmp.MemberOf("space_long").ToString());
        int height = int.Parse(tmp.MemberOf("space_short").ToString());
        Vector3 objectsize;
        if(info.dir == 0)
        {
            objectsize = new Vector3(width,0,height);
        }
        else if(info.dir == 1)
        {
            objectsize = new Vector3(height, 0, width);
        }
        else if(info.dir == 2)
        {
            objectsize = new Vector3(width, 0, height);
        }
        else//(info.dir == 3)
        {
            objectsize = new Vector3(height, 0, width);

        }
        Vector3 buildcenter = MapModifier.Instance.CaculateCreateGameObjectCenter(girdlefttopcenter, objectsize);
        target.transform.position = buildcenter;
        {
            target.transform.parent = root.transform;
            if (target.GetComponent<ItemMark>() == null)
            {
                ItemMark itemMark = target.AddComponent<ItemMark>();
                itemMark.sceneMark = GameObject.FindObjectOfType<SceneMark>();
            }
        }
        buildings.Add(target);
        CheckInfoAndObject(info, target);
    }

    void InstantiateWallObject(WallInfo info)
    {

    }

    void InstantiateDoorObject(DoorInfo info)
    {
      
    }


    public void CreateObjectsAtFirst()
    {
        buildings = new List<GameObject>();
        foreach (var e in cm.itemlist)
        {
            InstantiateObject(e);
        }
        foreach (var e in cm.walllist)
        {
            InstantiateWallObject(e);
        }
        foreach (var e in cm.doorlist)
        {
            InstantiateDoorObject(e);
        }
    }

    public void SetObjectsActive(bool act)
    {
        if (isObjectsActive != act)
        {
            foreach (var e in buildings)
            {
                e.SetActive(act);
            }
            isObjectsActive = act;
        }
    }


     void CheckInfoAndObject(object info,GameObject go)
    {
        if (!infoAndGameObject.ContainsKey(info))
        {
            infoAndGameObject.Add(info, go);
        }
        else if (infoAndGameObject[info] != go)
        {
            infoAndGameObject[info] = go;
        }
    }

}
