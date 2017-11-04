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
public struct Grids
{
    public int onegrid;
    public int another;
}


[System.Serializable]
public class CustomItemInfo //: ScriptableObject
{
    public int lefttopsite;
    public int tid;
    public int dir;

}

[System.Serializable]
public class NodeInfo//:ScriptableObject
{
    public Vector2 site;
    public string name = "";
    public int id;
}

[System.Serializable]
public class AreaInfo//:ScriptableObject
{
    public Vector2 start;
    public Vector2 end;
    public string name = "";
    public int id;
}

[System.Serializable]
public class WallInfo
{
    //  public int onegrid;
    //   public int another;
    // public List
    public List<Grids> grids;
    public int tid;

}

[System.Serializable]
public class DoorInfo
{
    // public int onegrid;
    // public int another;
    public List<Grids> grids;
    public int tid;
    public int openstate;
}
