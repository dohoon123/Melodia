using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SheetData", fileName = "New SheetData")]
public class SheetDataSO : ScriptableObject {
    

    public int playSpeed = 1;
    public Sheet sheet;
}

[Serializable]
public class MelodyAndRhythm { 
    public int melody = 0;
    public int rhythm = 0;
}

[Serializable]
public class Sheet {
    public List<MelodyAndRhythm> notes;
}
