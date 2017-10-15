using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomMap : ScriptableObject {

    public int mapwidth;
    public int mapheight;
    public int unitlength;
    public int tilelength;
    public Vector3 dir;
    public float max = 5;
    public GameObject scene;
    public string prefabName;
    public bool hasGeneratedData = false;
    public Vector3 center;

    //public CustomItemInfo[] itemList = new CustomItemInfo[0];
    public List<CustomItemInfo> itemlist = new List<CustomItemInfo>();
    public List<int> unreachable = new List<int>();
    //public int[] unreachable = new int[0];
}
