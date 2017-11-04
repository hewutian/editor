using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CellEditorHandler : EditorHandler
{
   // int cellIndex;
  //  int bigcellIndex;
    public override void ShowAuxInfo()
    {
        
      
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
        int lefttopindex = MapModifier.Instance.CaculateIndexForPos(lefttopcenter);
        string name = MapModifier.Instance.GetCollisionNameFromGUIPoint(current.mousePosition);
        MapAux.ShowLabel(new Vector3(lefttopcenter.x + cm.unitlength, lefttopcenter.y,lefttopcenter.z + cm.unitlength), lefttopindex.ToString() + ",name:" + name, Color.black);

        Vector3 size = new Vector3(cm.unitlength, 0, cm.unitlength);
        if(lefttopindex == 335)
        {
            Debug.Log(lefttopindex);
        }
        var flag = MapModifier.Instance.CheckContainUnreachable(lefttopindex, size);
        if (flag)
        {
            Handles.color = Color.green;
        }
        else
        {
            Handles.color = Color.red;
        }
        Handles.DrawSolidDisc(lefttopcenter, Vector3.up, cm.unitlength/2);
        switch (current.type)
        {
            case EventType.mouseDown:
                if (current.button == 0)
                {
                    if (flag == false)
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

    public CellEditorHandler(CustomMap map) : base(map)
    { }
}