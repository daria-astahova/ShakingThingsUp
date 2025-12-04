using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public Tile[,] tiles = new Tile[5, 8]; 
    // x = 0..4 (A–E)
    // y = 0..7 (1–8 but zero-index)

    private Dictionary<char, int> columnMap = new Dictionary<char, int>()
    {
        { 'A', 0 },
        { 'B', 1 },
        { 'C', 2 },
        { 'D', 3 },
        { 'E', 4 }
    };

    void Awake()
    {
        LoadAllTiles();
        Debug.Log("BoardManager loaded tiles successfully.");
    }

    /// <summary>
    /// Finds all tile objects named A1–E8 and stores them in the 2D array.
    /// </summary>
    void LoadAllTiles()
    {
        Tile[] allTiles = FindObjectsOfType<Tile>();

        foreach (Tile t in allTiles)
        {
            string name = t.gameObject.name.ToUpper(); // Example "C7"

            if (name.Length < 2) continue;

            char letter = name[0];     // A–E
            char number = name[1];     // 1–8

            if (!columnMap.ContainsKey(letter)) continue;
            if (!char.IsDigit(number)) continue;

            int x = columnMap[letter];
            int y = (int)char.GetNumericValue(number) - 1; // convert '1' → 0

            // Assign coordinates to the tile
            t.x = x;
            t.y = y;

            // Store it
            tiles[x, y] = t;
        }
    }

    /// <summary>
    /// Returns the tile at grid (x,y). Returns null if out of bounds.
    /// </summary>
    public Tile GetTile(int x, int y)
    {
        if (x < 0 || x >= 5 || y < 0 || y >= 8)
            return null;

        return tiles[x, y];
    }

    /// <summary>
    /// Returns all tiles within a square radius (Manhattan/chebyshev), excluding center.
    /// </summary>
    public List<Tile> GetNeighborsRadius(int cx, int cy, int radius)
    {
        List<Tile> results = new List<Tile>();

        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                if (dx == 0 && dy == 0) continue; // skip center
                Tile t = GetTile(cx + dx, cy + dy);

                if (t != null)
                    results.Add(t);
            }
        }

        return results;
    }
}
