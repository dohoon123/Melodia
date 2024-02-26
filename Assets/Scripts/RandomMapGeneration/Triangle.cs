using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Triangle
{
    private Vector2 posA = new Vector2();
    private Vector2 posB = new Vector2();
    private Vector2 posC = new Vector2();

    private Vector2 circumCenter = new Vector2();

    float circumCenterRadius = 0;

    public override bool Equals(object obj)
    {
        if (obj == null) {
            return false;
        }
        else{
            Triangle target = (Triangle) obj;
            return (posA == target.GetPosA()) && (posB == target.GetPosB()) && (posC == target.GetPosC());
        }
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }

    int Sort(Vector2 A, Vector2 B) {
        if (A.x > B.x) return 1;
        else if (A.x < B.x) return -1;
        else {
            if (A.y >= B.y) return 1;
            else return -1;
        }
    }

    public void SetVertices(List<Vector2> position){
        if (position.Count != 3) return;      

        position.Sort(Sort);

        posA.Set(position[0].x, position[0].y);
        posB.Set(position[1].x, position[1].y);
        posC.Set(position[2].x, position[2].y);

        SetCircumCenter();

        circumCenterRadius = Vector2.Distance(circumCenter, posA);
    }


    private void SetCircumCenter() {

        float a = 0, b = 0, c = 0;
        float d = 0, e = 0, f = 0;

        Mathematics.GetLineFromPoints(posA, posB, ref a, ref b, ref c);
        Mathematics.GetLineFromPoints(posB, posC, ref d, ref e, ref f);

        Mathematics.GetPerpendicularBisectorFromLine(posA, posB, ref a, ref b, ref c);
        Mathematics.GetPerpendicularBisectorFromLine(posB, posC, ref d, ref e, ref f);

        circumCenter = Mathematics.LineLineIntersection(a, b, c, d, e, f);
    }

    public List<Vector2> GetVertices() {
        List<Vector2> vertices = new List<Vector2>();
        vertices.Add(posA);
        vertices.Add(posB);
        vertices.Add(posC);

        return vertices;
    }

    public void SetEquilateralTriangle(float length) {
        List<Vector2> positions = new List<Vector2>();
        positions.Add(new Vector2(0, 2 * length));
        positions.Add(new Vector2(Mathf.Floor(-length * Mathf.Sqrt(3)), -length));
        positions.Add(new Vector2(Mathf.Floor(length * Mathf.Sqrt(3)), -length));

        SetVertices(positions);
    }

    float Sign(Vector2 p1, Vector2 p2, Vector2 p3) {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }

    public bool IsDotInTriangle(Vector2 dot) {
        float d1, d2, d3;
        bool has_neg, has_pos;

        d1 = Sign(dot, posA, posB);
        d2 = Sign(dot, posB, posC);
        d3 = Sign(dot, posC, posA);

        has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

        return !(has_neg && has_pos);
    }

    public int FindVerticeToConnectExternalDot(Vector2 dot){
        float dA = Sign(dot, posB, posA) * Sign(dot, posC, posA);
        float dB = Sign(dot, posA, posB) * Sign(dot, posC, posB);
        float dC = Sign(dot, posB, posC) * Sign(dot, posA, posC);

        
        dA = Mathf.Clamp(dA, -1.0f, 1.0f);
        dB = Mathf.Clamp(dB, -1.0f, 1.0f);
        dC = Mathf.Clamp(dC, -1.0f, 1.0f);

        if (dA < 0) { return 0; }
        if (dB < 0) { return 1; }
        if (dC < 0) { return 2; }

        return 5;
    }

    public Vector2 FindMaxAnglePoint(){
        float dA, dB, dC;

        dA = CalcAngle(posA, posB, posC);
        dB = CalcAngle(posB, posC, posA);
        dC = CalcAngle(posC, posA, posB);
        
        float maxAngle = Mathf.Max(Mathf.Max(dA, dB), dC);

        if (maxAngle == dA) return posA; 
        if (maxAngle == dB) return posB; 
        if (maxAngle == dC) return posC; 

        return new Vector2(0, 0);
    }

    float CalcAngle(Vector2 main, Vector2 v1, Vector2 v2){

        Vector2 nv1 = (v1 - main); nv1.Normalize();
        Vector2 nv2 = (v2 - main); nv2.Normalize();

        float dot = Vector3.Dot(nv1, nv2);
        float angle = Mathf.Acos(dot);

        return angle;
    }

    public void DrawTriangle(){
        Debug.DrawLine(posA, posB);
        Debug.DrawLine(posB, posC);
        Debug.DrawLine(posA, posC);
    }

    public void DrawTriangle(float duration){
        Debug.DrawLine(posA, posB, Color.white, duration);
        Debug.DrawLine(posB, posC, Color.white, duration);
        Debug.DrawLine(posA, posC, Color.white, duration);
    }

    public float GetCircumCenterRadius() { return circumCenterRadius; }
    public Vector2 GetCircumCenter() { return circumCenter; }
    public Vector2 GetPosA() { return posA; }
    public Vector2 GetPosB() { return posB; }
    public Vector2 GetPosC() { return posC; }
}
