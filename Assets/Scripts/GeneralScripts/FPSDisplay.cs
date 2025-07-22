using UnityEngine;
using TMPro;

public class FPSDisplay : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    private float deltaTime = 0.0f;
    private bool isVisible = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            isVisible = !isVisible;
            fpsText.enabled = isVisible;
        }

        if (isVisible)
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;
            fpsText.text = string.Format("{0:0.} FPS", fps);
        }
    }
}
