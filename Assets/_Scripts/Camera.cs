using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public float fixedHeight = 5f; // Desired height in world units
    public Vector2 fixedResolution = new Vector2(960, 540); // Target resolution (e.g., for itch.io)
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        AdjustCameraSize();
    }

    void Update()
    {
        AdjustCameraSize();
    }

    void AdjustCameraSize()
    {
        if (cam.orthographic)
        {
            float targetAspect = fixedResolution.x / fixedResolution.y;
            float currentAspect = (float)Screen.width / Screen.height;
            
            if (currentAspect >= targetAspect)
            {
                // Match the fixed height when aspect ratio is wider or equal
                cam.orthographicSize = fixedHeight / 2f;
            }
            else
            {
                // Adjust height to maintain fixed width if aspect ratio is narrower
                cam.orthographicSize = (fixedHeight / 2f) * (targetAspect / currentAspect);
            }
        }
    }
}
