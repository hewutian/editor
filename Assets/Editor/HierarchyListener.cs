using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class EditorMonoBehaviour
{
    [InitializeOnLoadMethod]
    static void Start()
    {
        Selection.selectionChanged = delegate
        {
            //Debug.Log(Selection.activeObject.name);
        };

    }
}