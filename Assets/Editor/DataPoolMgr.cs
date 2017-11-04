using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class DPM
{

    private static DPM instance = new DPM();
    public static DPM Instance
    {
        get { return instance; }
    }

    public DataPool BuildingsDP = null;

    public bool Init()
    {
        if (!initElementData())
            return false;
        return true;
    }

    public void Release()
    {
        BuildingsDP.Release();
    }

    bool initElementData()
    {

        BuildingsDP = new DataPool();
        bool retB = BuildingsDP.LoadDPCFile("DataPool/build.dpc", DataPool.DataPoolLoad.ReadOnly);
       // var rettest = BuildingsDP.LoadFromPackage("build.dpc", DataPool.DataPoolLoad.ReadOnly);
        SetKeyOfArray(BuildingsDP, "build", "build", "id");
        return retB;
        // return retE && retD;
    }

    static DataPoolVariable[] getEnumMember(DataPoolIterator itBegin, DataPoolIterator itEnd)
    {
        List<DataPoolVariable> list = new List<DataPoolVariable>();

        for (DataPoolIterator it = itBegin; it != itEnd; ++it)
        {
            DataPoolVariable var = it.ToVariable();
            list.Add(var);
        }
        return list.ToArray();
    }

    static DataPoolVariable[] EnumArray(DataPoolElementIterator itFirst, DataPoolElementIterator itLast)
    {
        List<DataPoolVariable> list = new List<DataPoolVariable>();
        // 这个地方要克隆，否则itFirst和it是一个地址的引用
        for (DataPoolElementIterator it = itFirst.Clone(); it != itLast; ++it)
        {
            DataPoolVariable var = it.ToVariable();
            list.Add(var);
        }
        return list.ToArray();
    }

    public static DataPoolVariable[] GetArray(DataPoolVariable var)
    {
        if (var.IsValid())
            return EnumArray(var.array_begin(), var.array_end());
        return null;
    }

    public static DataPoolVariable[] GetMember(DataPoolVariable var)
    {
        if (var.IsValid())
            return getEnumMember(var.begin(), var.end());
        return null;
    }

    public static DataPoolVariable FindByKey(DataPool pool, string strType, int id)
    {
        DataPoolVariable var = null;
        if (pool != null)
            var = pool.FindByKey(strType, id);
        return var;
    }

    public static void SetKeyOfArray(DataPool pool, string strType, string strTemplate, string strKey)
    {
        if (pool != null)
        {
            pool.SetKeyOfArray(strType, strTemplate, strKey);
        }
        else
            Debug.LogError("There is no datapool to setKeyofArray");
    }


    public static void PrintData(DataPoolVariable src)
    {
        for (DataPoolIterator it = src.begin(); it != src.end(); ++it)
        {
            DataPoolVariable var = it.ToVariable();
            DataPool.TypeCategory type = var.GetTypeCategory();
            if (type == DataPool.TypeCategory.STRING)
                Debug.Log(var.GetTypeCategory() + " " + var.VariableName() + "=" + var);
            else if (type == DataPool.TypeCategory.FLOAT)
                Debug.Log(var.GetTypeCategory() + " " + var.VariableName() + "=" + var);
            else if (type == DataPool.TypeCategory.DWORD)
                Debug.Log(var.GetTypeCategory() + " " + var.VariableName() + "=" + var);
            else if (type == DataPool.TypeCategory.STRING)
                Debug.Log(var.GetTypeCategory() + " " + var.VariableName() + "=" + var);
            else if (type == DataPool.TypeCategory.SDWORD)
                Debug.Log(var.GetTypeCategory() + " " + var.VariableName() + "=" + var);
            else if (type == DataPool.TypeCategory.STRUCT)
                PrintData(var);
        }
    }

}


