using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class GameObjectEditorHandler : EditorHandler
{
    int selectedObjectIndex = 0;
    public override void ShowAuxInfo()
    {
        Handles.BeginGUI();
        var re = SceneView.lastActiveSceneView.position;
        GUILayout.BeginArea(new Rect(0, re.height - 100, re.width, 100), EditorStyles.objectFieldThumb);//EditorStyles.toolbarButton);
        {
            var objs = ResourceCenter.Instance.prefabObjects;
            var thumbs = ResourceCenter.Instance.thumbnails;
            GUIContent[] gc = new GUIContent[objs.Length];
            for (int i = 0; i < gc.Length; ++i)
            {
                gc[i] = new GUIContent(thumbs[i]);
            }
            selectedObjectIndex = GUILayout.SelectionGrid(
                selectedObjectIndex,
                gc,
                gc.Length,
               EditorStyles.objectFieldThumb,//EditorStyles.toolbarButton,
               GUILayout.Width(200),
               GUILayout.Height(100));
        }
        GUILayout.EndArea();
        Handles.EndGUI();
        Vector3 mapsize = MapModifier.Instance.MapSize;// / (cm.paintedgridlength / cm.unitlength);
        MapAux.DrawMapCellsDotted(cm.center, mapsize, cm.unitlength, Color.yellow);
        MapAux.DrawMapCells(cm.center, mapsize / (cm.paintedgridlength / cm.unitlength), cm.paintedgridlength, Color.blue);
        MapModifier.Instance.ShowGameObjectIndexInfo();
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
        Vector3 objectsize = MapModifier.Instance.CaculateGameObjectSize(selectedObjectIndex);
        int lefttopindex = MapModifier.Instance.CaculateIndexForPos(lefttopcenter);
        int biggridindex = MapModifier.Instance.CaculatePaintedGridFromUnitIndex(lefttopindex);
        int[] unitindexes = MapModifier.Instance.CaculateUnitIndexesOfGrid(biggridindex);
        int girdlefttopindex = unitindexes[0];
        Vector3 girdlefttopcenter = MapModifier.Instance.TranselateIndexToPostion(girdlefttopindex);
        Vector3 buildcenter = MapModifier.Instance.CaculateCreateGameObjectCenter(girdlefttopcenter, objectsize);

        MapAux.ShowLabel(new Vector3(lefttopcenter.x + cm.unitlength, lefttopcenter.y, lefttopcenter.z + cm.unitlength), biggridindex.ToString(), Color.black);

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
                    MapModifier.Instance.AddObject(girdlefttopindex, buildcenter, selectedObjectIndex);
                    MapModifier.Instance.AddNewItem(girdlefttopindex, objectsize, selectedObjectIndex);
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

}