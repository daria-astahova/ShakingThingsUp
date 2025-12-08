using UnityEngine;
using System.Collections.Generic;

public class ChaosAI : MonoBehaviour
{
    public int level = 0;         // Chaos power level
    public Tile currentTile;      // The tile the AI is standing on
    public BoardManager board;    // Reference to board

    // -----------------------------
    // Initialization
    // -----------------------------
    public void Initialize(BoardManager b)
    {
        board = b;

        // Safety: ensure board is valid before moving
        if (board == null || board.tiles == null)
        {
            Debug.LogError("ChaosAI.Initialize: Board or tiles array not ready.");
            return;
        }

        MoveAIToRandomTile(); // Spawn AI on start
    }

    // -----------------------------
    // Level progression logic
    // -----------------------------
    public void AdvanceLevel(int round)
    {
        if (round <= 2) level = 0;
        else if (round <= 4) level = 1;
        else level = 3;
    }

    // -----------------------------
    // AI Turn Logic
    // -----------------------------
    public void PerformTurn()
    {
        MoveAIToRandomTile();

        if (level > 0)
            ApplyInfluence();
    }

    // -----------------------------
    // Move AI to a random unclaimed tile
    // -----------------------------
    void MoveAIToRandomTile()
    {
        if (board == null || board.tiles == null)
        {
            Debug.LogError("ChaosAI: Board not initialized yet when trying to move AI.");
            return;
        }

        List<Tile> unclaimed = new List<Tile>();

        // Collect empty tiles (no character + neutral value)
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Tile t = board.tiles[x, y];

                if (t != null && !t.hasCharacter && t.value == 0)
                    unclaimed.Add(t);
            }
        }

        // If no tiles left → AI becomes inactive
        if (unclaimed.Count == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        // Choose a random tile
        Tile dest = unclaimed[Random.Range(0, unclaimed.Count)];
        currentTile = dest;

        // Move the AI model above the tile
        transform.position = dest.WorldPos + Vector3.up * 0.5f;
    }

    // -----------------------------
    // Apply influence to adjacent tiles
    // -----------------------------
    void ApplyInfluence()
    {
        if (currentTile == null) return;

        Tile up = board.GetTile(currentTile.x, currentTile.y + 1);
        Tile down = board.GetTile(currentTile.x, currentTile.y - 1);
        Tile left = board.GetTile(currentTile.x - 1, currentTile.y);
        Tile right = board.GetTile(currentTile.x + 1, currentTile.y);

        ApplyTo(up);
        ApplyTo(down);
        ApplyTo(left);
        ApplyTo(right);
    }

    // -----------------------------
    // Influence logic for one tile
    // -----------------------------
    void ApplyTo(Tile t)
    {
        if (t == null) return;
        if (t.hasCharacter) return;

        // AI pushes tiles toward 0
        if (t.value > 0)
            t.AddInfluence(-level);
        else if (t.value < 0)
            t.AddInfluence(level);
    }
}
