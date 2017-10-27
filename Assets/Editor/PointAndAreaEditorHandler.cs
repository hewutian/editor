using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class PointAndAreaEditorHandler : EditorHandler
{
    bool AdjustmentMode = false;
    Vector3 startpoint;
    Vector3 endpoint;
    bool isDrag = false;
    int curFocusID = 0;
    public override void ShowAuxInfo()
    {
        Vector3 mapsize = MapModifier.Instance.MapSize;
        MapAux.DrawMapCellsDotted(cm.center, mapsize, cm.unitlength, Color.yellow);
        MapAux.DrawMapCells(cm.center, mapsize / (cm.paintedgridlength / cm.unitlength), cm.paintedgridlength, Color.blue);
        foreach (var e in cm.unreachable)
        {
            Vector3 center = MapModifier.Instance.TranselateIndexToPostion(e);
            MapAux.DrawMapUnreachableArea(center, cm.unitlength, Color.red);
        }
        MapModifier.Instance.ShowAreaFreeMoveHandles();
        MapModifier.Instance.ShowPointFreeMoveHandles();
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(Screen.width - 100, Screen.height - 80, 90, 50));
        if (GUILayout.Button("绘制模式"))
            AdjustmentMode = false;
        if (GUILayout.Button("调节模式"))
            AdjustmentMode = true;
        GUILayout.EndArea();
        Handles.EndGUI();
    }

    public override void DealWithEvent()
    {
        Event current = Event.current;
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        Vector3 collisionPos = MapModifier.Instance.CaculateCollisionPosFromGUIPoint(current.mousePosition);
        if (collisionPos.y == float.MaxValue)
            return;
        collisionPos = new Vector3(collisionPos.x, 0, collisionPos.z);
        Vector3 lefttopcenter = MapModifier.Instance.CaculateCellCenterByPos(collisionPos);
        int lefttopindex = MapModifier.Instance.CaculateIndexForPos(lefttopcenter);
        Vector3 size = new Vector3(1, 0, 1);
        var flag = MapModifier.Instance.CheckContainUnreachable(lefttopindex, size);
        Handles.color = Color.green;
        Handles.DrawWireDisc(collisionPos, Vector3.up, .5f);
        //Debug.Log(current.type);
        if (isDrag == true)
        {
            MapAux.DrawRectHandles(startpoint, endpoint);
        }
        if (!AdjustmentMode)
        {
            HandleUtility.AddDefaultControl(controlID);
            switch (current.type)
            {
                case EventType.mouseDown:
                    if (current.button == 0 && (!isDrag))
                    {
                        startpoint = collisionPos;
                    }
                    break;
                case EventType.mouseUp:
                    if (current.button == 0 && (isDrag == false))
                    {
                        MapModifier.Instance.AddPoint(startpoint);
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
                    }
                    break;
                default:
                    break;
            }
        }
        else
       	{
            int curID = GUIUtility.hotControl;
            Debug.Log(curID);
            if (HandleRecorder.handleIDAndTarget.ContainsKey(curID))
            {
                curFocusID = curID;
            }
            if (HandleRecorder.handleIDAndTarget.ContainsKey(curFocusID))
            {
                Handles.BeginGUI();
                var re = SceneView.lastActiveSceneView.position;
                GUILayout.BeginArea(new Rect(re.width / 4, re.height - 120, re.width/2, 120), EditorStyles.textArea);
                var m_fields = ExposeProperties.GetProperties(HandleRecorder.handleIDAndTarget[curFocusID]);
                ExposeProperties.Expose(m_fields);
                if(GUILayout.Button("删除"))
                {
                    MapModifier.Instance.RemoveInfo(HandleRecorder.handleIDAndTarget[curFocusID]);
                    HandleRecorder.handleIDAndTarget.Remove(curFocusID);
                }
                GUILayout.EndArea();
                Handles.EndGUI();
            }
        }
        SceneView.RepaintAll();
    }
    public PointAndAreaEditorHandler(CustomMap map) : base(map)
    { }
}
