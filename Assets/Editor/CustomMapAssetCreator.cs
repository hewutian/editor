using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class CustomMapAssetCreator
{
    [MenuItem("CustomMap/Create File")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<CustomMap>();
    }
	

    [MenuItem("CustomMap/Create File From CurTxtFile")]
    public static void CreateAssetFromFile()
    {

    }
}
