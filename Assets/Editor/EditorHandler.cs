using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public abstract class EditorHandler
{
    protected CustomMap cm;
    abstract public void DealWithEvent();
    abstract public void ShowAuxInfo();
    public EditorHandler(CustomMap map)
    {
        cm = map;
    }

    public void DefaultShow()
    {
        Vector3 mapsize = MapModifier.Instance.MapSize;
        if (MapSceneView.Instance.isShowingUnit == true)
            MapAux.DrawMapCellsDotted(cm.center, mapsize, cm.unitlength, Color.yellow);
        if (MapSceneView.Instance.isShowingGrid == true)
            MapAux.DrawMapCells(cm.center, mapsize / (cm.paintedgridlength / cm.unitlength), cm.paintedgridlength, Color.blue);
        if (MapSceneView.Instance.isShowingUnreachable == true)
        {
            foreach (var e in cm.unreachable)
            {
                Vector3 center = MapModifier.Instance.TranselateIndexToPostion(e);
                MapAux.DrawMapUnreachableArea(center, cm.unitlength, Color.red);
            }
            foreach(var e in MapModifier.Instance.UnreachableDontRecord)
            {
                Vector3 center = MapModifier.Instance.TranselateIndexToPostion(e);
                MapAux.DrawMapUnreachableArea(center, cm.unitlength, Color.red);
            }
        }
        if (MapSceneView.Instance.isShowingPointAndArea == true)
        {
            MapModifier.Instance.ShowAreaFreeMoveHandles();
            MapModifier.Instance.ShowPointFreeMoveHandles();
        }
        if (MapSceneView.Instance.isShowingBuilding == true)
        {
            MapModifier.Instance.ShowGameObjectIndexInfo();

        }
        
        if (MapSceneView.Instance.isShowingEdge == true)
        {
            MapModifier.Instance.WallHandles();
            MapModifier.Instance.DoorHandles();
        }
        MapModifier.Instance.SetObjectsActive(MapSceneView.Instance.isShowingBuilding);
    }
}


public class DefualtEditorHandler : EditorHandler
{
    public override void ShowAuxInfo()
    {
       
    }

    public override void DealWithEvent()
    {
        
    }

    public DefualtEditorHandler(CustomMap map) : base(map)
    { }
}


