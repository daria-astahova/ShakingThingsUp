using UnityEngine;

public class InputDebugger : MonoBehaviour
{
    void Update()
    {
        // WASD movement (Joystick 1)
        if (Input.GetKeyDown(KeyCode.W)) Debug.Log("W pressed");
        if (Input.GetKeyDown(KeyCode.A)) Debug.Log("A pressed");
        if (Input.GetKeyDown(KeyCode.S)) Debug.Log("S pressed");
        if (Input.GetKeyDown(KeyCode.D)) Debug.Log("D pressed");

        // Arrow keys (Joystick 2)
        if (Input.GetKeyDown(KeyCode.UpArrow)) Debug.Log("Up Arrow pressed");
        if (Input.GetKeyDown(KeyCode.DownArrow)) Debug.Log("Down Arrow pressed");
        if (Input.GetKeyDown(KeyCode.LeftArrow)) Debug.Log("Left Arrow pressed");
        if (Input.GetKeyDown(KeyCode.RightArrow)) Debug.Log("Right Arrow pressed");

        // Buttons
        if (Input.GetMouseButtonDown(0)) Debug.Log("Mouse Left Click (Button 1)");
        if (Input.GetKeyDown(KeyCode.Space)) Debug.Log("Spacebar (Button 2)");
        if (Input.GetKeyDown(KeyCode.Z)) Debug.Log("Z key (Button 3)");
        if (Input.GetKeyDown(KeyCode.X)) Debug.Log("X key (Button 4)");
    }
}
