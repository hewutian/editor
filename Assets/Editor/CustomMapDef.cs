using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum e_ItemType
{
     Tree = 0,
     Box ,
     Stone ,

}




[System.Serializable]
public class CustomItemInfo :ScriptableObject
{
    public e_ItemType type;
    public float posy;
    public int lefttopsite;
    public int width;
    public int height;
    public bool isreachable;
    public GameObject prefab;
}

