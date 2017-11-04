using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class HandleRecorder
{
    static  int curControlID;
   public static  Dictionary<int, object> handleIDAndTarget = new Dictionary<int, object>();



   public static void RecordHandles(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
    {
        //Debug.Log(controlID);

        curControlID = controlID;
    }

    public static void CheckID(object info)
    {
        if (curControlID == 0)
            return;
        if (!HandleRecorder.handleIDAndTarget.ContainsKey(curControlID))
        {
            HandleRecorder.handleIDAndTarget.Add(curControlID, info);
        }
        else if (HandleRecorder.handleIDAndTarget[curControlID] != info)
        {
            HandleRecorder.handleIDAndTarget[curControlID] =info;
        }
    }

}
