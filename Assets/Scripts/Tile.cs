using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
    public float H { get; set; }

    public float G { get; set; }

    public float F { get; set; }

    public Tile Parent { get; set; }

    public int X { get; set; }

    public int Y { get; set; }

    public bool HasBeenCheckedBySearch { get; set; }

    public string Name { get; set; }

    void Awake()
    {
        H = H = 0;
        G = G = 0;
        F = F = 0;
        HasBeenCheckedBySearch = false;
    }

    public void SetUpTile(int x, int y)
    {
        X = x;
        Y = y;

        name = x + " , " +  y;
        Name = X + " , " +  y;
    }

    public void ResetTile()
    {
        H = G = F = 0;
        Parent = null;
        HasBeenCheckedBySearch = false;
    }
}
