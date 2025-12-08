using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Winner : MonoBehaviour
{
    [Header("Win Condition")]
    public int pointsNeededToWin = 100;       // Total points required to win (per player)

    [Header("Scenes")]
    public string player1WinScene = "P1Win";  // Name of Player 1 victory scene
    public string player2WinScene = "P2Win";  // Name of Player 2 victory scene

    private Tile[] allTiles;

    void Start()
    {
        // Cache all tiles at start (static board)
        allTiles = FindObjectsOfType<Tile>();

        // Check every half-second
        InvokeRepeating(nameof(CheckForWinner), 0.5f, 0.5f);
    }

    void CheckForWinner()
    {
        int p1Score = 0;
        int p2Score = 0;

        foreach (Tile tile in allTiles)
        {
            if (tile == null) continue;

            if (tile.value > 0)
            {
                // Player 1 points (sum positives)
                p1Score += tile.value;
            }
            else if (tile.value < 0)
            {
                // Player 2 points (sum magnitude of negatives)
                p2Score += -tile.value;  // e.g. value = -3 → +3 points
            }
        }

        // Optional: debug
        // Debug.Log($"P1 Score: {p1Score}, P2 Score: {p2Score}");

        // Winner conditions
        if (p1Score >= pointsNeededToWin)
        {
            Debug.Log("Player 1 wins!");
            SceneManager.LoadScene(2);
        }
        else if (p2Score >= pointsNeededToWin)
        {
            Debug.Log("Player 2 wins!");
            SceneManager.LoadScene(3);
        }
    }
}
