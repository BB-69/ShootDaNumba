using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public static float height, width;
    void Start()
    {
        Camera.main.orthographicSize = Screen.height / 100f / 2f;
    }
}
