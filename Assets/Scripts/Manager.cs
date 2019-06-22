using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSegment
{
    public Vector3 from;
    public Vector3 to;

    public LineSegment(Vector3 f, Vector3 t)
    {
        this.from = f;
        this.to = t;
    }

    public void Draw()
    {
        Gizmos.DrawLine(from, to);
    }

    public override bool Equals(object obj)
    {
        if (obj is LineSegment)
        {
            LineSegment tmp = (LineSegment)obj;
            return this == tmp;
        }

        return false;
    }

    public static bool operator ==(LineSegment a, LineSegment b)
    {
        if ((a.from == b.from && a.to == b.to) || (a.to == b.from && a.from == b.to))
            return true;
        return false;
    }

    public static bool operator !=(LineSegment a, LineSegment b)
    {
        return !(a == b);
    }
}

public class Triangle
{
    public Vector3 a;
    public Vector3 b;
    public Vector3 c;

    public Triangle(Vector3 a, Vector3 b, Vector3 c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
    }

    public void Draw()
    {
        Vector3 center = Utils.NeiQieYuan(a.x, a.y, b.x, b.y, c.x, c.y);

        Vector3 acenter = (a - b) * 0.5f;
        Vector3 bcenter = (b - c) * 0.5f;
        Vector3 ccenter = (a - c) * 0.5f;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(center, a);
        Gizmos.DrawLine(center, b);
        Gizmos.DrawLine(center, c);

        Gizmos.DrawLine(center, acenter);
        Gizmos.DrawLine(center, bcenter);
        Gizmos.DrawLine(center, ccenter);
    }

}

public class Manager : MonoBehaviour
{
    public List<Area> areas = new List<Area>();

    private float left = float.MaxValue;
    private float right = float.MinValue;
    private float top = float.MinValue;
    private float bottom = float.MaxValue;
    private List<LineSegment> lineSegments = new List<LineSegment>();
    private List<Triangle> triangles = new List<Triangle>();

    private void Start()
    {
        areas.Clear();
    }

    public void StartDraw()
    {
        for (int i=0; i < areas.Count; ++i)
        {
            Area area = areas[i];
            Debug.Log(area.ToString());
            if (area.pos.x < left) left = area.pos.x;
            if (area.pos.y < bottom) bottom = area.pos.y;
            if (area.pos.x > right) right = area.pos.x;
            if (area.pos.y > top) top = area.pos.y;
        }

        left -= 2;
        right += 2;
        bottom -= 2;
        top += 2;
    }

    public void Clear()
    {
        areas.Clear();
    }
	
	void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AddPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
	}

    private void AddPoint(Vector3 pos)
    {
        areas.Add(new Area(pos));
    }

    private void OnDrawGizmos()
    {
        if (areas.Count < 4) return;

        //Gizmos.color = Color.green;
        //Gizmos.DrawSphere(Vector3.zero, 0.05f);
        Gizmos.color = Color.red;
        for (int i = 0; i < areas.Count; ++i)
        {
            areas[i].Draw();
        }

        Vector3 right_bottom = new Vector3(right, bottom);
        Vector3 right_top = new Vector3(right, top);
        Vector3 left_top = new Vector3(left, top);
        Vector3 left_bottom = new Vector3(left, bottom);

        Debug.Log("right_bottom : " + right_bottom.ToString());
        Debug.Log("right_top : " + right_top.ToString());
        Debug.Log("left_top : " + left_top.ToString());
        Debug.Log("left_bottom : " + left_bottom.ToString());

        Gizmos.DrawLine(right_bottom, right_top);
        Gizmos.DrawLine(right_top, left_top);
        Gizmos.DrawLine(left_top, left_bottom);
        Gizmos.DrawLine(left_bottom, right_bottom);

        //Vector3 rightCenterPos = (right_top - right_bottom) * 0.5f;
        //Vector3 topCenterPos = (left_top - right_top) * 0.5f;
        //Vector3 leftCenterPos = (left_bottom - left_top) * 0.5f;
        //Vector3 bottomCenterPos = (right_bottom - left_bottom) * 0.5f;

        //List<Vector3> centers = new List<Vector3>();
        //centers.Add(rightCenterPos);
        //centers.Add(topCenterPos);
        //centers.Add(leftCenterPos);
        //centers.Add(bottomCenterPos);

        Gizmos.color = Color.black;
        DrawTriangle(right_bottom, right_top);
        DrawTriangle(right_top, left_top);
        DrawTriangle(left_top, left_bottom);
        DrawTriangle(left_bottom, right_bottom);
        Debug.Log("line : " + lineSegments.Count);

        for (int i=0; i < lineSegments.Count; ++i)
        {
            lineSegments[i].Draw();
        }

        Debug.Log("Triangle : " + triangles.Count);
        for (int i =0; i < triangles.Count; ++i)
        {
            triangles[i].Draw();
        }
    }

    private void DrawTriangle(Vector3 from, Vector3 to)
    {
        float distance = float.MaxValue;
        Area a = null;
        Vector3 center = (to - from) * 0.5f;
        for (int i =0; i < areas.Count; ++i)
        {
            Area area = areas[i];
            if (area.pos == from || area.pos == to) continue;

            float tmpDistance = Vector3.Distance(area.pos, center);
            if (tmpDistance < distance)
            {
                distance = tmpDistance;
                a = area;
            }
        }

        if (a != null)
        {
            LineSegment ls1 = new LineSegment(from, a.pos);
            bool has1 = GetLineSegments(ls1);
            if (!has1)
            {
                lineSegments.Add(ls1);
                DrawTriangle(from, a.pos);
            }

            LineSegment ls2 = new LineSegment(a.pos, to);
            bool has2 = GetLineSegments(ls2);
            if (!has2)
            {
                lineSegments.Add(ls2);
                DrawTriangle(a.pos, to);
            }

            if (!has1 || !has2)
            {
                bool a1 = IsInArea(a.pos);
                bool f1 = IsInArea(from);
                bool t1 = IsInArea(to);

                if (a1 && f1 && t1)
                {
                    Triangle t = new Triangle(a.pos, from, to);
                    triangles.Add(t);
                }
            }
        }
    }

    private bool IsInArea(Vector3 pos)
    {
        for (int i=0; i < areas.Count; ++i)
        {
            if (areas[i].pos == pos)
                return true;
        }

        return false;
    }

    private bool GetLineSegments(LineSegment value)
    {
        for (int i=0; i < lineSegments.Count; ++i)
        {
            if (lineSegments[i] == value) return true;
        }

        return false;
    }
}

