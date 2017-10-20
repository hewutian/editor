using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
public interface ICommand
{
     void Undo();
     void Do();
}

public class AddAndDeleteCommand:ICommand
{
    object obj;
    bool add;
    string valuename;
    public object Obj
    {
        get { return obj; }
    }
    public string Valuename
    {
        get { return valuename; }
    }


    public AddAndDeleteCommand(object go,bool aod,string valuename)
    {
        obj = go;
        add = aod;
    }

    public void Undo()
    {
        if (add == true )
        {
            PropertyInfo info = MapModifier.Instance.CurMap.GetType().GetProperty(valuename);
            System.Collections.IList test = (System.Collections.IList)info.GetValue(MapModifier.Instance.CurMap, null);
            test.Remove(obj);
        }
        else
        {
            PropertyInfo info = MapModifier.Instance.CurMap.GetType().GetProperty(valuename);
            System.Collections.IList test = (System.Collections.IList)info.GetValue(MapModifier.Instance.CurMap, null);
            test.Add(obj);
        }
    }

    public void Do()
    {
        if (add == true)
        {
            PropertyInfo info =MapModifier.Instance.CurMap.GetType().GetProperty(valuename);
            System.Collections.IList test = (System.Collections.IList)info.GetValue(MapModifier.Instance.CurMap, null);
            test.Add(obj);

        }
        else
        {
            PropertyInfo info = MapModifier.Instance.CurMap.GetType().GetProperty(valuename);
            System.Collections.IList test = (System.Collections.IList)info.GetValue(MapModifier.Instance.CurMap, null);
            test.Remove(obj);
        }
    }
}


public class ChangePropetyCommand:ICommand
{
    object old;
    object present;
    bool addordelete;
    string valuename;
    public object Old
    {
        get { return old; }
    }
    public string Valuename
    {
        get { return valuename; }
    }
    public object Present
    {
        get { return present; }
    }

    public ChangePropetyCommand(object obj, object newobj, string valuename)
    {
        old = obj;
        present = newobj;
    }

    public void Undo()
    {
        PropertyInfo info = MapModifier.Instance.CurMap.GetType().GetProperty(valuename);
        System.Collections.IList test = (System.Collections.IList)info.GetValue(MapModifier.Instance.CurMap, null);
        test.Remove(present);
        test.Add(old);
    }

    public void Do()
    {
        PropertyInfo info = MapModifier.Instance.CurMap.GetType().GetProperty(valuename);
        System.Collections.IList test = (System.Collections.IList)info.GetValue(MapModifier.Instance.CurMap, null);
        test.Remove(old);
        test.Add(present);
    }
}



public class CommandManager
{
    public Stack<ICommand> UndoStack
    {
        get;
        private set;
    }
    public Stack<ICommand> RedoStack
    {
        get;
        private set;
    }

    private CommandManager()
    {
        UndoStack = new Stack<ICommand>();
        RedoStack = new Stack<ICommand>();
    }

    static CommandManager instance;
    public static CommandManager Instance
    {
        get
        {
            if (instance == null)
                instance = new CommandManager();
            return instance;
        }
    }

    public void Undo()
    {
        if (UndoStack.Count == 0)
            return;
        ICommand currentundo = UndoStack.Pop();
        RedoStack.Push(currentundo);
        currentundo.Undo();


    }

    public void Do()
    {
        if (RedoStack.Count == 0)
            return;
        ICommand currentundo = RedoStack.Pop();
        UndoStack.Push(currentundo);
        currentundo.Do();


    }

    public void AddNewCommand(ICommand cmd)
    {
        UndoStack.Push(cmd);
        RedoStack.Clear();
        //cmd.Do();
    }

}
