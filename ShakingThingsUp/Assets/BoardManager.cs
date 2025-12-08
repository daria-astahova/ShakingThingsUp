using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public Tile[,] tiles = new Tile[8, 8]; // A–H, 1–8

    private Dictionary<char, int> columnMap = new Dictionary<char, int>()
    {
        { 'A', 0 },
        { 'B', 1 },
        { 'C', 2 },
        { 'D', 3 },
        { 'E', 4 },
        { 'F', 5 },
        { 'G', 6 },
        { 'H', 7 },
    };

    void Awake()
    {
        LoadAllTiles();
        Debug.Log("BoardManager loaded tiles successfully.");
    }

    void LoadAllTiles()
    {
        Tile[] allTiles = FindObjectsOfType<Tile>();

        foreach (Tile t in allTiles)
        {
            string name = t.gameObject.name.ToUpper(); // Example: "C7"

            if (name.Length < 2) continue;

            char letter = name[0];     // A–H
            char number = name[1];     // 1–8

            if (!columnMap.ContainsKey(letter)) continue;
            if (!char.IsDigit(number)) continue;

            int x = columnMap[letter];
            int y = (int)char.GetNumericValue(number) - 1;

            t.x = x;
            t.y = y;

            tiles[x, y] = t;
        }
    }

    public Tile GetTile(int x, int y)
    {
        if (x < 0 || x >= 8 || y < 0 || y >= 8)
            return null;

        return tiles[x, y];
    }

    public List<Tile> GetNeighborsRadius(int cx, int cy, int radius)
    {
        List<Tile> results = new List<Tile>();

        for (int dx = -radius; dx <= radius; dx++)
            for (int dy = -radius; dy <= radius; dy++)
                if (!(dx == 0 && dy == 0))
                {
                    Tile t = GetTile(cx + dx, cy + dy);
                    if (t != null)
                        results.Add(t);
                }

        return results;
    }
}
