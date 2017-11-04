using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomMap : ScriptableObject
{
    public float mapwidth;
    public float mapheight;
    public float unitlength;
    public float paintedgridlength;
    [DPCIgnore]
    public Vector3 dir;
    public float max = 5;
    [DPCIgnore]
    public GameObject scene;
    [DPCIgnore]
    public bool hasGeneratedData = false;
    [DPCIgnore]
    public Vector3 center;

    public string prefabName;
    public int tid;
    [SerializeField]
    public List<int> unreachable = new List<int>();
    [SerializeField]
     public List<NodeInfo> designerNode = new List<NodeInfo>();
    [SerializeField]
    public List<AreaInfo> designerArea = new List<AreaInfo>();
    [SerializeField]
    public List<CustomItemInfo> itemlist = new List<CustomItemInfo>();
    [SerializeField]
    public List<WallInfo> walllist = new List<WallInfo>();
    [SerializeField]
    public List<DoorInfo> doorlist = new List<DoorInfo>();
}
