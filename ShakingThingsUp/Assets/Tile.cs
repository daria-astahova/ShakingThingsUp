using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    public Vector3 WorldPos => transform.position;

    public int x;
    public int y;
    public int value = 0;

    public Material neutralMat;
    public Material p1Mat;
    public Material p2Mat;

    public bool hasCharacter = false;
    public GameObject occupyingPiece;

    private MeshRenderer rend;
    private TextMeshPro text;

    void Awake()
    {
        rend = GetComponent<MeshRenderer>();
        text = GetComponentInChildren<TextMeshPro>();
        UpdateAppearance();
        UpdateText();
    }

    public void UpdateAppearance()
    {
        if (hasCharacter) return;

        if (value > 0)
            rend.material = p1Mat;
        else if (value < 0)
            rend.material = p2Mat;
        else
            rend.material = neutralMat;
    }

    public void UpdateText()
    {
        if (text == null) return;

        if (value == 0)
        {
            text.text = "";
            return;
        }

        text.text = value.ToString();
        text.color = Color.black;
    }

    public void AddInfluence(int amount)
    {
        if (hasCharacter) return;

        value += amount;
        if (value > 10) value = 10;
        if (value < -10) value = -10;

        UpdateAppearance();
        UpdateText();
    }
}
