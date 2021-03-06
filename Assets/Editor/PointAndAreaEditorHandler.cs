﻿using System.Collections;
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
    int lastFocusID = 0;
    public override void ShowAuxInfo()
    {
       
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
        int curID = GUIUtility.hotControl;
        if (HandleRecorder.handleIDAndTarget.ContainsKey(curID))
        {
            curFocusID = curID;
        }
        if (HandleRecorder.handleIDAndTarget.ContainsKey(curFocusID))
        {
            // Handles.BeginGUI();
            // var re = SceneView.lastActiveSceneView.position;
            // GUILayout.BeginArea(new Rect(re.width / 4, re.height - 120, re.width/2, 120), EditorStyles.textArea);
            EditDetailsWindow.deleteAction = () =>
            {
                MapModifier.Instance.RemoveInfo(HandleRecorder.handleIDAndTarget[curFocusID]);
                HandleRecorder.handleIDAndTarget.Remove(curFocusID);
            };
            EditDetailsWindow.toDraw = ExposeProperties.GetProperties(HandleRecorder.handleIDAndTarget[curFocusID], out EditDetailsWindow.toDrawType);
            //ExposeProperties.Expose(m_fields);

            // GUILayout.EndArea();
            // Handles.EndGUI();
        }
        SceneView.RepaintAll();
    }
    public PointAndAreaEditorHandler(CustomMap map) : base(map)
    { }
}
