using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageScroller : MonoBehaviour {
    [Header("Debug")]
    public float currentSpeed;

    [SerializeField, Header("Interface")]
    private Renderer imageRenderer;
    public float speed;
    public bool horizontal, vertical;

    void Update()
    {
        imageRenderer.material.mainTextureOffset +=
            new Vector2(horizontal ? speed*Time.deltaTime*Level.level : 0f, vertical ? speed*Time.deltaTime*Level.level : 0f);

        currentSpeed = speed*Level.level;
    }
}
