using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class MapModifier
{
    CustomMap cm;
    Vector3 mapsize;

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
        int xlength = (int)size.x;
        int zlength = (int)size.z;
        Vector3 center = new Vector3();
        center.x = pos.x + (float)(xlength / 2f - 0.5) * cm.unitlength;
        center.y = pos.y;
        center.z = pos.z - (float)(zlength / 2f - 0.5) * cm.unitlength;
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
        int index = (row - 1) * cm.mapwidth / cm.unitlength + (rank - 1);
        Debug.Log(index + "----index");
        return index;
    }
    //根据左上索引和轮廓来看是否包含了不可达的位置
    public bool CheckContainUnreachable(int siteindex, Vector3 size)
    {
        int xlength = (int)size.x / cm.unitlength;
        int zlength = (int)size.z / cm.unitlength;
        int num = xlength * zlength;

        //格子越界
        if (siteindex % (cm.mapwidth / cm.unitlength) + xlength > (cm.mapwidth / cm.unitlength))
            return true;

        var unreachable = cm.unreachable;
        for (int i = 0; i < num; ++i)
        {
            int index = siteindex + (i / xlength) * cm.mapwidth / cm.unitlength + i % xlength;
            if (unreachable.Contains(index))
            {
                return true;
            }
        }
        return false;
    }
    
    void CreateGameObject(Vector3 center, int index)
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
    
    public void AddNewItem(int posindex, Vector3 size, int itemindex)
    {
        // int index = CaculateIndexForPos(collisionPos);
        int xlength = (int)size.x;
        int zlength = (int)size.z;
        int num = (int)size.x * (int)size.z;
        for (int i = 0; i < num; ++i)
        {
            int index = posindex + (i / xlength) * cm.mapwidth / cm.unitlength + i % xlength;
            cm.unreachable.Add(index);
        }
        CustomItemInfo newitem = new CustomItemInfo();
       // newitem.type = itemtype;
        newitem.lefttopsite = posindex;
        newitem.prefab = ResourceCenter.Instance.prefabObjects[itemindex];
        cm.itemlist.Add(newitem);
    }

    public void AddObject(int posindex, Vector3 center,int itemindex)
    {
        CreateGameObject(center, itemindex);
        Debug.Log("mouse click");
    }


    public void GenerateBaseData()
    {
        mapsize = new Vector3();
        mapsize.x = cm.mapwidth / cm.unitlength;
        mapsize.y = 0;
        mapsize.z = cm.mapheight / cm.unitlength;
        maplefttopcenter = new Vector3(cm.center.x - cm.mapwidth / 2.0f + cm.unitlength/2f, 0, cm.center.z + cm.mapheight / 2.0f - cm.unitlength/2f);
        if(cm.hasGeneratedData == false)
        {
            GenerateBaseUnreachableData();
            cm.hasGeneratedData = true;
        }
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
                Debug.Log(hit.point);
            }
        }
        return flag;
    }
    //仅仅根据模板地图生成不可达点
    void GenerateBaseUnreachableData()
    {
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < cm.mapwidth / cm.unitlength; ++i)
            for (int j = 0; j < cm.mapheight / cm.unitlength; ++j)
            {
                // Vector3 pos = new Vector3((float)(i - cm.mapwidth / 2 + 0.5) * cm.tilelength, 1000, (float)(j - cm.mapheight / 2 + 0.5) * cm.tilelength);
                Vector3 pos = new Vector3(cm.unitlength*i + maplefttopcenter.x, 1000, maplefttopcenter.z - cm.unitlength*j);
                positions.Add(pos);
            }
        // AssetDatabase.Refresh();
        cm.unreachable = Detect(positions, cm.dir, cm.max);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    //把地图的小格索引转成对应的格子中心位置
    public Vector3 TranselateIndexToPostion(int index)
    {
        int xlength = (int)cm.mapwidth / cm.unitlength;
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
    // 根据信息来创建物体，并且计算该物体新产生的不可达点，并添加进去
   public  void CreateGameObjectAndAddUnreachable(CustomItemInfo iteminfo)
    {
        int index = iteminfo.lefttopsite;
        Vector3 lefttop = new Vector3(cm.center.x - cm.mapwidth / 2.0f, 0, cm.center.z + cm.mapheight / 2.0f);
        int rank = index % (cm.mapwidth / cm.unitlength) + 1;//(int)Mathf.Ceil((pos.x - lefttop.x) / (float)cm.unitlength);
        int row = index / (cm.mapwidth / cm.unitlength) + 1;// (int)Mathf.Ceil(Mathf.Abs(pos.z - lefttop.z) / (float)cm.unitlength);
        // int index = (row - 1) * cm.mapwidth / cm.unitlength + (rank);
        var centerpos = new Vector3(rank * cm.unitlength + lefttop.x - cm.unitlength / 2.0f, 0, lefttop.z - row * cm.unitlength + cm.unitlength / 2.0f);
        GameObject objTarget;
        objTarget = ResourceCenter.Instance.objectDic[iteminfo.name];
        //if (objTarget)
        //objTarget.transform.position = centerpos;
        objTarget.transform.position = new Vector3(centerpos.x, iteminfo.posy, centerpos.z);
        cm.unreachable.Add(index);
        //return centerpos;
    }

    public int CaculatePaintedGridFromUnitIndex(int lefttopindex)
    {
        int scale = cm.paintedgridlength / cm.unitlength;
        int rank = lefttopindex % (cm.mapwidth / cm.unitlength) + 1;
        int row = lefttopindex / (cm.mapwidth / cm.unitlength) + 1;
        int gridrow = (int)Mathf.Ceil(row /(float) scale);
        int gridrank = (int)Mathf.Ceil(rank / (float)scale);
        int gridindex = (gridrow - 1) * cm.mapwidth / cm.paintedgridlength + (gridrank - 1);
        return gridindex;
    }

    public int CaculateLefttopUnitIndexOfGrid(int gridindex)
    {
        


        return 1;
    }

    public int[] CaculateUnitIndexesOfGrid(int gridindex)
    {
        int scale = cm.paintedgridlength / cm.unitlength;
        int[] indexes = new int[scale * scale];
        int rank = gridindex % (cm.mapwidth / cm.paintedgridlength) ;
        int row = gridindex / (cm.mapwidth / cm.paintedgridlength) ;

        int startnum = row * (cm.mapwidth / cm.paintedgridlength) * scale * scale + rank * scale;
        for(int i = 0;i<scale;++i)
            for(int j=0;j<scale;++j)
            {
                indexes[scale * i +  j] = startnum + (cm.mapwidth / cm.unitlength) * i + j;
            }
        return indexes;
    }

    public void AddPoint(Vector3 site)
    {
        NodeInfo tmp = new NodeInfo();
        tmp.site = site;
        if(!cm.designerNode.Contains(tmp))
        {
            cm.designerNode.Add(tmp);
        }
    }

    public void AddArea(Vector3 start,Vector3 end)
    {
        AreaInfo tmp = new AreaInfo();
        tmp.start = start;
        tmp.end = end;
        if (!cm.designerArea.Contains(tmp))
        {
            cm.designerArea.Add(tmp);
        }
    }

    public void ShowAreaFreeMoveHandles()
    {
        Handles.color = Color.green;
        foreach( var e in cm.designerArea)
        {
            e.start = Handles.FreeMoveHandle(e.start, Quaternion.identity, .5f, Vector3.zero, Handles.CubeHandleCap);
            e.end = Handles.FreeMoveHandle(e.end, Quaternion.identity, .5f, Vector3.zero, Handles.CubeHandleCap);
            MapAux.DrawRectHandles(e.start, e.end);
        }
    }

    public void ShowPointFreeMoveHandles()
    {
        Handles.color = Color.green;
        foreach (var e in cm.designerNode)
        {
            e.site = Handles.FreeMoveHandle(e.site, Quaternion.identity, .25f, Vector3.zero, Handles.SphereHandleCap);
            Handles.DrawWireDisc(e.site, Vector3.up, .5f);
        }
    }
}
