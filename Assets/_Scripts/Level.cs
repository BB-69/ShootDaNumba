using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    public ObjectPool objPool;
    public float loop = 0f, originLoop, currentLoop;
    
    [Header("Statics")]
    public TextMeshPro levelText;
    public TextMeshPro expText;
    public Slider[] expIndicator;
    private float expSpeed = 2f;
    private float snapDelta = 0.003f;

    public static int level = 1, exp = 0;
    private static int expCap => level * level * level * 2;

    void Start()
    {
        originLoop = objPool.loopInterval;

        gameJumpFXObj.SetActive(true);
        gameJumpFX = gameJumpFXObj.GetComponent<SpriteRenderer>();

        Color col = gameJumpFX.color;
        col.a = 0f;
        gameJumpFX.color = col;
    }

    void Update()
    {
        loop += Time.deltaTime; currentLoop = objPool.loopInterval;
        if (loop > 20f) {loop = 0f; objPool.loopInterval -= 0.1f;}
        if (objPool.loopInterval < 0.45f) {objPool.loopInterval = originLoop; StartCoroutine(GameJump());}

        try {
            levelText.text = $"{level}";
            expText.text = $"EXP: {exp}/{expCap}";
            SideEXPIndicator();
        }
        catch {}
    }

    public static void LevelUp()
    {
        exp++;
        if (exp >= expCap) {level++; exp = 0;}
    }

    private void SideEXPIndicator()
    {
        if (expIndicator[0].value == exp/(float)expCap) return;
        else if (Mathf.Abs( exp/(float)expCap - expIndicator[0].value ) < snapDelta) for (int i = 0; i < expIndicator.Length; i++) {
            expIndicator[i].value = exp/(float)expCap;
        } else for (int i = 0; i < expIndicator.Length; i++) {
                expIndicator[i].value = Mathf.Lerp(expIndicator[i].value, exp/(float)expCap, expSpeed * Time.deltaTime);
        }
    }

    [Header("Effects")]
    public GameObject gameJumpFXObj;
    private SpriteRenderer gameJumpFX;

    public IEnumerator GameJump()
    {
        Debug.Log("Game Jumping!");
        objPool.ballMedian = objPool.ballMedian * 2;
        Color oldCol = gameJumpFX.color, newCol = oldCol;
        oldCol.a = 0f; newCol.a = 0.5f;

        float time = 0f, duration = 2f;

        Ball.gameJump = true;
        while (time < duration * 1/4f) {
            gameJumpFX.color = Color.Lerp(oldCol, newCol, time / duration * 1/4f);
            time += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(duration * 1/2f);
        time = duration * 3/4;

        while (time < duration) {
            gameJumpFX.color = Color.Lerp(newCol, oldCol, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        Ball.gameJump = false;
    }
}
