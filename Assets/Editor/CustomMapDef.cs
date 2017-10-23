using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum e_ItemType
{
     Tree = 0,
     Box ,
     Stone ,

}

public enum e_NodeType
{
    SourcePoint = 0,
    MonsterPoint,
}

public enum e_AreaType
{
    Event = 0,
}

[System.Serializable]
public class CustomItemInfo //: ScriptableObject
{
   // public e_ItemType type;
    public float posy;
    public int lefttopsite;
    public int width;
    public int height;
    public bool isreachable;
    public string name = "";
    public int id;
    //public GameObject prefab;

}

[System.Serializable]
public class NodeInfo//:ScriptableObject
{
   // public e_NodeType type;
    public int  id;
    public float posy;
    public Vector3 site;
    public string name = "";
}

[System.Serializable]
public class AreaInfo//:ScriptableObject
{
   // public e_AreaType type;
   // public int width;
    //public int height;
   // public int lefttopindex;
    public Vector3 start;
    public Vector3 end;
    public string name = "";
    public int id;
}
