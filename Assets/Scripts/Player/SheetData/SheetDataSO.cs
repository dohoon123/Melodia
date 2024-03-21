using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SheetData", fileName = "New SheetData")]
public class SheetDataSO : ScriptableObject {
    public float playSpeed = 0.5f;
    public Sheet sheet;
}

[Serializable]
public class Note { 
    public int melody = 0;
    public int rhythm = 0;
}

[Serializable]
public class Sheet {
    public List<Note> notes;
}
