using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponParent : MonoBehaviour 
{
    public Vector2 pointerPosition { get; set; }

    [SerializeField] SpriteRenderer weaponSpriteRenderer;
    [SerializeField] SpriteRenderer playerSpriteRenderer;

    private void Update() {
        Vector2 direction = (pointerPosition - (Vector2)transform.position).normalized;
        transform.right = direction;

        Vector2 scale = transform.localScale;

        if (direction.x < 0) {
            scale.y = -1;
        }else if (direction.x > 0) {
            scale.y = 1;
        }

        transform.localScale = scale;

        HideWeaponBehindPlayer();
    }

    void HideWeaponBehindPlayer() {
        if (transform.eulerAngles.z > 0 && transform.eulerAngles.z < 180) {
            weaponSpriteRenderer.sortingOrder = playerSpriteRenderer.sortingOrder - 1;
        }else {
            weaponSpriteRenderer.sortingOrder = playerSpriteRenderer.sortingOrder + 1;
        }
    }
}
