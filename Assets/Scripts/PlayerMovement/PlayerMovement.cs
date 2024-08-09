using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace MyGameNamespace
{
    public class PlayerMovement : NetworkBehaviour
    {
        public float moveSpeed = 3.0f;

        void Update()
        {
            if (isLocalPlayer)
            {
                HandleMovement();
            }
        }

        void HandleMovement()
        {
            // Keyboard input for movement
            float x = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
            float z = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;

            // Apply transformations
            transform.Translate(x, 0, z);
        }
    }
}

