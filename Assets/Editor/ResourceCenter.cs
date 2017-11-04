using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
public class ResourceCenter
{

    public GameObject[] prefabObjects;// = new GameObject[0];
    public Texture2D[] thumbnails;//= new Texture2D[0];
    public Dictionary<string, GameObject> objectDic;

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
        DPM.Instance.Init();
       // LoadDPC();
        test();
    }
    
    public void LoadAllPrefabsInDirectory(string path)
    {
        DirectoryInfo dirs = new DirectoryInfo(path);
        FileInfo[] files = dirs.GetFiles("*.prefab");
        List<GameObject> gameobjects = new List<GameObject>();
        objectDic = new Dictionary<string, GameObject>();
        foreach(FileInfo fi in files)
        {
            string fullpath = fi.FullName.Replace(@"\", "/");
            fullpath = "Assets" + fullpath.Replace(Application.dataPath, "");
            GameObject pre = AssetDatabase.LoadAssetAtPath(fullpath,typeof(GameObject)) as GameObject;
            if (pre != null)
            {
                gameobjects.Add(pre);
                objectDic.Add(pre.name, pre);
            }
        }
        prefabObjects = new GameObject[gameobjects.Count];
        prefabObjects = gameobjects.ToArray();
    }

    public void LoadDPC()
    {
        var tmp = DPM.Instance.BuildingsDP;
    }
    public void test()
    {

        var tmp = DPM.FindByKey(DPM.Instance.BuildingsDP, "build", 2);
        var tmp2 = DPM.GetArray(tmp);
        List<DataPoolVariable> list = new List<DataPoolVariable>();
        // 这个地方要克隆，否则itFirst和it是一个地址的引用
        DPM.PrintData(tmp);
    }

    public  GameObject GetPrefabInstance(int type)
    {
        string path;
        switch(type)
        {
            case 1:
                path = "DynamicObject\\floor\\prefab";
                break;
            case 2:
                path = "DynamicObject\\wall\\prefab";
                break;
            case 3:
                path = "DynamicObject\\door\\prefab";
                break;
         //   case 4:
         //       path = "";
        //        break;
            default:
                path = "";
                break;
        }
        if (path == "")
            return null;
        DirectoryInfo dirs = new DirectoryInfo("Assets/" + path);
        FileInfo[] files = dirs.GetFiles("*.prefab");
        if (files.Length <= 0)
            return null;
        string fullpath = files[0].FullName.Replace(@"\", "/");
        fullpath = "Assets" + fullpath.Replace(Application.dataPath, "");
        GameObject sample = (GameObject)AssetDatabase.LoadAssetAtPath(fullpath, typeof(GameObject));
        GameObject target = GameObject.Instantiate<GameObject>(sample);
        return target;
    }


 }
