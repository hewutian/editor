using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapEditorFSM {

    private static MapEditorFSM instance;
    public static MapEditorFSM Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new MapEditorFSM()
                {
                    curState = e_Editor_State.Unwork,
                };
            }
            return instance;
        }
    }
    public e_Editor_State curState;

    [InitializeOnLoadMethod]
    public void Init()
    {
        instance = new MapEditorFSM()
        {
            curState = e_Editor_State.Unwork,
        };
    }
}

public enum e_Editor_State
{
    Unwork = 0,
    Edit_Map = 2,
    Edit_Pathfind_Cell = 4,
    Edit_Point_and_Area = 8,
    Edit_Object = 16,
}
