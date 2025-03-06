using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorInitialize : MonoBehaviour
{
    [Header("Aesthetic")]
    public SpriteRenderer[] maskSPRITE;
    public Image[] maskIMAGE;
    static Dictionary<string, Color> Col = new Dictionary<string, Color>() {
        { "Black", new Color(0f, 0f, 0f)},
        { "Red", new Color(1f, 0f, 0f)},
        { "Green", new Color(0f, 1f, 0f)}
    };
    private float flashTime, flashDuration = 0.3f;

    public static Color SetColor(string col) {
        return Col[col];
    }

    public void InvokeColor(string c, float maxAlpha) {
        InvokeColor(c, maxAlpha, flashDuration);
    }

    public void InvokeColor(string c, float maxAlpha, float duration) {
        StopAllCoroutines();
        StartCoroutine(FlashColor(c, maxAlpha, duration));
    }

    private IEnumerator FlashColor(string c, float maxAlpha, float duration) {
        if (maskSPRITE != null) {
            flashTime = 0f;
            Color col = Col[c];
            while (col.a > 0f) {
                col.a = Mathf.Lerp(maxAlpha, 0f, flashTime / duration);
                for (int i = 0; i < maskSPRITE.Length; i++) maskSPRITE[i].color = col;

                flashTime += Time.deltaTime;
                yield return null;
            }
            col.a = 0f;
            for (int i = 0; i < maskSPRITE.Length; i++) maskSPRITE[i].color = col;
        } else {
            flashTime = 0f;
            Color col = Col[c];
            while (col.a > 0f) {
                col.a = Mathf.Lerp(maxAlpha, 0f, flashTime / duration);
                for (int i = 0; i < maskIMAGE.Length; i++) maskIMAGE[i].color = col;

                flashTime += Time.deltaTime;
                yield return null;
            }
            col.a = 0f;
            for (int i = 0; i < maskIMAGE.Length; i++) maskIMAGE[i].color = col;
        }

        
    }
}
