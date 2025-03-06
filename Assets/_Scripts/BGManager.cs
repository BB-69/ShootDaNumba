using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGManager : MonoBehaviour
{
    public ObjectPool objPool;
    public GameObject[] yellowBG;
    public GameObject[] redBG;
    public GameObject redFilter;

    private float alpha;
    private int level => Level.level;

    void Start()
    {
        redFilter.GetComponent<SpriteRenderer>().enabled = true;
    }

    void Update()
    {
        if (Level.level <= 10) {
            alpha = Mathf.Lerp(0f, 0.4f, (level-1)/19f);
            Color col = redFilter.GetComponent<SpriteRenderer>().color;
            col = new Color(col.r, col.g, col.b, alpha);
            redFilter.GetComponent<SpriteRenderer>().color = col;
        }

        for (int i = 0; i < yellowBG.Length; i++) {
            Vector2 scale = yellowBG[i].GetComponent<RectTransform>().localScale;
            scale.x = (objPool.loopInterval-0.45f) / (Level.originLoop-0.45f) * 1080f;
            Vector2 currentScale = Vector2.Lerp(yellowBG[i].GetComponent<RectTransform>().localScale, scale, 0.5f*Time.deltaTime);
            yellowBG[i].GetComponent<RectTransform>().localScale = currentScale;
        }
    }
}
