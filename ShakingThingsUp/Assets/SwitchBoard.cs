using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// code from https://discussions.unity.com/t/how-can-i-rotate-the-camera-in-90-with-pressing-one-button/205986/2


public class SwitchBoard : MonoBehaviour
{
    public Transform cam;

    public void FlipCamera()
    {
        Vector3 rot = cam.eulerAngles;
        rot.z += 180f;
        cam.eulerAngles = rot;
    }
}

