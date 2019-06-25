using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Area
{
    public const int Max_Size = 20;

    public List<Vector3> sides = new List<Vector3>();
    public Point pt;

    public Area(Point p)
    {
        pt = p;
    }

    public override string ToString()
    {
        return string.Format("Area pos : " + pt.pos.ToString());
    }

    public void Draw()
    {
        //Vector3 dir = pos - Vector3.zero;
        //Vector3 center = dir * 0.5f;
        //Ray r = new Ray(center, GetMidperpendicular(dir));
        //Vector3 from = r.GetPoint(10); Vector3 to = r.GetPoint(-10);
        ////Gizmos.DrawRay(r);
        //Gizmos.DrawLine(from, to);
        Gizmos.DrawSphere(pt.pos, 0.05f);
    }

    public Vector3 GetMidperpendicular(Vector3 pos)
    {
        if (pos.y == 0)
        {
            return new Vector3(1, 0, 0);
        }
        else
        {
            return new Vector3(-pos.y / pos.x, 1, 0).normalized;
        }
    }
}
