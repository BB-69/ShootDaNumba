using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorInitialize : MonoBehaviour
{
    [Header("Aesthetic")]
    public SpriteRenderer[] mask;
    Dictionary<string, Color> Col = new Dictionary<string, Color>() {
        { "Red", new Color(1f, 0f, 0f)},
        { "Green", new Color(0f, 1f, 0f)}
    };
    private float flashTime, flashDuration = 0.3f;

    public void InvokeColor(string c, float maxAlpha) {
        InvokeColor(c, maxAlpha, flashDuration);
    }

    public void InvokeColor(string c, float maxAlpha, float duration) {
        StopAllCoroutines();
        StartCoroutine(FlashColor(c, maxAlpha, duration));
    }

    private IEnumerator FlashColor(string c, float maxAlpha, float duration) {
        flashTime = 0f;
        Color col = Col[c];
        while (col.a > 0f) {
            col.a = Mathf.Lerp(maxAlpha, 0f, flashTime / duration);
            for (int i = 0; i < mask.Length; i++) mask[i].color = col;

            flashTime += Time.deltaTime;
            yield return null;
        }
        col.a = 0f;
        for (int i = 0; i < mask.Length; i++) mask[i].color = col;
    }
}
