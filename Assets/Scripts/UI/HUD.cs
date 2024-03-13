using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { Level, Health }
    [SerializeField] InfoType type;
    [SerializeField] Health playerHealth;
    Slider mySlider;

     private void Awake() {
        mySlider = GetComponent<Slider>();
        mySlider.maxValue = playerHealth.GetMaxHP();
    }

    private void LateUpdate() {
        switch (type)
        {
            case InfoType.Health:
                mySlider.value = playerHealth.GetCurrentHP();
                Debug.Log(mySlider.value);
                break;
            case InfoType.Level:
                break;
        }
    }

}
