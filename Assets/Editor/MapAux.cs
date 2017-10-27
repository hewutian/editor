using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class MapAux
{
    //绘制地图的所有小格
    public static void DrawMapCells(Vector3 center,Vector3 size ,float unitlength,Color cl)
    {
        int xnum = (int)size.x;
        int znum = (int)size.z;
        Handles.color = cl;
        Vector3 lefttop = new Vector3(center.x - xnum * unitlength / 2, size.y, center.z + znum * unitlength / 2);
       // Vector3[] xpoints = new Vector3[xnum + 1];
       // Vector3[] zpoints = new Vector3[znum + 1];
        for(int i =0;i<= xnum;++i)
        {
            Vector3 curpoint = lefttop + new Vector3(i *unitlength, 0, 0);
            Handles.DrawLine(curpoint, curpoint - new Vector3(0,0,znum * unitlength));
        }

        for (int i = 0; i <= znum; ++i)
        {
            Vector3 curpoint = lefttop - new Vector3(0, 0, unitlength *i );
            Handles.DrawLine(curpoint, curpoint + new Vector3(xnum*unitlength, 0, 0));
        }
    }
    public static void DrawMapCellsDotted(Vector3 center, Vector3 size, float unitlength, Color cl)
    {
        int xnum = (int)size.x;
        int znum = (int)size.z;
        Handles.color = cl;
        Vector3 lefttop = new Vector3(center.x - xnum * unitlength / 2, size.y, center.z + znum * unitlength / 2);
        // Vector3[] xpoints = new Vector3[xnum + 1];
        // Vector3[] zpoints = new Vector3[znum + 1];
        for (int i = 0; i <= xnum; ++i)
        {
            Vector3 curpoint = lefttop + new Vector3(i * unitlength, 0, 0);
            Handles.DrawDottedLine(curpoint, curpoint - new Vector3(0, 0, znum * unitlength),2f);
        }

        for (int i = 0; i <= znum; ++i)
        {
            Vector3 curpoint = lefttop - new Vector3(0, 0, unitlength * i);
            Handles.DrawDottedLine(curpoint, curpoint + new Vector3(xnum * unitlength, 0, 0),2f);
        }
    }
    //绘制不可达的格子 
    public static void DrawMapUnreachableArea(Vector3 center,float unitlength,Color cl)
    {
        Vector3 p2 = center + Vector3.up * 0f + Vector3.right * 0.5f * unitlength + Vector3.forward * 0.5f * unitlength;
        Vector3 p3 = center + Vector3.up * 0f + Vector3.right * 0.5f * unitlength - Vector3.forward * 0.5f * unitlength;
        Vector3 p4 = center + Vector3.up * 0f - Vector3.right * 0.5f * unitlength - Vector3.forward * 0.5f * unitlength;
        Vector3 p1 = center + Vector3.up * 0f - Vector3.right * 0.5f * unitlength + Vector3.forward * 0.5f * unitlength;

        Handles.color = cl;
        Handles.DrawLine(p1, p2);
        Handles.DrawLine(p2, p3);
        Handles.DrawLine(p3, p4);
        Handles.DrawLine(p4, p1);

        DrawCrossOnRect(p1, p2, p4, p3, cl);
    }

    public static void DrawGameObjectBorder(Vector3  center,Vector3 size, float unitlength, Color cl)
    {

    }

    public static void DrawGameObjectBorderByLefttop(Vector3 lefttop, Vector3 size, float unitlength, Color cl)
    {

    }
    
    //在特定的格子上画叉
    private static void DrawCrossOnRect(Vector3 lt, Vector3 lb, Vector3 rt, Vector3 rb, Color cl)
    {
        Handles.color = cl;
        Handles.DrawLine(lt, rb);
        Handles.DrawLine(lb, rt);
    }

    public static void DrawRectHandles(Vector3 start,Vector3 end)
    {

        //Vector3[] verts = {start, new Vector3(end.x,start.y,start.z) , end, new Vector3(start.x, start.y, end.z) };
        //  Handles.color = Color.green;
        //Handles.DrawSolidRectangleWithOutline(verts, new Color(1, 1, 1, 0.2f), new Color(0, 0, 0, 1));
        // Handles.DrawSolidRectangleWithOutline(verts, Color.red,Color.green);
        Handles.color = Color.green;
        Vector3[] verts = { start, new Vector3(end.x, start.y, start.z), end, new Vector3(start.x, start.y, end.z), start };
        Handles.DrawPolyLine(verts);

       
    }

    public static void ShowLabel(Vector3 pos,string text,Color cl)
    {
        var newstyle = new GUIStyle();
        newstyle.fontSize = 10;
        newstyle.normal.textColor = cl;
        Handles.Label(pos, text, newstyle);
    }


    //根据左上格子的中心位置来绘制一定规格的矩形
    public static void DrawLines(Vector3 pos, Vector3 size, Color cl)
    {
        int xlength = (int)size.x;
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
}
