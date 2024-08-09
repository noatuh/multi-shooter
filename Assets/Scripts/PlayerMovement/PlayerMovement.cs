using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace MyGameNamespace
{
    public class PlayerMovement : NetworkBehaviour
    {
        public float moveSpeed = 3.0f;
        public float sprintSpeed = 6.0f;

        void Update()
        {
            if (isLocalPlayer)
            {
                HandleMovement();
            }
        }

        void HandleMovement()
        {
            // Determine the current speed
            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

            // Keyboard input for movement
            float x = Input.GetAxis("Horizontal") * Time.deltaTime * currentSpeed;
            float z = Input.GetAxis("Vertical") * Time.deltaTime * currentSpeed;

            // Apply transformations
            transform.Translate(x, 0, z);
        }
    }
}

