using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerRotateLock : NetworkBehaviour
{
    private bool isColliding = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (!isColliding)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            transform.Rotate(Vector3.up, mouseX);
            transform.Rotate(Vector3.left, mouseY);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isLocalPlayer)
        {
            return;
        }

        isColliding = true;
    }

    void OnCollisionExit(Collision collision)
    {
        if (!isLocalPlayer)
        {
            return;
        }

        isColliding = false;
    }
}
