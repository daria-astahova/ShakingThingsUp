using UnityEngine;

// No changes needed for the base class
public abstract class ChessFigure : MonoBehaviour
{
    public int CurrentX { get; set; }
    public int CurrentY { get; set; }
    public bool isWhite;

    public void Awake()
    {
        // Automatically determine color from prefab name for convenience
        isWhite = gameObject.name.Contains("White");
    }
    
    public void SetPosition(int x, int y)
    {
        CurrentX = x;
        CurrentY = y;
    }

    public virtual bool[,] PossibleMove()
    {
        return new bool[8, 8];
    }
}

// --- EXAMPLE PIECE ---

public class Rook : ChessFigure
{
    // REFACTORED: Much cleaner logic using a helper function
    public override bool[,] PossibleMove()
    {
        bool[,] moves = new bool[8, 8];

        // Check moves in all four cardinal directions
        CheckLine(moves, 1, 0);  // Right
        CheckLine(moves, -1, 0); // Left
        CheckLine(moves, 0, 1);  // Up
        CheckLine(moves, 0, -1); // Down

        return moves;
    }

    // ADDED: Helper function to check a line of squares
    void CheckLine(bool[,] moves, int xIncrement, int yIncrement)
    {
        int x = CurrentX + xIncrement;
        int y = CurrentY + yIncrement;

        while (x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            ChessFigure piece = Board.instance.pieces[x, y];

            if (piece == null) // Empty square
            {
                moves[x, y] = true;
            }
            else // Square is occupied
            {
                if (piece.isWhite != this.isWhite) // It's an enemy piece
                {
                    moves[x, y] = true;
                }
                // Stop checking further in this direction (path is blocked)
                break;
            }
            
            x += xIncrement;
            y += yIncrement;
        }
    }
}