using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class EdgeObjectEditorHandler:EditorHandler
{

    Vector3 start;
    Vector3 end;
    bool isDrag = false;
    public override void ShowAuxInfo()
    {
        Vector3 mapsize = MapModifier.Instance.MapSize;
        MapAux.DrawMapCellsDotted(cm.center, mapsize, cm.unitlength, Color.yellow);
        MapAux.DrawMapCells(cm.center, mapsize / (cm.paintedgridlength / cm.unitlength), cm.paintedgridlength, Color.blue);

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
       // Vector3 objectsize = Vector3.zero;//MapModifier.Instance.CaculateGameObjectSize(selectedObjectIndex);
        int lefttopindex = MapModifier.Instance.CaculateIndexForPos(lefttopcenter);
        int biggridindex = MapModifier.Instance.CaculatePaintedGridFromUnitIndex(lefttopindex);
        int[] unitindexes = MapModifier.Instance.CaculateUnitIndexesOfGrid(biggridindex);
        int girdlefttopindex = unitindexes[0];
        Vector3 girdlefttopcenter = MapModifier.Instance.TranselateIndexToPostion(girdlefttopindex);
        Vector3[] fourCorner = new Vector3[4];
        fourCorner[0] = new Vector3(girdlefttopcenter.x  - cm.unitlength/2f, girdlefttopcenter.y, girdlefttopcenter.z + cm.unitlength/2f); //girdlefttopcenter ;
        fourCorner[1] =new Vector3(girdlefttopcenter.x + cm.paintedgridlength - cm.unitlength/2f, girdlefttopcenter.y, girdlefttopcenter.z + cm.unitlength / 2f);
        fourCorner[2] = new Vector3(girdlefttopcenter.x - cm.unitlength/2f, girdlefttopcenter.y, girdlefttopcenter.z - cm.paintedgridlength + cm.unitlength/2f);
        fourCorner[3] = new Vector3(girdlefttopcenter.x + cm.paintedgridlength - cm.unitlength/2f, girdlefttopcenter.y, girdlefttopcenter.z - cm.paintedgridlength + cm.unitlength/2f);
        float min = cm.paintedgridlength;
        int minid = 0;
        for(int i =0;i<4;++i)
        {
            float tmp = Vector3.Distance(fourCorner[i], collisionPos);
           // Debug.Log(tmp + "distance");
            if (tmp< min)
            {
                min = tmp;
                minid = i;
            }
        }
        //Debug.Log(minid + "nearest id");
        MapAux.ShowLabel(new Vector3(lefttopcenter.x + cm.unitlength, lefttopcenter.y, lefttopcenter.z + cm.unitlength), biggridindex.ToString(), Color.black);
        if(isDrag == true)
        {
            Handles.color = Color.green;
            Handles.DrawWireDisc(start, Vector3.up, .5f);
            Handles.DrawWireDisc(end, Vector3.up, .5f);
            Handles.color = new Color(128 / 255f, 0, 128 / 255f);
            Handles.DrawLine(start, end);
            Debug.Log(start + "start end" + end);
        }
        HandleUtility.AddDefaultControl(controlID);
        switch (current.type)
        {
            case EventType.mouseDown:
                if (current.button == 0 && (!isDrag))
                {
                    start = fourCorner[minid];
                   // Handles.color = Color.green;
                  //  Handles.DrawWireDisc(start, Vector3.up, .5f);
                }
                break;
            case EventType.mouseUp:
                if (current.button == 0 && (isDrag == false))
                {
                  //  MapModifier.Instance.AddPoint(startpoint);
                }
                else if (current.button == 0 && (isDrag == true))
                {
                    end = fourCorner[minid];
                    //MapModifier.Instance.AddArea(startpoint, endpoint);
                    isDrag = false;
                }
                break;
            case EventType.mouseDrag:
                if (current.button == 0)
                {
                    end = fourCorner[minid];
                    isDrag = true;
                   // Handles.DrawWireDisc(end, Vector3.up, .5f);
                   // Handles.color = new Color(128 / 255f, 0, 128 / 255f);
                   // Handles.DrawLine(start, end);
                }
                break;
            default:
                break;
        }
        SceneView.RepaintAll();
    }

    public EdgeObjectEditorHandler(CustomMap map) : base(map)
    { }
}
    