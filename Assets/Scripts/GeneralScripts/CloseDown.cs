using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDown : MonoBehaviour
{
    // Time to wait before activating the controls
    private float waitTime = 5.0f;
    private float timer = 0.0f;

    // Update is called once per frame
    void Update()
    {
        // Increment the timer by the time elapsed since the last frame
        timer += Time.deltaTime;

        // Check if the wait time has passed
        if (timer >= waitTime)
        {
            // Check if the Escape key is pressed
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // Close the game
                Application.Quit();
            }

            // Check if the left mouse button is clicked
            if (Input.GetMouseButtonDown(0))
            {
                // Lock the cursor to the game window and make it invisible
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
