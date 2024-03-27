using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField] protected TilemapVisualizer tilemapVisualizer = null;

    public void GenerateDungeon(){
        tilemapVisualizer.Clear();
        StartCoroutine(RunProceduralGeneration());
    }

    protected abstract IEnumerator RunProceduralGeneration();
}