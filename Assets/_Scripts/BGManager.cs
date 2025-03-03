using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGManager : MonoBehaviour
{
    public GameObject[] yellowBG;
    public GameObject[] redBG;
    public GameObject redFilter;

    private float alpha;
    private int level => Level.level;

    void Start()
    {
        /*for (int i = 0; i < yellowBG.Length; i++) {
            Color col = new Color(1f, 1f, 1f, 1f);
            yellowBG[i].GetComponent<Renderer>().material.color = col;
        }*/
        redFilter.GetComponent<SpriteRenderer>().enabled = true;
    }

    void Update()
    {
        /*if (Level.level <= 10) for (int i = 0; i < redBG.Length; i++) {
            float alpha = Mathf.Lerp(0f, 1f, Level.level/10);
            Color col = new Color(1f, 1f, 1f, alpha);
            redBG[i].GetComponent<Renderer>().material.color = col;
        }*/

        if (Level.level <= 10) {
            alpha = Mathf.Lerp(0f, 0.4f, (level-1)/9f);
            Color col = redFilter.GetComponent<SpriteRenderer>().color;
            col = new Color(col.r, col.g, col.b, alpha);
            redFilter.GetComponent<SpriteRenderer>().color = col;
        }
    }
}
