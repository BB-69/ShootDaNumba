using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public float fixedHeight = 5f; // Set the camera to always be this tall in world units
    private float aspectRatio;
    private float taskbarHeight = 40f; // Adjust this based on your platform (e.g., 40px for Windows taskbar)

    void Start()
    {
        Camera cam = Camera.main;
        
        if (cam.orthographic)
        {
            // Get the usable screen height after subtracting the taskbar height
            float adjustedHeight = Screen.height - taskbarHeight;

            // Set the orthographic size based on the adjusted height
            cam.orthographicSize = fixedHeight / 2f; // Keep a fixed height for the camera

            // Adjust the width based on the aspect ratio of the screen, excluding taskbar space
            aspectRatio = (float)Screen.width / adjustedHeight;
            float fixedWidth = fixedHeight * aspectRatio;

            // Optional: Log the adjusted width (for debugging)
            Debug.Log("Fixed Width: " + fixedWidth);
        }
    }
}
