using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Ball : MonoBehaviour
{
    // Debug
    private static List<int> ballList = new List<int>();
    private int thisBall = 1;
    public static bool gameJump;
    private float jumpRepeat = 0f;

    // Main
    public float warpWidth => 1080f + 150f;

    [Header("Interface")]
    public ObjectPool objPool;
    private RectTransform rect;
    private TextMeshPro tmp;
    private Rigidbody2D rb;

    private float velx, vely;
    private Vector2 vector;

    private int num;
    private float reductionTimer = 0.3f;

    private System.Random rnd = new System.Random();

    // Color
    public ColorInitialize colorInitialize;

    void Initialize()
    {
        num = (int) Mathf.Round(objPool.ballMedian - 0.6f*objPool.ballMedian);
    }

    void UpdateInitialize()
    {
        try {
            tmp.text = $"{num}";
            rb.mass = 0.5f*num;
            rb.drag = 0.001f*num;
            rb.gravityScale = 0.05f + 0.01f*(objPool.ballMedian - num);
            
            Vector3 pos = transform.localPosition;
            pos.z = 5f;
            transform.localPosition = pos;
        } catch {}
    }

    void Start()
    {
        // Debug
        for (int i = 0; i < ballList.Count; i++) thisBall++;
        ballList.Add(thisBall);

        // Main
        gameJump = false;

        rect = this.GetComponent<RectTransform>();
        tmp = this.GetComponentInChildren<TextMeshPro>();
        rb = this.GetComponent<Rigidbody2D>();
        Initialize();
    }

    void OnEnable()
    {
        try {RandomVel();}
        catch {}

        colorInitialize.InvokeColor("Green", 0f);
    }

    void Update()
    {
        UpdateInitialize();
        jumpRepeat += Time.deltaTime;

        if (!gameObject.activeInHierarchy) return;

        // Game Jump
        if (gameJump && jumpRepeat > 0.1f) {
            jumpRepeat = 0f;

            num += Level.level;
            colorInitialize.InvokeColor("Green", 0.5f, 0.1f);
        }

        // No Wall Limit
        try {
            if (rect.anchoredPosition.x >= warpWidth/2) SideWarp(0);
        else if (rect.anchoredPosition.x <= -warpWidth/2) SideWarp(1);
        if (rect.anchoredPosition.y > objPool.despawnPos.y) Disappear();
        } catch {}

        // Explode when out of value
        if (num <= 0) Explode();
    }

    IEnumerator OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet")) {
            num -= Level.level;
            colorInitialize.InvokeColor("Red", 0.5f);
        } else if (collision.gameObject.CompareTag("Ball")) {
            num++;
            colorInitialize.InvokeColor("Green", 0.5f);
        }
        yield return new WaitForSeconds(reductionTimer);
        StartCoroutine(OnCollisionEnter2D(collision));
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        StopAllCoroutines();
    }

    void Disappear()
    {
        gameObject.SetActive(false); objPool.activeObj--;
        Initialize();
    }

    void Explode()
    {
        Level.LevelUp();
        Disappear();
    }

    void SideWarp(int i)
    {
        Vector2 pos = rect.anchoredPosition;
        pos.x += -warpWidth + 2*warpWidth*i;
        rect.anchoredPosition = pos;
    }

    private void RandomVel() {
        vector = Vector2.zero;
        velx = rnd.Next(0, 100)/10f - 5f;
        vector.x += velx;
        vely = rnd.Next(0, 100)/50f - 1f;
        vector.y -= vely;
        rb.velocity = vector;
    }
}