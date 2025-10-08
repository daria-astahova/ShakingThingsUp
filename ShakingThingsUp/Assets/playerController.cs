using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
//works for both the arduino leanardo controller and normal keyboard
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 200f;

    private Vector3 moveDirection;

    void Update()
    {
        HandleMovement();
        HandleActions();
    }

    void HandleMovement()
    {
        // Joystick 1 → WASD (or actual keyboard)
        float moveX = 0f;
        float moveZ = 0f;

        if (Input.GetKey(KeyCode.W)) moveZ += 1f;
        if (Input.GetKey(KeyCode.S)) moveZ -= 1f;
        if (Input.GetKey(KeyCode.A)) moveX -= 1f;
        if (Input.GetKey(KeyCode.D)) moveX += 1f;

        moveDirection = new Vector3(moveX, 0, moveZ).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        // Joystick 2 → Arrow keys (camera or rotation)
        float rotateY = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) rotateY -= 1f;
        if (Input.GetKey(KeyCode.RightArrow)) rotateY += 1f;

        transform.Rotate(Vector3.up * rotateY * rotationSpeed * Time.deltaTime);
    }

    void HandleActions()
    {
        // Button 1 → Left click
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Left click (button 1)");
            // Insert shooting, selecting, etc.
        }

        // Button 2 → Spacebar (jump or confirm)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space pressed (button 2)");
            // Insert jump or confirm action
        }

        // Button 3 → Z (settings, etc.)
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("Z pressed (button 3)");
            // Insert settings action
        }

        // Button 4 → X (exit or cancel)
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("X pressed (button 4)");
            // Insert exit or cancel action
        }
    }
}
