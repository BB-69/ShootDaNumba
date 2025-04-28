using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    public ObjectPool objPool;
    public static float originLoop, currentLoop;

    [Header("Statics")]
    public TextMeshPro levelText;
    public TextMeshPro expText;
    public Slider[] expIndicator;
    private float expSpeed = 2f;
    private float snapDelta = 0.003f;

    public static int level = 1, exp = 0, highestLevel = 0;
    private static int expCap => level * level;
    private static bool toggleLevelDown = false;

    private float totalTime = 0.0f;

    void OnEnable()
    {
        gameOver = false;
        toggleGameOverScreen = false;
        toggleReset = false;
        level = 1;

        originLoop = objPool.loopInterval;

        gameJumpFXObj.SetActive(true);
        gameJumpFX = gameJumpFXObj.GetComponent<SpriteRenderer>();

        Color col = gameJumpFX.color;
        col.a = 0f;
        gameJumpFX.color = col;
    }

    void Update()
    {
        highestLevel = Mathf.Max(highestLevel, level);

        if (toggleGameOverScreen)
        {
            gameOverScreen.SetActive(true);
            highScoreText.text = $"Highest Level = {highestLevel}";

            // Final total time initialize
            int totalHour, totalMin, totalSecond;
            if (totalTime > 3600) {
                totalHour = (int) Mathf.Floor(totalTime / 3600f);
                totalMin = (int) totalTime % 3600;
                totalSecond = (int) totalTime % 60;
                totalTimeText.text = $"Time Survived: {totalHour}h {totalMin}m {totalSecond}s";
            } else if (totalTime > 60) {
                totalMin = (int) Mathf.Floor(totalTime / 60f);
                totalSecond = (int) totalTime % 60;
                totalTimeText.text = $"Time Survived: {totalMin}m {totalSecond}s";
            } else {
                totalSecond = (int) Mathf.Floor(totalTime);
                totalTimeText.text = $"Time Survived: {totalSecond}s";
            }
            
            if (Input.anyKeyDown) {
                ResetGame();
            }
        }
        else totalTime += Time.deltaTime;

        try
        {
            levelText.text = $"{level}";
            expText.text = $"EXP: {exp}/{expCap}";
            SideEXPIndicator();
            if (gameOver) return;
        }
        catch (Exception e) // Log errors in WebGL
        {
            Debug.LogError($"Error in Update(): {e.Message}");
        }

        objPool.loopInterval -= 0.010f * Time.deltaTime;
        if (objPool.loopInterval < 0.8f)
        {
            objPool.loopInterval = originLoop;
            StartCoroutine(GameJump());
        }

        if (toggleLevelDown)
        {
            toggleLevelDown = false;
            colorInitializeLevel.InvokeColor("Red", 1f);
        }
    }

    public static void LevelUp()
    {
        exp++;
        if (exp >= expCap)
        {
            level++;
            exp = 0;
            Glock.reload = true;
        }
    }

    public static void LevelDown(int expDecrement)
    {
        exp -= expDecrement;
        if (exp <= 0)
        {
            level--;
            exp = expCap;
        }
        toggleLevelDown = true;
        if (level <= 0 && !gameOver)
        {
            level = 0;
            Instance.StartCoroutine(Instance.GameOverCoroutine()); // WebGL-friendly coroutine
        }
    }

    private void SideEXPIndicator()
    {
        float targetValue = exp / (float)expCap;

        if (Mathf.Abs(targetValue - expIndicator[0].value) < snapDelta)
        {
            foreach (var slider in expIndicator)
                slider.value = targetValue;
        }
        else
        {
            foreach (var slider in expIndicator)
                slider.value = Mathf.Lerp(slider.value, targetValue, expSpeed * Time.deltaTime);
        }
    }

    [Header("Effects")]
    public ColorInitialize colorInitializeLevel;
    public GameObject gameJumpFXObj;
    private static SpriteRenderer gameJumpFX;
    public static bool gameOver, toggleGameOverScreen, toggleReset;

    public IEnumerator GameJump()
    {
        Debug.Log("Game Jumping!");
        AudioManager.PlaySound("Thunder");
        AudioManager.PlaySound("NumberFill");
        objPool.ballMedian += Level.level;

        Color oldCol = gameJumpFX.color, newCol = oldCol;
        oldCol.a = 0f;
        newCol.a = 0.7f;

        float time = 0f, duration = 2f;

        Ball.gameJump = true;
        while (time < duration * 1 / 4f)
        {
            gameJumpFX.color = Color.Lerp(oldCol, newCol, time / (duration * 1 / 4f));
            time += Time.deltaTime;
            yield return null;
        }
        gameJumpFX.color = newCol;

        yield return new WaitForSeconds(duration * 1 / 2f);
        time = 0f;

        while (time < duration * 1 / 4f)
        {
            gameJumpFX.color = Color.Lerp(newCol, oldCol, time / (duration * 1 / 4f));
            time += Time.deltaTime;
            yield return null;
        }
        gameJumpFX.color = oldCol;
        Ball.gameJump = false;
        AudioManager.StopSound("NumberFill");
    }

    public static Level Instance { get; private set; }

    void Awake()
    {
        Instance = this; // Allows static methods to call instance coroutines
    }

    public IEnumerator GameOverCoroutine()
    {
        gameOver = true;
        AudioManager.PlaySound("BassDrop");

        Color oldCol = gameJumpFX.color, newCol = oldCol;
        oldCol.a = 0f;
        newCol.a = 0.7f;
        newCol.g = 0f;
        newCol.b = 0.6f;

        float duration = 0.5f * 1 / 4f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            gameJumpFX.color = Color.Lerp(oldCol, newCol, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gameJumpFX.color = newCol;

        yield return new WaitForSeconds(2.5f); // 2.5 seconds delay

        toggleGameOverScreen = true;
    }

    [Header("Game Over")]
    public GameObject gameOverScreen;
    public TextMeshPro highScoreText;
    public TextMeshPro totalTimeText;

    void ResetGame()
    {
        toggleReset = true;
        toggleGameOverScreen = false;

        SceneManager.LoadScene("SampleScene");
    }
}
