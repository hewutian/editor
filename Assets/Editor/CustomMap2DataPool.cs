using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public static class CustomMap2DataPool {
    private static string CustomMap2String(CustomMap cm)
    {
        string context = "";
        int tab_count = 0;

        //头部
        context += AddContent(tab_count, "\r\n");
        context += AddContent(tab_count++, "mapdata{\r\n");

        context += AddContent(tab_count, string.Format("mapwidth={0}", "\""+cm.mapwidth.ToString()+"\";\r\n"));
        context += AddContent(tab_count, string.Format("mapheight={0}", "\""+cm.mapheight.ToString()+"\";\r\n"));
        context += AddContent(tab_count, string.Format("unitlength={0}", "\""+cm.unitlength.ToString()+"\";\r\n"));
        context += AddContent(tab_count, string.Format("paintedgridlength={0}", "\""+cm.paintedgridlength.ToString()+"\";\r\n"));
        context += AddContent(tab_count, string.Format("tilelength={0}", "\""+cm.tilelength.ToString()+"\";\r\n"));

        context += AddContent(tab_count++, "dir{\r\n");
        context += AddContent(tab_count, string.Format("x={0}", "\""+cm.dir.x.ToString()+"\";\r\n"));
        context += AddContent(tab_count, string.Format("y={0}", "\""+cm.dir.y.ToString()+"\";\r\n"));
        context += AddContent(tab_count, string.Format("z={0}", "\""+cm.dir.z.ToString()+"\";\r\n"));
        context += AddContent(--tab_count, "};\r\n");

        context += AddContent(tab_count, string.Format("max={0}", "\""+cm.max.ToString()+"\";\r\n"));

        context += AddContent(tab_count++, "scene{\r\n");
        context += AddContent(tab_count, string.Format("instanceID={0}", "\""+cm.scene.GetInstanceID().ToString()+"\";\r\n"));
        context += AddContent(--tab_count, "};\r\n");

        context += AddContent(tab_count, string.Format("prefabName={0}", "\""+cm.prefabName.ToString()+"\";\r\n"));
        
        context += AddContent(tab_count++, "center{\r\n");
        context += AddContent(tab_count, string.Format("x={0}", "\""+cm.center.x.ToString()+"\";\r\n"));
        context += AddContent(tab_count, string.Format("y={0}", "\""+cm.center.y.ToString()+"\";\r\n"));
        context += AddContent(tab_count, string.Format("z={0}", "\""+cm.center.z.ToString()+"\";\r\n"));
        context += AddContent(--tab_count, "};\r\n");

        foreach(CustomItemInfo info in cm.itemlist)
        {
            context += AddContent(tab_count++, "itemlist{\r\n");
            context += AddContent(tab_count, string.Format("type={0}", "\""+info.type.ToString()+"\";\r\n"));
            context += AddContent(tab_count, string.Format("posy={0}", "\""+info.posy.ToString()+"\";\r\n"));
            context += AddContent(tab_count, string.Format("lefttopsite={0}", "\""+info.lefttopsite.ToString()+"\";\r\n"));
            context += AddContent(tab_count, string.Format("width={0}", "\""+info.width.ToString()+"\";\r\n"));
            context += AddContent(tab_count, string.Format("height={0}", "\""+info.height.ToString()+"\";\r\n"));
            context += AddContent(tab_count, string.Format("isreachable={0}", "\""+(info.isreachable?1:0).ToString()+"\";\r\n"));
            context += AddContent(tab_count++, "prefab{\r\n");
            context += AddContent(tab_count, string.Format("instanceID={0}", "\""+info.prefab.GetInstanceID().ToString()+"\";\r\n"));
            context += AddContent(--tab_count, "};\r\n");
            context += AddContent(--tab_count, "};\r\n");
        }

        foreach(int unreach in cm.unreachable)
        {
            context += AddContent(tab_count, string.Format("unreachable={0}", "\""+unreach.ToString()+"\";\r\n"));              
        }

        context += AddContent(--tab_count, "};\r\n");
        return context;
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

        if (!File.Exists(DPath + "/mapdata.dpc"))
            File.Delete(DPath + "/mapdata.dpc");

        FileStream fstream = File.Create(DPath + "/mapdata.dpc");

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
