using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public abstract class EditorHandler
{
    protected CustomMap cm;
    abstract public void DealWithEvent();
    abstract public void ShowAuxInfo();
    public EditorHandler(CustomMap map)
    {
        cm = map;
    }
}


public class DefualtEditorHandler : EditorHandler
{
    public override void ShowAuxInfo()
    {
        
    }

    public override void DealWithEvent()
    {
        
    }

    public DefualtEditorHandler(CustomMap map) : base(map)
    { }
}


