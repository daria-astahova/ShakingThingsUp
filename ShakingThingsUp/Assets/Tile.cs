using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector3 WorldPos => transform.position;

    [Header("Tile Settings")]
    public int x;
    public int y;


    [Tooltip("Integer strength: >0 = P1, <0 = P2, 0 = neutral.")]
    public int value = 0;

    [Header("Materials")]
    public Material neutralMat;
    public Material p1Mat;
    public Material p2Mat;

    [Header("Occupancy")]
    public bool hasCharacter = false;   // If someone is standing on it
    public GameObject occupyingPiece;   // Reference to character object

    private MeshRenderer rend;

    void Awake()
    {
        rend = GetComponent<MeshRenderer>();
        UpdateAppearance();
    }

    public void UpdateAppearance()
    {
        if (hasCharacter)
        {
            // DO NOT CHANGE TILE while character stands on it
            return;
        }

        if (value > 0)
            rend.material = p1Mat;
        else if (value < 0)
            rend.material = p2Mat;
        else
            rend.material = neutralMat;
    }

    /// <summary>
    /// Adds influence to this tile. Positive = P1, Negative = P2.
    /// </summary>
    public void AddInfluence(int amount)
    {
        if (hasCharacter) return; // Do not change tile under character

        value += amount;

        // Clamp ridiculous values (optional)
        if (value > 10) value = 10;
        if (value < -10) value = -10;

        UpdateAppearance();
    }
}
