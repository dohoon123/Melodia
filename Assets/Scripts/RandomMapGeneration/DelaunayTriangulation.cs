using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DelaunayTriangulation
{
    Triangle superTriangle = new Triangle();
    Stack<Triangle> triangles = new Stack<Triangle>();
    List<Triangle> rightTriangles = new List<Triangle>();
    List<Triangle> wrongTriangles = new List<Triangle>();

    List<Vector2> initialDots;
    public int triangleCount = 0;

    public List<Triangle> ConnectDelaunayTriangulation(List<Vector2> dots){
        SetInitalDots(dots);
        SetSuperTriangle();
        AdjustTriangles();
        DeleteFriendOfSuperTriangles();

        return rightTriangles;
    }

    private void DeleteFriendOfSuperTriangles() {
        for (int i = rightTriangles.Count - 1; i >= 0; i--) {
            if (rightTriangles[i].GetPosA() == superTriangle.GetPosA() || rightTriangles[i].GetPosA() == superTriangle.GetPosB() ||rightTriangles[i].GetPosA() == superTriangle.GetPosC()) 
            { rightTriangles.RemoveAt(i); continue; }
            if (rightTriangles[i].GetPosB() == superTriangle.GetPosA() || rightTriangles[i].GetPosB() == superTriangle.GetPosB() ||rightTriangles[i].GetPosB() == superTriangle.GetPosC()) 
            { rightTriangles.RemoveAt(i); continue; }
            if (rightTriangles[i].GetPosC() == superTriangle.GetPosA() || rightTriangles[i].GetPosC() == superTriangle.GetPosB() ||rightTriangles[i].GetPosC() == superTriangle.GetPosC()) 
            { rightTriangles.RemoveAt(i); continue; }
        }
    }

    public void SetInitalDots(List<Vector2> dots) {
        initialDots = dots.ToList();
    }

    private void SetSuperTriangle()
    {
        float longestDistance = 0.0f;
        Vector2 startPoint = new Vector2(0, 0);

        foreach (Vector2 dot in initialDots)
        {
            float currentDistance = Vector2.Distance(dot, startPoint);
            if (currentDistance > longestDistance) {
                longestDistance = currentDistance;
            }
        }
        
        longestDistance = Mathf.Floor(longestDistance * 1.2f);

        superTriangle.SetEquilateralTriangle(longestDistance); 
        triangles.Push(superTriangle);

        initialDots.Add(superTriangle.GetPosA());
        initialDots.Add(superTriangle.GetPosB());
        initialDots.Add(superTriangle.GetPosC());
    }

    public void AdjustTriangles() {
        while (triangles.Count != 0) {
            Triangle tempTriangle = triangles.Pop();

            if (IsInWrongTriangles(tempTriangle)) continue;

            bool isRightTriangle = true;

            foreach (Vector2 dot in initialDots) {
                if (IsDotVertexOfTriangle(tempTriangle, dot)) { continue; }
                if (IsDotInCircumcircle(tempTriangle, dot)) {
                    isRightTriangle = false;

                    if (!IsInWrongTriangles(tempTriangle)) wrongTriangles.Add(tempTriangle);

                    //case 1. dot in triangle
                    if (IsDotInTriangle(tempTriangle, dot)) {
                        AddThreeTriangles(tempTriangle, dot);
                    }
                    //case 2. dot out of triangle
                    else {
                        AddTwoTriangles(tempTriangle, dot);
                    }
                    break;
                }
            }

            if (isRightTriangle && !rightTriangles.Contains(tempTriangle)) {
                rightTriangles.Add(tempTriangle);
            }
        }
    }

    private bool IsDotVertexOfTriangle(Triangle triangle, Vector2 dot) {
        if ((triangle.GetPosA().x == dot.x && triangle.GetPosA().y == dot.y) || 
            (triangle.GetPosB().x == dot.x && triangle.GetPosB().y == dot.y) || 
            (triangle.GetPosC().x == dot.x && triangle.GetPosC().y == dot.y) ) 
        {
            return true;
        }
        return false;
    }

    private bool IsDotInCircumcircle(Triangle triangle, Vector2 dot) {
        return triangle.GetCircumCenterRadius() > Vector2.Distance(dot, triangle.GetCircumCenter());
    }

    private bool IsDotInTriangle(Triangle triangle, Vector2 dot) {
        return triangle.IsDotInTriangle(dot);
    }

    private void AddTwoTriangles(Triangle triangle, Vector2 dot)
    {
        List<Vector2> triangleVertices = new List<Vector2> {
            triangle.GetPosA(),
            triangle.GetPosB(),
            triangle.GetPosC()
        };

        // 0 -> posA, 1 -> posB, 2 -> posC
        int verticeNumber= triangle.FindVerticeToConnectExternalDot(dot);

        for (int i = 1; i < 3; i++) {
            Triangle tempTriangle = new Triangle();
            List<Vector2> vertices = new List<Vector2>();

            vertices.Add(dot);
            vertices.Add(triangleVertices[verticeNumber]);
            vertices.Add(triangleVertices[(verticeNumber + i) % 3]);

            tempTriangle.SetVertices(vertices);
            if (!IsInWrongTriangles(tempTriangle)) {
                triangles.Push(tempTriangle);
            }
        }

    }
    private void AddThreeTriangles(Triangle triangle, Vector2 dot)
    {
        List<Vector2> prevVertices = new List<Vector2> {
            triangle.GetPosA(),
            triangle.GetPosB(),
            triangle.GetPosC()
        };

        for (int i = 0 ; i < 3; i++) {
            Triangle tempTriangle = new Triangle();
            List<Vector2> vertices = new List<Vector2> {
                dot,
                prevVertices[i],
                prevVertices[(i + 1) % 3]
            };

            tempTriangle.SetVertices(vertices);
            if (!IsInWrongTriangles(tempTriangle)) {
                triangles.Push(tempTriangle);
            }
        }
    }

    private bool IsInWrongTriangles(Triangle triangle) {
        foreach (Triangle wrongTriangle in wrongTriangles) {
            if (wrongTriangle.Equals(triangle)) {
                return true;
            }
        }
        return false;
    }

    public List<Triangle> GetRightTriangles() {
        return rightTriangles;
    }

    public void DrawRightTriangles(){
        foreach (Triangle triangle in rightTriangles) {
            triangle.DrawTriangle();
        }
    }

    public void DrawTriangles(float duration){
        foreach (Triangle triangle in triangles) {
            triangle.DrawTriangle(duration);
        }
    }

    public void DrawTriangles(){
        foreach (Triangle triangle in triangles) {
            triangle.DrawTriangle(1.0f);
        }
    }
}
