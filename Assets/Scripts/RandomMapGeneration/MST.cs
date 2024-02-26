using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MST
{
    List<Triangle> triangleList = new List<Triangle>();
    List<List<Vector2>> path = new List<List<Vector2>>();
    List<List<Vector2>> segments = new List<List<Vector2>>();
    Dictionary<Vector2, Vector2> parents = new Dictionary<Vector2, Vector2>();


    public void SetTriangleList(List<Triangle> list){
        triangleList = list.ToList();
    }

    int Sort(List<Vector2> A, List<Vector2> B) {
        if (Vector2.Distance(A[0], A[1]) >= Vector2.Distance(B[0], B[1])) {
            return 1; 
        }else{
            return -1;
        }
    }

    public List<List<Vector2>> GetMST(List<Triangle> list){
        SetTriangleList(list);
        SegmentListFromTriangleList();
        InitializeParentList();

        segments.Sort(Sort);

        foreach (List<Vector2> segment in segments) {
            if (GetParent(segment[0]) != GetParent(segment[1])) {
                parents[GetParent(segment[1])] = GetParent(segment[0]);
                path.Add(segment);
            }
        }

        return path;
    }

     private Vector2 GetParent(Vector2 index){
        if (index == parents[index]) {
            return index;
        }else {
            return GetParent(parents[index]);
        }
    }

   private void InitializeParentList() {
        foreach (List<Vector2> segment in segments) {
            for (int i = 0; i < segments.Count; i++) {
                parents.TryAdd(segment[0], segment[0]);
                parents.TryAdd(segment[1], segment[1]);
            }
        }
    }

    private void SegmentListFromTriangleList() {
        foreach (Triangle triangle in triangleList) {
            TriangleToSegments(triangle.GetPosA(), triangle.GetPosB());
            TriangleToSegments(triangle.GetPosB(), triangle.GetPosC());
            TriangleToSegments(triangle.GetPosC(), triangle.GetPosA());
        }
    }

    private void TriangleToSegments(Vector2 currentVertex, Vector2 targetVertex) {
        List<Vector2> tempList = new List<Vector2>();
        tempList.Add(currentVertex);
        tempList.Add(targetVertex);

        segments.Add(tempList);
    }

    public void DrawPath(){
        foreach (List<Vector2> pair in path) {
            Debug.DrawLine(pair[0], pair[1]);
        }
    }
}
