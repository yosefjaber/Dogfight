using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullscreenToggle : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // Example: Toggle fullscreen when the F key is pressed
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleFullscreen();
        }
    }

    void ToggleFullscreen()
    {
        // Set fullscreen mode to the opposite of the current state
        Screen.fullScreen = !Screen.fullScreen;
    }

    // Call this method to exit fullscreen
    public void ExitFullscreen()
    {
        Screen.fullScreen = false;
    }
}

