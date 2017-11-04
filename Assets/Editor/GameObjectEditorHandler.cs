using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
public class GameObjectEditorHandler : EditorHandler
{
    Vector2 scrollPos;
    
    enum ObjectType
    {
        none = 0,
        normal = 1,
        wall,
        door,
    }
    ObjectType curSelectType = ObjectType.none;
    Vector2 origin;
    float maxlength = 50;
    object curInfo;
    Dictionary<int, int> record;
    int selectedObjectIndex = 0;
    GUIContent[] gc;
    public override void ShowAuxInfo()
    {
        Handles.BeginGUI();
        var re = SceneView.lastActiveSceneView.position;
        GUI.BeginGroup(new Rect(0, re.height -75, re.width, re.height));
        scrollPos = GUILayout.BeginScrollView( scrollPos, GUILayout.Width(re.width/4), GUILayout.Height(40));
        GUILayout.BeginHorizontal();
        int num = GetTableLength();
        selectedObjectIndex = GUILayout.SelectionGrid(
            selectedObjectIndex,
            gc,
            gc.Length,
           EditorStyles.toolbarButton);//,//EditorStyles.toolbarButton,
                                       // GUILayout.Width(200),
                                       //   GUILayout.Height(100));

        GUILayout.EndHorizontal();
        GUILayout.EndScrollView();
        GUI.EndGroup();
        //GUILayout.BeginArea(new Rect(0, re.height - 40, re.width, 40), EditorStyles.toolbarButton);//EditorStyles.toolbarButton);
        //{
        //    int num = GetTableLength();
        //    selectedObjectIndex = GUILayout.SelectionGrid(
        //        selectedObjectIndex,
        //        gc,
        //        gc.Length,
        //       EditorStyles.toolbarButton);//,//EditorStyles.toolbarButton,
        //      // GUILayout.Width(200),
        //    //   GUILayout.Height(100));
        //}
        //GUILayout.EndArea();
        Handles.EndGUI();
    }
    public override void DealWithEvent()
    {
        Event current = Event.current;
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        HandleUtility.AddDefaultControl(controlID);
        Vector3 collisionPos = MapModifier.Instance.CaculateCollisionPosFromGUIPoint(current.mousePosition);
        if (collisionPos.y == float.MaxValue)
            return;
        collisionPos = new Vector3(collisionPos.x, 0, collisionPos.z);
        Vector3 lefttopcenter = MapModifier.Instance.CaculateCellCenterByPos(collisionPos);
        Vector3 objectsize = CaculateSize(record[selectedObjectIndex]);// * cm.paintedgridlength;// MapModifier.Instance.CaculateGameObjectSize(selectedObjectIndex);
        if (objectsize == Vector3.zero)
            return;
        ObjectType curType = GetType(record[selectedObjectIndex]);
        if(curType == ObjectType.door || curType == ObjectType.wall)
        {
            objectsize.x = 1;// *cm.paintedgridlength;
        }
        int lefttopindex = MapModifier.Instance.CaculateIndexForPos(lefttopcenter);
        int biggridindex = MapModifier.Instance.CaculatePaintedGridFromUnitIndex(lefttopindex);
        int[] unitindexes = MapModifier.Instance.CaculateUnitIndexesOfGrid(biggridindex);
        int girdlefttopindex = unitindexes[0];
        Vector3 girdlefttopcenter = MapModifier.Instance.TranselateIndexToPostion(girdlefttopindex);
        Vector3 buildcenter = MapModifier.Instance.CaculateCreateGameObjectCenter(girdlefttopcenter, objectsize);
        MapAux.ShowLabel(new Vector3(lefttopcenter.x + cm.unitlength, lefttopcenter.y, lefttopcenter.z + cm.unitlength), biggridindex.ToString(), Color.black);
        MapAux.ShowLabel(new Vector3(lefttopcenter.x + cm.unitlength, lefttopcenter.y, lefttopcenter.z - cm.unitlength), girdlefttopcenter.ToString(), Color.black);
        var flag = MapModifier.Instance.CheckContainUnreachable(girdlefttopindex, objectsize);
        if (flag == true)
            MapAux.DrawLines(girdlefttopcenter, objectsize*cm.paintedgridlength, cm.unitlength, Color.red);
        else
            MapAux.DrawLines(girdlefttopcenter, objectsize * cm.paintedgridlength, cm.unitlength, Color.green);

        int curID = GUIUtility.hotControl;
        if (HandleRecorder.handleIDAndTarget.ContainsKey(curID))
        {
            ObjectType tmpType = ObjectType.none;
            curInfo = HandleRecorder.handleIDAndTarget[curID];
            if (curInfo.GetType() == typeof(CustomItemInfo))
            {
                tmpType = ObjectType.normal;
                origin = current.mousePosition;
            }
            else if (curInfo.GetType() == typeof(WallInfo))
            {
                tmpType = ObjectType.wall;
                origin = current.mousePosition;
            }
            else if (curInfo.GetType() == typeof(DoorInfo))
            {
                tmpType = ObjectType.door;
                origin = current.mousePosition;
            }
            else
            {
                tmpType = ObjectType.none;
            }

            if (tmpType != curSelectType)
            {
                curSelectType = tmpType;
            }
        }

        if (curSelectType != ObjectType.none)
        {
            float distance = Vector3.Distance(origin, current.mousePosition);
            if (distance < maxlength)
            {
                Handles.BeginGUI();
                //GUI.BeginGroup(new Rect(origin.x - 100, origin.y - 100, 100, 60));
                GUILayout.BeginArea(new Rect(origin.x -40, origin.y - 20, 100, 60));
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("左转90"))
                {
                    RotateObject(curInfo, curSelectType, -1);
                }
                if (GUILayout.Button("右转90"))
                {
                    RotateObject(curInfo, curSelectType, 1);
                }
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
                //GUI.EndGroup();
                Handles.EndGUI();
            }
            else
            {
                HandleUtility.AddDefaultControl(controlID);
                curSelectType = ObjectType.none;
            }
        }


