using System;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class EasytouchHierachyCallBack
{
    // 层级窗口项回调
    private static readonly EditorApplication.HierarchyWindowItemCallback hiearchyItemCallback;

    private static Texture2D hierarchyIcon;
    private static Texture2D HierarchyIcon {
        get {
            if (EasytouchHierachyCallBack.hierarchyIcon==null){
                EasytouchHierachyCallBack.hierarchyIcon = (Texture2D)Resources.Load("new hint");
            }
            return EasytouchHierachyCallBack.hierarchyIcon;
        }
    }   

    private static Texture2D hierarchyEventIcon;
    private static Texture2D HierarchyEventIcon {
        get {
            if (EasytouchHierachyCallBack.hierarchyEventIcon==null){
                EasytouchHierachyCallBack.hierarchyEventIcon = (Texture2D)Resources.Load( "new hint");
            }
            return EasytouchHierachyCallBack.hierarchyEventIcon;
        }
    }

    /// <summary>
    /// 静态构造
    /// </summary>
    static EasytouchHierachyCallBack()
    {
        EasytouchHierachyCallBack.hiearchyItemCallback = new EditorApplication.HierarchyWindowItemCallback(EasytouchHierachyCallBack.DrawHierarchyIcon);
        EditorApplication.hierarchyWindowItemOnGUI = (EditorApplication.HierarchyWindowItemCallback)Delegate.Combine(
            EditorApplication.hierarchyWindowItemOnGUI, 
            EasytouchHierachyCallBack.hiearchyItemCallback);

        //EditorApplication.update += Update;
    }

    // 绘制icon方法
    private static void DrawHierarchyIcon(int instanceID, Rect selectionRect)
    {
        GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (gameObject)//场景不是gameObject
        {
            if (gameObject.name.Length >= 7 && gameObject.name.Substring(gameObject.name.Length - 7, 7) == "(Clone)")
            {
                // 设置icon的位置与尺寸（Hierarchy窗口的左上角是起点）
                Rect rect = new Rect(selectionRect.x + selectionRect.width - 16f, selectionRect.y, 16f, 16f);
                // 画icon
                GUI.DrawTexture(rect, EasytouchHierachyCallBack.HierarchyEventIcon);
            }
        }
    }
}