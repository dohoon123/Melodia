using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Mathematics
{
    public static void GetLineFromPoints(Vector2 P, Vector2 Q, ref float a, ref float b, ref float c){
        a = Q[1] - P[1];
        b = P[0] - Q[0];
        c = a * (P[0]) + b * (P[1]);
    }

    public static void GetPerpendicularBisectorFromLine(Vector2 P, Vector2 Q, ref float a, ref float b, ref float c){
        Vector2 mid_point = new((P[0] + Q[0]) / 2, (P[1] + Q[1])/2);

        c = -b * (mid_point[0]) + a * (mid_point[1]);
    
        float temp = a;
        a = -b;
        b = temp;
    }

    public static Vector2 LineLineIntersection(float a1, float b1, float c1,
                            float a2, float b2, float c2)
    {
        float determinant = a1 * b2 - a2 * b1;
        if (determinant == 0) {
            // The lines are parallel. This is simplified
            // by returning a pair of FLT_MAX
            return new Vector2(float.MaxValue, float.MaxValue);
        }else {
            float x = (b2 * c1 - b1 * c2) / determinant;
            float y = (a1 * c2 - a2 * c1) / determinant;
            return new Vector2(x, y);
        }
    }

    public static bool isDotInRectangle(Vector2 dot, Vector2 center, float width, float height){
        
        return true;
    }
}