public static class Utils
{
    public static Vector3 NeiQieYuan(float m0, float n0, float m1, float n1, float m2, float n2)
    {
        float px;
        float py;
        float pr;

        float dax = 0;
        float day = 0;

        float dbx = 0;
        float dby = 0;

        float absA = 0.0f;
        float absB = 0.0f;
        float temp = 0;

        dax = m0 - m1;
        day = n0 - n1;

        dbx = m2 - m1;
        dby = n2 - n1;

        temp = dax * dax + day * day * 1.0f;
        absA = Mathf.Sqrt(temp);
        temp = dbx * dbx + dby * dby * 1.0f;
        absB = Mathf.Sqrt(temp);

        // (absB * day - absA * dby)(y - n1) = (absA * dbx - absB * dax)(x - m1)

        // 第一个角平分线方程
        // a(y - n1) = b(x - m1)


        // 方程1 
        float a = 0.0f;
        float b = 0.0f;

        a = (absB * day - absA * dby);
        b = (absA * dbx - absB * dax);



        dax = m0 - m2;
        day = n0 - n2;

        dbx = m1 - m2;
        dby = n1 - n2;

        temp = dax * dax + day * day * 1.0f;
        absA = Mathf.Sqrt(temp);
        temp = dbx * dbx + dby * dby * 1.0f;
        absB = Mathf.Sqrt(temp);

        float c = 0.0f;
        float d = 0.0f;

        c = (absB * day - absA * dby);
        d = (absA * dbx - absB * dax);
        // 第二个角平分线方程
        // c(y - n2) = d(x - m2)
        float PointX = 0.0f;
        float PointY = 0.0f;


        if (a != 0)
        {
            PointX = (c * b * m1 + n2 * a * c - n1 * a * c - a * d * m2) / (c * b - a * d);
            PointY = b * (PointX - m1) / a + n1;

        }
        else
        {
            PointX = m1;
            PointY = d * (m1 - m2) / c + n2;
        }

        // dax * (y - n2) = day * (x - m2)

        // 点到直线的方程 (-day)(y - PointY) = (dax)(x - PointX)

        // 计算点到直线的距离

        float intersectionX = 0.0f;
        float intersectionY = 0.0f;

        if (dax != 0)
        {
            intersectionX = (day * day * m2 - day * dax * n2 + day * dax * PointY + dax * dax * PointX) / (dax * dax + day * day);
            intersectionY = day * (intersectionX - m2) / dax + n2;

        }
        else
        {
            intersectionX = m2;
            intersectionY = dax * (intersectionX - PointX) / (-day) + PointY;
        }

        px = PointX;
        py = PointY;

        float temp1 = (intersectionX - PointX) * (intersectionX - PointX) + (intersectionY - PointY) * (intersectionY - PointY);
        pr = Mathf.Sqrt((intersectionX - PointX) * (intersectionX - PointX) + (intersectionY - PointY) * (intersectionY - PointY));

        return new Vector3(px, py, 0);

    }
}

