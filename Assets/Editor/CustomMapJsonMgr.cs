using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

/// <summary>
/// CustomMap文件与Json交互的管理类
/// </summary>
public static class CustomMapJsonMgr {
    /// <summary>
    /// 将CustomMap导出成Json文件，生成目录为Assets/Json，文件名为<mapname>.json
    /// </summary>
    /// <param name="target">需要导出json的CustomMap</param>
    public static void MapDataToJson(Object target)
    {
        string DPath = Application.dataPath;
        int num = DPath.LastIndexOf("/");
        DPath = DPath.Substring(0, num);
        string directory = DPath +"/Assets/Json";

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        string json_text = JsonUtility.ToJson(target);

        //当文件不存在时直接生成
        if (!File.Exists(directory + "/" + target.name + ".json"))
        {
            try
            {
                FileStream fstream = File.Create(directory + "/" + target.name + ".json");

                char[] charData = json_text.ToCharArray();
                byte[] byteData = new byte[charData.Length];
                Encoder e = Encoding.UTF8.GetEncoder();
                e.GetBytes(charData, 0, charData.Length, byteData, 0, true);

                fstream.Seek(0, SeekOrigin.Begin);
                fstream.Write(byteData, 0, byteData.Length);
            }
            catch(IOException ex)
            {
                Debug.Log("IO Exception rised : "+ex.ToString());
            }
        }
        //文件已存在，则提示是否覆盖成新的
        else
        {
            if (UnityEditor.EditorUtility.DisplayDialog("Notice", target.name + ".json existed, Click <OK> to cover the old file", "OK", "Cancel"))
            {
                File.Delete(directory + "/" + target.name + ".json");
                try
                {
                    FileStream fstream = File.Create(directory + "/" + target.name + ".json");

                    char[] charData = json_text.ToCharArray();
                    byte[] byteData = new byte[charData.Length];
                    Encoder e = Encoding.UTF8.GetEncoder();
                    e.GetBytes(charData, 0, charData.Length, byteData, 0, true);

                    fstream.Seek(0, SeekOrigin.Begin);
                    fstream.Write(byteData, 0, byteData.Length);
                }
                catch (IOException ex)
                {
                    Debug.Log("IO Exception rised : " + ex.ToString());
                }
            }
        }
    }

    /// <summary>
    /// 外部Json文件导入成CustomMap文件
    /// </summary>
    /// <param name="json_text">Json文件路径</param>
    public static void JsonToMapData(CustomMap cm, string json_path)
    {
        FileStream fstream = File.OpenRead(json_path);
        if (fstream == null)
            return;

        byte[] byteData = new byte[fstream.Length];
        int num = fstream.Read(byteData, 0, byteData.Length);
        string json_text = Encoding.UTF8.GetString(byteData);

        JsonUtility.FromJsonOverwrite(json_text, cm);
    }
}
