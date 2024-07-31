using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerLook : NetworkBehaviour
{
    public Transform playerCamera;
    public float mouseSensitivity = 100.0f;
    private float xRotation = 0.0f;

    void Start()
    {
        if (!isLocalPlayer && playerCamera != null)
        {
            playerCamera.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            HandleMouseLook();
        }
    }

    void HandleMouseLook()
    {
        if (playerCamera == null)
        {
            Debug.LogError("Player camera is not assigned.");
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
