using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Reflection;

public static class CustomMap2DataPool {
    private static string CustomMap2String(CustomMap cm)
    {
        string result = "";
        int count = 0;
        result += AddContent(count++, "\r\nmapdata{\r\n");

        FieldInfo[] fieldInfos = cm.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
        foreach (FieldInfo info in fieldInfos)
        {
            Convert2String(info.GetValue(cm), ref result, ref count, info);
        }

        result += AddContent(--count, "};\r\n");
        Debug.Log(result);
        return result;
    }

    public static void Convert2DataPool(CustomMap cm)
    {
        string DPath = Application.dataPath + "/DataPool";
        if (!Directory.Exists(DPath))
            Directory.CreateDirectory(DPath);

        try
        {
            FileStream fstream = File.Open(DPath + "/MAPDATA.data.txt", FileMode.OpenOrCreate);
            string context = CustomMap2String(cm);

            char[] charData = context.ToCharArray();
            byte[] byteData = new byte[charData.Length];
            Encoder e = Encoding.UTF8.GetEncoder();
            e.GetBytes(charData, 0, charData.Length, byteData, 0, true);

            fstream.Seek(0, SeekOrigin.End);
            fstream.Write(byteData, 0, byteData.Length);
            //Debug.Log(byteData.Length);
            //Debug.Log(context);
        }
        catch (IOException ex)
        {
            Debug.Log("IO Exception rised : " + ex.ToString());
        }
    }

    private static void Convert2String(object data, ref string result, ref int count, FieldInfo field)
    {
        if (data is GameObject)
            return;

        if (data is ICollection && data is IList)
        {
            for (int i = 0; i < (data as IList).Count; i++)
            {
                Convert2String((data as IList)[i], ref result, ref count, field);
            }
            return;
        }

        if ((data.GetType().IsClass && !(data is string)) || data is Vector3)
        {
            result += AddContent(count++, field.Name + "{\r\n");
            FieldInfo[] fieldInfos = data.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (FieldInfo info in fieldInfos)
            {
                Convert2String(info.GetValue(data), ref result, ref count, info);
            }
            result += AddContent(--count, "};\r\n");
            return;
        }

        string details = data.ToString();
        if (data is bool)
            details = (bool)data ? "1" : "0";

        result += AddContent(count, string.Format("{0}=\"{1}\";\r\n", field.Name, details));
    }

    private static string AddContent(int tab_count, string context_plus)
    {
        string result = "";
        for (int i = 0; i < tab_count; i++)
        {
            result += "\t";
        }
        result += context_plus;
        return result;
    }

    [MenuItem("DataPool/Export File")]
    public static bool GenerateDataPoolFile()
    {
        string DPath = Application.dataPath + "/DataPool";
        if (!Directory.Exists(DPath))
            Directory.CreateDirectory(DPath);

        if (!File.Exists(DPath + "/MAPDATA.data.txt"))
            File.Delete(DPath + "/MAPDATA.data.txt");

        FileStream fstream = File.Create(DPath + "/MAPDATA.data.txt");

        FileInfo[] allFiles = new DirectoryInfo(Application.dataPath).GetFiles("*.asset");
        string resultStream = "";
        foreach (FileInfo file in allFiles)
        {
            if (file.Extension == ".asset")
            {
                CustomMap temp = AssetDatabase.LoadAssetAtPath<CustomMap>("Assets/" + file.Name);
                if (temp != null)
                {
                    resultStream += CustomMap2String(temp);
                    //Debug.Log(resultStream);
                }
                else
                {
                    Debug.Log("Error Asset File");
                }
            }
        }

        try
        {
            char[] charData = resultStream.ToCharArray();
            byte[] byteData = new byte[charData.Length];
            Encoder e = Encoding.UTF8.GetEncoder();
            e.GetBytes(charData, 0, charData.Length, byteData, 0, true);

            fstream.Seek(0, SeekOrigin.Begin);
            fstream.Write(byteData, 0, byteData.Length);
        }
        catch(IOException ex)
        {
            Debug.Log("IO Exception rised : " + ex.ToString());
        }

        return true;
    }
}
