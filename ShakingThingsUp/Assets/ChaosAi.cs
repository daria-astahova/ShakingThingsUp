using UnityEngine;
using System.Collections.Generic;

public class ChaosAI : MonoBehaviour
{
    public int level = 0;         // Chaos power level
    public Tile currentTile;      // Where AI is standing
    public BoardManager board;    // Reference to board

    private enum DominantPlayer { None, Player1, Player2 }

    // -----------------------------
    // Initialization
    // -----------------------------
    public void Initialize(BoardManager b)
    {
        board = b;

        if (board == null || board.tiles == null)
        {
            Debug.LogError("ChaosAI.Initialize: Board or tiles array not ready.");
            return;
        }

        MoveAIToRandomTile(); // spawn at start
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
        if (board == null || board.tiles == null)
        {
            Debug.LogWarning("ChaosAI.PerformTurn: board not ready.");
            return;
        }

        MoveAIToRandomTile();

        if (level > 0)
            ApplyInfluence();
    }

    // -----------------------------
    // Move AI to a random neutral tile
    // -----------------------------
    void MoveAIToRandomTile()
    {
        if (board == null || board.tiles == null)
        {
            Debug.LogError("ChaosAI: Board not initialized yet when trying to move AI.");
            return;
        }

        List<Tile> unclaimed = new List<Tile>();

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Tile t = board.tiles[x, y];
                if (t != null && !t.hasCharacter && t.value == 0)
                    unclaimed.Add(t);
            }
        }

        // If no neutral tiles left, AI disappears
        if (unclaimed.Count == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        Tile dest = unclaimed[Random.Range(0, unclaimed.Count)];
        currentTile = dest;

        transform.position = dest.WorldPos + Vector3.up * 0.5f;
    }

    // -----------------------------
    // Figure out who's winning
    // -----------------------------
    DominantPlayer GetDominantPlayer()
    {
        int p1Score = 0;
        int p2Score = 0;

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Tile t = board.tiles[x, y];
                if (t == null) continue;

                if (t.value > 0)
                    p1Score += t.value;      // P1 control
                else if (t.value < 0)
                    p2Score += -t.value;     // P2 control (abs)
            }
        }

        int diff = Mathf.Abs(p1Score - p2Score);

        // If the game is very close, AI stays "neutral"
        if (diff < 3)
            return DominantPlayer.None;

        return p1Score > p2Score ? DominantPlayer.Player1 : DominantPlayer.Player2;
    }

    // -----------------------------
    // Apply influence around current tile
    // -----------------------------
    void ApplyInfluence()
    {
        if (currentTile == null) return;

        DominantPlayer leader = GetDominantPlayer();

        Tile up = board.GetTile(currentTile.x, currentTile.y + 1);
        Tile down = board.GetTile(currentTile.x, currentTile.y - 1);
        Tile left = board.GetTile(currentTile.x - 1, currentTile.y);
        Tile right = board.GetTile(currentTile.x + 1, currentTile.y);

        ApplyTo(up, leader);
        ApplyTo(down, leader);
        ApplyTo(left, leader);
        ApplyTo(right, leader);
    }

    // -----------------------------
    // Influence logic for one tile
    // -----------------------------
    void ApplyTo(Tile t, DominantPlayer leader)
    {
        if (t == null) return;
        if (t.hasCharacter) return;
        if (t.value == 0) return;  // nothing to push

        bool isP1Tile = t.value > 0;
        bool isLeaderTile =
            (leader == DominantPlayer.Player1 && isP1Tile) ||
            (leader == DominantPlayer.Player2 && !isP1Tile);

        // Direction: always push toward 0
        int stepDir = t.value > 0 ? -1 : 1;

        // Base magnitude from AI level
        int magnitude = level;

        // 🔥 If this tile belongs to the winning player, hit it harder
        if (isLeaderTile)
            magnitude *= 2;

        t.AddInfluence(stepDir * magnitude);
    }
}
