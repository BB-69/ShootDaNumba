using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public ObjectPool objPool;
    private RectTransform rect;
    private SpriteRenderer rend;
    private Vector2 startScale, endScale;
    private float startAlpha, endAlpha;

    private bool justStart = true;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        rend = GetComponent<SpriteRenderer>();

        startScale = rect.localScale;
        endScale = startScale * 2f;
        startAlpha = 1f; endAlpha = 0f;

        Color col = rend.color;
        col.a = endAlpha;
        rend.color = col;

        justStart = false;
    }

    void OnEnable()
    {
        if (justStart) return;
        StartCoroutine(Explode());
    }

    IEnumerator Explode()
    {
        float time = 0f, duration = 0.2f;

        Color col = rend.color;
        col.a = startAlpha;
        rect.localScale = startScale;

        while (time < duration) {
            col.a = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            rend.color = col;

            rect.localScale = Vector2.Lerp(startScale, endScale, time / duration);

            time += Time.deltaTime;
            yield return null;
        }

        col.a = endAlpha;
        rect.localScale = endScale;
        Disappear();
    }

    void Disappear()
    {
        gameObject.SetActive(false); objPool.activeObj--;
    }
}
