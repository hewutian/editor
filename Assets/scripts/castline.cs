using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class castline : MonoBehaviour {

    //Vector3 position;
    Vector3 dir = new Vector3(0,-1,0);
    float length = 1000;
    float max = 5;
	// Use this for initialization
		
    public bool CastLine(Vector3 pos)
    {
        RaycastHit hit;
        bool flag = false;
        if (Physics.Raycast(pos,dir,out hit,length))
        {
            float depth = hit.collider.gameObject.transform.position.y;
            if(depth <= max)
            {
                flag = true;
            }

        }
        return flag;
    }

}
