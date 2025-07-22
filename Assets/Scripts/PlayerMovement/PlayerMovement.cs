using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace MyGameNamespace
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : NetworkBehaviour
    {
        public float moveSpeed = 3.0f;
        public float sprintSpeed = 6.0f;

        private Rigidbody rb;
        private Vector3 moveDirection;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            // It's good practice to ensure the Rigidbody doesn't have strange rotations.
            rb.freezeRotation = true; 
        }

        void Update()
        {
            if (isLocalPlayer)
            {
                HandleInput();
            }
        }

        void FixedUpdate()
        {
            if (isLocalPlayer)
            {
                HandleMovement();
            }
        }

        void HandleInput()
        {
            // Determine the current speed
            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

            // Keyboard input for movement
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            // Store movement direction
            moveDirection = (transform.right * x + transform.forward * z).normalized * currentSpeed;
        }

        void HandleMovement()
        {
            // Apply movement to the rigidbody
            rb.MovePosition(rb.position + moveDirection * Time.fixedDeltaTime);
        }
    }
}

