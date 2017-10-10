using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
public class ResourceCenter
{

    public GameObject[] prefabObjects;// = new GameObject[0];
    public Texture2D[] thumbnails;//= new Texture2D[0];
    private static ResourceCenter instance;
    public static ResourceCenter Instance
    {
        get
        {
            if (instance == null)
                instance = new ResourceCenter();
            return instance;
        }

    }

    void LoadAllPrefabs(string path)
    {
        var tmp = AssetDatabase.LoadAllAssetsAtPath(path);
        int i = 0;
       foreach(var o in tmp)
        {
            prefabObjects[i] = (GameObject)o;
            ++i;
        }
    }

    void GenerateAllThumbnails()
    {
        thumbnails = new Texture2D[prefabObjects.Length];
        int i = 0;
        foreach (var g in prefabObjects)
        {
            thumbnails[i] = AssetPreview.GetAssetPreview(g);
            ++i;
        }
    }

	public void Init(string prefabpath)
    {
        // LoadAllPrefabs(prefabpath);
        LoadAllPrefabsInDirectory(prefabpath);
        GenerateAllThumbnails();
    }



    public void LoadAllPrefabsInDirectory(string path)
    {
        DirectoryInfo dirs = new DirectoryInfo(path);
        FileInfo[] files = dirs.GetFiles("*.prefab");
        List<GameObject> gameobjects = new List<GameObject>();
        foreach(FileInfo fi in files)
        {
            string fullpath = fi.FullName.Replace(@"\", "/");
            fullpath = "Assets" + fullpath.Replace(Application.dataPath, "");
            GameObject pre = AssetDatabase.LoadAssetAtPath(fullpath,typeof(GameObject)) as GameObject;
            if (pre != null)
                gameobjects.Add(pre);
        }
        prefabObjects = new GameObject[gameobjects.Count];
        prefabObjects = gameobjects.ToArray();
    }
}
