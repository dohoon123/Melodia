using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GenerationData", menuName = "GenerationData")]
public class GenerationDataSO : ScriptableObject
{
    public int minWidth = 16;
    public int maxWidth = 128;

    public int minHeight = 16;
    public int maxHeight = 128;
}
