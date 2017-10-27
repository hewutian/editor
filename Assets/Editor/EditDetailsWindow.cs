using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class EditDetailsWindow : EditorWindow {
    public static Action destroyEvent;
    public static Action deleteAction;
    public static PropertyField[] toDraw;
    public static Type toDrawType;
    private static EditDetailsWindow myWindow;

    public static EditDetailsWindow MyWindow
    {
        get
        {
            if (myWindow == null)
                myWindow = EditorWindow.GetWindow<EditDetailsWindow>();
            return myWindow;
        }
    }

    private void Awake()
    {
    }

    public static void AddWindow(Action ac)
    {
        Rect wr = new Rect(0, 0, 250, 300);
        myWindow = GetWindowWithRect<EditDetailsWindow>(wr);
        destroyEvent = ac;
        myWindow.Show();
    }

    private void OnGUI()
    {
        if (toDraw != null && toDrawType != null)
        {
            GUILayout.Label("类型：" + toDrawType.ToString());
            Expose(toDraw);
        }
    }

    public static void Expose(PropertyField[] properties)
    {
        if (properties == null)
            return;

        var emptyOptions = new GUILayoutOption[0];
        GUILayout.BeginVertical();
        foreach (PropertyField field in properties)
        {
            GUILayout.BeginHorizontal();
            if (field.Type == SerializedPropertyType.Integer)
            {
                var oldValue = (int)field.GetValue();
                GUILayout.Label(field.Name);
                var newValue = int.Parse(GUILayout.TextField(oldValue.ToString()));
                if (oldValue != newValue)
                    field.SetValue(newValue);
            }
            else if (field.Type == SerializedPropertyType.Float)
            {
                var oldValue = (float)field.GetValue();
                GUILayout.Label(field.Name);
                var newValue = float.Parse(GUILayout.TextField(oldValue.ToString()));
                if (oldValue != newValue)
                    field.SetValue(newValue);
            }
            else if (field.Type == SerializedPropertyType.Boolean)
            {
                var oldValue = (bool)field.GetValue();
                GUILayout.Label(field.Name);
                var newValue = bool.Parse(GUILayout.TextField(oldValue.ToString()));
                if (oldValue != newValue)
                    field.SetValue(newValue);
            }
            else if (field.Type == SerializedPropertyType.String)
            {
                var oldValue = (string)field.GetValue();
                GUILayout.Label(field.Name);
                var newValue = GUILayout.TextField(oldValue.ToString());
                if (oldValue != newValue)
                    field.SetValue(newValue);
            }
            else if (field.Type == SerializedPropertyType.Vector3)
            {
                var oldValue = (Vector3)field.GetValue();

                GUILayout.Label(field.Name);
                GUILayout.Label("x:");
                var newValuex = float.Parse(GUILayout.TextField(oldValue.x.ToString()));
                GUILayout.Label("y:");
                var newValuey = float.Parse(GUILayout.TextField(oldValue.y.ToString()));
                GUILayout.Label("z:");
                var newValuez = float.Parse(GUILayout.TextField(oldValue.z.ToString()));
                field.SetValue(new Vector3(newValuex, newValuey, newValuez));
            }
            GUILayout.EndHorizontal();
        }
        if (GUILayout.Button("删除"))
        {
            if (deleteAction != null)
            {
                deleteAction();
                toDraw = null;
                toDrawType = null;
            }
        }
        GUILayout.EndVertical();
    }

    private void OnInspectorUpdate()
    {
        this.Repaint();
    }

    private void OnDestroy()
    {
        Debug.Log("CLose!");
        if (destroyEvent != null)
            destroyEvent();
        destroyEvent = null;
        toDraw = null;
        toDrawType = null;
    }
}
