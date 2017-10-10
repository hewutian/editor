using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//[CustomEditor(typeof(DesignerNeed))]
//[CustomEditor(typeof(Camera))]
public class CameraExtend : Editor
{

    //void OnSceneGUI()
    //{
        


    //    //if(Event.current.type == EventType.mouseDown)
    //    //{

    //    //}

    //    Event current = Event.current;
    //    // int controlID = GUIUtility.GetControlID(FocusType.Passive);
    //    switch (current.type)
    //    {
    //        case EventType.mouseDown:
    //            AddObject();
    //            break;

    //        case EventType.mouseUp:
    //            AddObject();
               
    //            break;
    //        case EventType.layout:
    //            // HandleUtility.AddDefaultControl(controlID);
    //            break;
    //    }
    //   // current.Use();
    //}

    void AddObject()

    {
        Debug.Log("mouse click");
    }
}
