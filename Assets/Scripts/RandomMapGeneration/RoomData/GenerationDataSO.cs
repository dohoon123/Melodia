using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GenerationData", menuName = "GenerationData")]
public class GenerationDataSO : ScriptableObject
{
    public int minWidth = 2;
    public int maxWidth = 12;

    public int minHeight = 2;
    public int maxHeight = 12;
}
