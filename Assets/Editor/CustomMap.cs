using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomMap : ScriptableObject
{
    public int mapwidth;
    public int mapheight;
    public int unitlength;
    public int paintedgridlength;
  //  public int tilelength;
    public Vector3 dir;
    public float max = 5;

    [DPCIgnore]
    public GameObject scene;

    public bool hasGeneratedData = false;
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
}
