using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    RectTransform rectTransform;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate() {
        rectTransform.position = Camera.main.WorldToScreenPoint(GameManager.instance.player.transform.position);
    }
}
