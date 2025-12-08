using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Winner : MonoBehaviour
{
    public int tilesNeededToWin = 100;         // Number of tiles required to win
    public string player1WinScene = "P1Win";  // Name of Player 1 victory scene
    public string player2WinScene = "P2Win";  // Name of Player 2 victory scene

    private Tile[] allTiles;

    void Start()
    {
        // Cache all tiles at start
        allTiles = FindObjectsOfType<Tile>();

        // Check every half-second (efficient, avoids Update spam)
        InvokeRepeating(nameof(CheckForWinner), 0.5f, 0.5f);
    }

    void CheckForWinner()
    {
        int p1Count = 0;
        int p2Count = 0;

        foreach (Tile tile in allTiles)
        {
            if (tile.value > 0)
                p1Count++;
            else if (tile.value < 0)
                p2Count++;
        }

        // Debug readout if useful
        // Debug.Log($"P1: {p1Count}  P2: {p2Count}");

        // Winner conditions
        if (p1Count >= tilesNeededToWin)
        {
            SceneManager.LoadScene(2);
        }
        else if (p2Count >= tilesNeededToWin)
        {
             SceneManager.LoadScene(3);
        }
    }
}
