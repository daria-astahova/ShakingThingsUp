using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class CoopCameraController : MonoBehaviour
{
    private int numberOfPlayers;
    private GameObject Player1;
    private GameObject Player2;
    private Vector3 cameraPoint;
    private float cameraSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (numberOfPlayers)
        {
            case 1:
                transform.position = new Vector3(Player1.transform.position.x, 15, Player1.transform.position.z);
                break;
            case 2:
                Vector3 midpoint = (Player1.transform.position + Player2.transform.position) / 2;
                float playerDistance = Vector3.Distance(Player1.transform.position, Player2.transform.position);
                if (playerDistance < 10)
                {
                    cameraPoint = new Vector3(midpoint.x, 15, midpoint.z);
                }
                else if (playerDistance >= 130)
                {
                    cameraPoint = new Vector3(midpoint.x, 75, midpoint.z);
                }
                else
                {
                    float CameraHeight = 15 + (playerDistance - 10) / 12;
                    cameraPoint = new Vector3(midpoint.x, CameraHeight, midpoint.z);
                }
                transform.position = Vector3.MoveTowards(transform.position, cameraPoint, cameraSpeed * Time.deltaTime);
                break;
        }
    }

    public void AddPlayer(GameObject p)
    {
        if (numberOfPlayers == 0)
        {
            Player1 = p;
        }
        else if (numberOfPlayers == 1)
        {
            Player2 = p;
        }
        numberOfPlayers++;
    }
}