        switch (current.type)
        {
            case EventType.mouseDown:
                if (current.button == 0 && (!flag))
                {   
                    switch(curType)
                    {
                        case ObjectType.normal:
                            CustomItemInfo info = MapModifier.Instance.AddNewItem(biggridindex, objectsize, 0, record[selectedObjectIndex]);
                            MapModifier.Instance.AddObject(girdlefttopindex, buildcenter, info);
                            
                            break;
                        case ObjectType.wall:
                            
                            WallInfo winfo = MapModifier.Instance.AddWallInfo(biggridindex,(int) objectsize.x, record[selectedObjectIndex]);
                            MapModifier.Instance.AddWall(biggridindex, buildcenter + new Vector3(0,0,-1*cm.paintedgridlength/2f),winfo);
                            break;
                        case ObjectType.door:
                            DoorInfo dinfo = MapModifier.Instance.AddDoorInfo(biggridindex, (int)objectsize.x, record[selectedObjectIndex]);
                            MapModifier.Instance.AddDoor(biggridindex, buildcenter + new Vector3(0, 0, -1 * cm.paintedgridlength / 2f), dinfo);
                            
                            break;
                    }

                    // MapModifier.Instance.AddObject(girdlefttopindex, buildcenter, selectedObjectIndex);
                    //MapModifier.Instance.AddNewItem(girdlefttopindex, objectsize, selectedObjectIndex);
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
    public GameObjectEditorHandler(CustomMap map) : base(map)
    { }



    void GetObjectInfo(int id)
    {


    }


    int GetTableLength()
    {
        DataPoolVariable tmp = DPM.Instance.BuildingsDP.Query("build");
        var tmp2 = DPM.GetArray(tmp);
        int num = 0;
        if (record == null)
            record = new Dictionary<int, int>();
        else
            return record.Count;
      //  DataPoolIterator it = tmp.begin();
      // DataPoolIterator itEnd = tmp.end();
      // for (var i = it;it != itEnd;++i)
        for (int i = 0;i<tmp2.Length;++i)
        {
            DataPoolVariable var = (tmp2[i]);//.ToVariable();
            int id =int.Parse( var.MemberOf("id").ToString());
            record.Add(num,id);
            ++num;
        }
        gc = new GUIContent[num];
        for (int i = 0; i < gc.Length; ++i)
        {
            string name = GetBuildName(record[i]);
            gc[i] = new GUIContent(name);
        }
        return num;
    }
    string GetBuildName(int id)
    {
        var tmp = DPM.FindByKey(DPM.Instance.BuildingsDP, "build", id);
        string name = tmp.MemberOf("name").ToString();
        return name;
    }

    ObjectType GetType(int index)
    {
        var tmp = DPM.FindByKey(DPM.Instance.BuildingsDP, "build", index);
        if (tmp == null)
            return ObjectType.none;
        int type = int.Parse(tmp.MemberOf("cat").ToString());
        ObjectType ot;
        switch(type)
        {
            case 2:
                ot = ObjectType.wall;
                break;
            case 3:
                ot = ObjectType.door;
                break;
            default:
                ot = ObjectType.normal;
                break;
        }
        return ot;
    }

    Vector3 CaculateSize(int index)
    {
        var tmp = DPM.FindByKey(DPM.Instance.BuildingsDP, "build", index);
        if (tmp == null)
            return Vector3.zero;
        int width = int.Parse(tmp.MemberOf( "space_long").ToString());
        int height = int.Parse(tmp.MemberOf("space_short").ToString());
        return new Vector3(width, 0, height);
    }

    void RotateObject(object info ,ObjectType ot,int dir)
    {
        Type objecttype;
        if(ot == ObjectType.wall)
        {
            //objecttype = typeof(WallInfo);
            int onegird = ((WallInfo)info).grids[0].onegrid;
            int another = ((WallInfo)info).grids[0].another;
            int curDir = GetDirIndex(onegird, another);
            RefreshGrids(((WallInfo)info).grids, onegird, (curDir + dir + 4) % 4);
            GameObject tar = MapModifier.Instance.infoAndGameObject[info];
            if (tar != null)
                tar.transform.Rotate(new Vector3(0, 180 +((curDir + dir + 4) % 4) * 90, 0));
        }
        else if(ot == ObjectType.door)
        {
           // objecttype = typeof(DoorInfo);
            int onegird = ((DoorInfo)info).grids[0].onegrid;
            int another = ((DoorInfo)info).grids[0].another;
            int curDir = GetDirIndex(onegird, another);
            RefreshGrids(((DoorInfo)info).grids, onegird, (curDir + dir + 4) % 4);
            GameObject tar = MapModifier.Instance.infoAndGameObject[info];
            if (tar != null)
                tar.transform.Rotate(new Vector3(0, 180 + ((curDir + dir + 4) % 4) * 90, 0));
        }
        else if(ot == ObjectType.normal)
        {
            //objecttype = typeof(CustomItemInfo);
            var tmp = DPM.FindByKey(DPM.Instance.BuildingsDP, "build", ((CustomItemInfo)info).tid);
            if (tmp == null)
                return ;
            int width = int.Parse(tmp.MemberOf("space_long").ToString());
            int height = int.Parse(tmp.MemberOf("space_short").ToString());
            int delta = GetNextIndexDelta(((CustomItemInfo)info).dir, dir);
            ((CustomItemInfo)info).lefttopsite += delta*(width - height);
            ((CustomItemInfo)info).dir = (((CustomItemInfo)info).dir + 4 +  dir) % 4;
            GameObject tar = MapModifier.Instance.infoAndGameObject[info];
            if(tar != null)
                tar.transform.Rotate(new Vector3(0,((CustomItemInfo)info).dir*90,0));
        }
     }

    int GetDirIndex(int one,int another)
    {
        int g = (int)Math.Round((cm.mapwidth / cm.paintedgridlength));//(int)(cm.mapwidth / cm.paintedgridlength);
        int delta = another - one;
        if (delta == (-1) * g)
            return 0;
        else if (delta == 1)
            return 1;
        else if (delta == g)
            return 2;
        else
            return 3;
    }

    void RefreshGrids(List<Grids> grids,int startone,int dir)
    {
        int another;
        int linenum = (int)Math.Round((cm.mapwidth / cm.paintedgridlength));//(int)cm.mapwidth / cm.paintedgridlength;
        int step ;
        switch(dir)
        {
            case 0:
                step = 1;
                another = startone - linenum;
                break;
            case 1:
                step = linenum;
                another = startone + 1;
                break;
            case 2:
                step = -1;
                another = startone + linenum;
                break;
            case 3:
                step = -linenum;
                another = startone -1;
                break;
            default:
                step = 1;
                another = startone - linenum;
                break;
        }

        for(int i =0;i<grids.Count;++i)
        {
            var tmp = grids[i];
            tmp.onegrid = startone + step*i;
            tmp.another = another + step*i;
            grids[i] = tmp;
        }
    }

    int GetNextIndexDelta(int curDir,int dir)
    {
        int delta;
        int g = (int)Math.Round((cm.mapwidth / cm.paintedgridlength));
        switch (curDir)
        {
            case 0:
                if (dir == -1)
                    delta = 0;
                else
                    delta = 1;
                    break;
            case 1:
                if (dir == -1)
                    delta = -1;
                else
                    delta = g -1;
                break;
            case 2:
                if (dir == -1)
                    delta = 1-g;
                else
                    delta = -g;
                break;
            case 3:
                if (dir == -1)
                    delta = g;
                else
                    delta = 0;
                break;
            default:
                if (dir == -1)
                    delta = 0;
                else
                    delta = 1;
                break;
        }
        return delta;
    }
}