using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tilemap : MonoBehaviour {

    public int width = 10;
    public int length = 1;
    List<Vector3> positions;
    private castline cl;
    List<int> unreachable;

	// Use this for initialization
	void Start () {
        cl = new castline();
        unreachable = new List<int>();
        positions = new List<Vector3>();
        InitData();
     //   Detect();
    }
	
    void InitData()

    {
        for(int i = 0; i<10;++i)
            for(int j = 0;j<10; ++j)
            {
                Vector3 pos = new Vector3((float)(i - 4.5) * 1, 1000, (float)(j - 5.5) * 1);
                positions.Add(pos);
            }
    }

    //void Detect()
    //{
    //    int i = 0;
    //    foreach(var p in positions)
    //    {
            
    //        bool res = cl.CastLine(p);
    //        if(res == false)
    //        {
    //            Record(i);
    //        }
    //        i++;
    //    }

    //    Debug.Log(unreachable.Count);
    //}

	// Update is called once per frame
	void Update () {
        //Debug.Log(unreachable.Count);
    }

    void Record(int i)
    {
        unreachable.Add(i);
    }

    public static List<int> Detect(List<Vector3> pos,Vector3 dir,float max)
    {
        int i = 0;
        List<int> unreachable = new List<int>();
        foreach (var p in pos)
        {

            bool res = CastLine(p,dir,max);
            if (res == false)
            {
                unreachable.Add(i);
            }
            i++;
        }

        return unreachable;

    }

    public static bool CastLine(Vector3 pos,Vector3 dir,float max)
    {
        RaycastHit hit;
        bool flag = false;
        if (Physics.Raycast(pos, dir, out hit))
        {
            float depth = hit.collider.gameObject.transform.position.y;
            if (depth <= max)
            {
                flag = true;
            }

        }
        return flag;
    }


}
