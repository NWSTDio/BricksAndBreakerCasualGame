using System.Collections.Generic;
using UnityEngine;
public static class Utils
    {
    public static void Log(this List<Vector3> vector)
        {
        Debug.Log("<color=yellow>Vector</color>");
        foreach (var v in vector)
            Debug.Log(v);
        }

    private static bool IsParalel(float a1, float a2, float b1, float b2)
        {
        if ((a1 / a2) == (b1 / b2))
            return true;
        return false;
        }
    private static Vector3 CalculateIntersection(float a1, float a2, float b1, float b2, float c1, float c2)
        {
        Vector3 pos = Vector3.zero;
        float det = a1 * b2 - a2 * b1;
        pos.x = ((b1 * c2 - b2 * c1) / det);
        pos.y = ((a2 * c1 - a1 * c2) / det);
        return pos;
        }

    public static Vector3 GetIntersectionPosition(Vector2 line1A, Vector2 line1B, Vector2 line2A, Vector2 line2B)
        {
        float a1, a2, b1, b2, c1, c2;

        // line equation ax + by + c = 0
        a1 = line1A.y - line1B.y;
        b1 = line1B.x - line1A.x;
        c1 = line1A.x * line1B.y - line1B.x * line1A.y;

        a2 = line2A.y - line2B.y;
        b2 = line2B.x - line2A.x;
        c2 = line2A.x * line2B.y - line2B.x * line2A.y;

        if (IsParalel(a1, a2, b1, b2))
            return Vector3.zero;

        return CalculateIntersection(a1, a2, b1, b2, c1, c2);
        }

    //public static bool IsIntersection(Vector2 line1_a, Vector2 line1_b, Vector2 line2_a, Vector2 line2_b)
    //    {
    //    if(line1_a.x >= line1_b.x)
    //        {
    //        float x = line1_b.x;
    //        line1_b.x = line1_a.x;
    //        line1_a.x = x;
    //        float y = line1_b.y;
    //        line1_b.y = line1_a.y;
    //        line1_a.y = y;
    //        }

    //    if (line2_a.x >= line2_b.x)
    //        {
    //        float x = line2_b.x;
    //        line2_b.x = line2_a.x;
    //        line2_a.x = x;
    //        float y = line2_b.y;
    //        line2_b.y = line2_a.y;
    //        line2_a.y = y;
    //        }

    //    float k1;// угловой коефициент 
    //    if (line1_a.y == line1_b.y)
    //        k1 = 0;
    //    else
    //        k1 = (line1_b.y - line1_a.y) / (line1_b.x - line1_a.x);

    //    float k2;// угловой коефициент 
    //    if (line2_a.y == line2_b.y)
    //        k2 = 0;
    //    else
    //        k2 = (line2_b.y - line2_a.y) / (line2_b.x - line2_a.x);

    //    if (k1 == k2) // отрезки паралельны
    //        return false;

    //    float b1 = line1_a.y - k1 * line1_a.x;
    //    float b2 = line2_a.y - k2 * line2_a.x;
    //    return false;
    //    }
    public static bool Not(this bool b) => b == false;

    private static bool RangeIntersection(float a, float b, float c, float d)
        {
        if (a > b)
            {
            float temp = a;
            a = b;
            b = temp;
            }
        if (c > d)
            {
            float temp = c;
            c = d;
            d = temp;
            }
        return Mathf.Max(a, c) <= Mathf.Min(b, d);
        }

    private static bool IsBoundingBox(Segment a, Segment b) => 
            RangeIntersection(a.Start.x, a.End.x, b.Start.x, b.End.x) && 
            RangeIntersection(a.Start.y, a.End.y, b.Start.y, b.End.y);

    public static float CrossProduct(Segment a, Segment b) => a.XComponent * b.YComponent - b.XComponent * a.YComponent;

    public static bool FastCheckSegmentIntersection(Segment a, Segment b)
        {
        if (IsBoundingBox(a, b).Not())
            return false;

        var vAB = new Segment(a.Start, a.End);
        var vAC = new Segment(a.Start, b.Start);
        var vAD = new Segment(a.Start, b.End);

        var vCD = new Segment(b.Start, b.End);
        var vCA = new Segment(b.Start, a.Start);
        var vCB = new Segment(b.Start, a.End);

        float d1 = CrossProduct(vAB, vAC);
        float d2 = CrossProduct(vAB, vAD);
        float d3 = CrossProduct(vCD, vCA);
        float d4 = CrossProduct(vCD, vCB);

        return ((d1 <= 0 && d2 >= 0) || (d1 >= 0 && d2 <= 0)) && ((d3 <= 0 && d4 >= 0) || (d3 >= 0 && d4 <= 0));
        }

    public static void DrawCross(Vector3 pos, float length, Color color)
        {
        Debug.DrawLine(pos + (Vector3.left * length), pos + (Vector3.right * length), color);
        Debug.DrawLine(pos + (Vector3.down * length), pos + (Vector3.up * length), color);
        }
    }