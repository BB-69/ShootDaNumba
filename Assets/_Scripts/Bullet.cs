using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Bullet : Shooter
{
    public float warpWidth => 1080f + 150f;

    [Header("Interface")]
    public ObjectPool objPool;
    public Glock glock;
    private RectTransform rect;
    private Rigidbody2D rb;
    private CapsuleCollider2D cl;
    private Vector2 velocity;

    private bool isActive;

    void Initialize()
    {
        rect = this.GetComponent<RectTransform>();
        rb = this.GetComponent<Rigidbody2D>();
        cl = this.GetComponent<CapsuleCollider2D>();
    }

    void UpdateInitialize()
    {
        try {
            Vector3 pos = transform.localPosition;
            pos.z = 5f;
            transform.localPosition = pos;
        } catch {}
    }

    void Start()
    {
        Initialize();
    }

    void OnEnable()
    {
        isActive = true;
        velocity = glock.bulletVelocity;

        SetAlpha(1f);
    }

    void Update()
    {
        UpdateInitialize();

        if (!isActive) {cl.enabled = false; return;}
        else cl.enabled = true;

        try {
            rb.velocity = velocity;
            rb.mass = 0.1f*Level.level;
        }
        catch {}

        if (rect.anchoredPosition.x >= warpWidth/2) SideWarp(0);
        else if (rect.anchoredPosition.x <= -warpWidth/2) SideWarp(1);
        if (rect.anchoredPosition.y > 0.5*objPool.despawnPos.y) StartCoroutine(Disappear());
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!gameObject.activeInHierarchy) return;

        if (collision.gameObject.CompareTag("Wall")) StartCoroutine(Disappear());
        else if (!collision.gameObject.CompareTag("Base") &&
            !collision.gameObject.CompareTag("BaseGlock") &&
            !collision.gameObject.CompareTag("Bullet")) Explode();
    }

    IEnumerator Disappear()
    {
        isActive = false;

        float time = 0f, duration = 0.3f;
        while (time < duration) {
            float alpha = duration - (time / duration);
            SetAlpha(alpha);
            time += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false); objPool.activeObj--;
    }

    void Explode()
    {
        isActive = false;
        gameObject.SetActive(false); objPool.activeObj--;
    }

    void SideWarp(int i)
    {
        Vector2 pos = rect.anchoredPosition;
        pos.x += -warpWidth + 2*warpWidth*i;
        rect.anchoredPosition = pos;
    }

    void SetAlpha(float a)
    {
        try {
            Color col;
            col = GetComponent<SpriteRenderer>().color;

            col.a = a;
            GetComponent<SpriteRenderer>().color = col;
        } catch {}

        for (int i = 0; i < transform.childCount; i++) {
            GameObject obj = transform.GetChild (i).gameObject;
            Color childCol = obj.GetComponent<SpriteRenderer>().color;

            childCol.a = a;
            obj.GetComponent<SpriteRenderer>().color = childCol;
        }
    }
}
