using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class Helper
{

    public static bool CheckContainUnreachable(int siteindex ,Vector3 size, CustomMap cm)
    {
        int xlength = (int)size.x / cm.unitlength;
        int zlength = (int)size.z / cm.unitlength;
        int num = xlength * zlength;
        var unreachable = cm.unreachable;
        for (int i = 0;i< num;++i)
        {
            int index = siteindex + (i / xlength) * cm.mapwidth / cm.unitlength + i%xlength ;
            if(  unreachable.Contains(index))
            {
                return true;
            }
         }
        return false;
    }

    public static void DrawLines(Vector3 pos, Vector3 size,CustomMap cm ,Color cl)
    {
        int xlength =(int)size.x;
        int ylength = (int)size.y;
        int zlength = (int)size.z;
         Vector3 p2 = pos + Vector3.up * 0f + Vector3.right * (xlength - 0.5f) + Vector3.forward * 0.5f * 1;
        Vector3 p3 = pos + Vector3.up * 0f + Vector3.right * (xlength - 0.5f) - Vector3.forward * (zlength - 0.5f);
        Vector3 p4 = pos + Vector3.up * 0f - Vector3.right * 0.5f * 1 - Vector3.forward * (zlength - 0.5f);
        Vector3 p1 = pos + Vector3.up * 0f - Vector3.right * 0.5f * 1 + Vector3.forward * 0.5f * 1;
        //Vector3 p1 = center + Vector3.up * 0.5f + Vector3.right * 0.5f + Vector3.forward * 0.5f;
        //Vector3 p2 = center + Vector3.up * 0.5f + Vector3.right * 0.5f - Vector3.forward * 0.5f;
        //Vector3 p3 = center + Vector3.up * 0.5f - Vector3.right * 0.5f - Vector3.forward * 0.5f;
        //Vector3 p4 = center + Vector3.up * 0.5f - Vector3.right * 0.5f + Vector3.forward * 0.5f;
        //Vector3 p5 = center - Vector3.up * 0.5f + Vector3.right * 0.5f + Vector3.forward * 0.5f;
        //Vector3 p6 = center - Vector3.up * 0.5f + Vector3.right * 0.5f - Vector3.forward * 0.5f;
        //Vector3 p7 = center - Vector3.up * 0.5f - Vector3.right * 0.5f - Vector3.forward * 0.5f;
        //Vector3 p8 = center - Vector3.up * 0.5f - Vector3.right * 0.5f + Vector3.forward * 0.5f;
        Handles.color = cl;
        Handles.DrawLine(p1, p2);
        Handles.DrawLine(p2, p3);
        Handles.DrawLine(p3, p4);
        Handles.DrawLine(p4, p1);
        //Handles.DrawLine(p5, p6);
        //Handles.DrawLine(p6, p7);
        //Handles.DrawLine(p7, p8);
        //Handles.DrawLine(p8, p5);
        //Handles.DrawLine(p1, p5);
        //Handles.DrawLine(p2, p6);
        //Handles.DrawLine(p3, p7);
        //Handles.DrawLine(p4, p8);
    }

   public static Vector3  CaculateGameObjectSize(CustomMap cm , int selectedIndex)
    {
        int xlength = 1;
        int ylength = 1;
        int zlength = 1;
        if (selectedIndex != null)
        {
            var obj = ResourceCenter.Instance.prefabObjects[selectedIndex];
            //var size = obj.GetComponent<Collider>().bounds.size;
            var size = obj.GetComponent<Renderer>().bounds.size;
            xlength = (int)Mathf.Ceil(size.x / cm.unitlength);
            zlength = (int)Mathf.Ceil(size.z / cm.unitlength);
            ylength = (int)Mathf.Ceil(size.y / cm.unitlength);
        }
        return new Vector3(xlength, ylength, zlength);
    }

    public static  List<int> Detect(List<Vector3> pos, Vector3 dir, float max)
    {
        int i = 0;
        List<int> unreachable = new List<int>();
        foreach (var p in pos)
        {
            bool res = CastLine(p, dir, max);
            if (res == false)
            {
                unreachable.Add(i); //保存的这个i是地图的小格子 从0开始计数
            }
            i++;
        }
        return unreachable;
    }

    public  static bool CastLine(Vector3 pos, Vector3 dir, float max)
    {
        RaycastHit hit;
        bool flag = false;
        if (Physics.Raycast(pos, dir, out hit))
        {
            float depth = hit.point.y;
            if (depth <= max)
            {
                flag = true;
            }
        }
        return flag;
    }

    public static Vector3 CaculateCreateGameObjectCenter(Vector3 pos, Vector3 size, CustomMap cm)
    {
        int xlength = (int)size.x;
        int zlength = (int)size.z;
        Vector3 center = new Vector3();
        center.x = pos.x + (float)(xlength / 2f - 0.5) * cm.unitlength;
        center.y = pos.y;
        center.z = pos.z - (float)(zlength / 2f - 0.5) * cm.unitlength;
        return center;
    }

    public static bool CheckListContainIndex(List<int> unreachable,int index)
    {
        return false;
    }


}
