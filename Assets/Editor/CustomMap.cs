using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomMap : ScriptableObject {

    public int mapwidth;
    public int mapheight;
    public int unitlength;
    public int paintedgridlength;
    public int tilelength;
    public Vector3 dir;
    public float max = 5;
    public GameObject scene;
    public string prefabName;
    public bool hasGeneratedData = false;
    public Vector3 center;

    //public CustomItemInfo[] itemList = new CustomItemInfo[0];
    [SerializeField]
    public List<CustomItemInfo> itemlist = new List<CustomItemInfo>(1);

    [SerializeField]
    public List<int> unreachable = new List<int>();

    [SerializeField]
    public List<NodeInfo> designerNode = new List<NodeInfo>();
    public List<AreaInfo> designerArea = new List<AreaInfo>();
    //public int[] unreachable = new int[0];
}
