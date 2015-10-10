using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

    private float h;
    public float H { get { return h; } set { h = value;} }

    private float g;
    public float G { get { return g; } set { g = value;} }

    private float f;
    public float F { get { return f; } set { f = value;} }

    private Tile parent;
    public Tile Parent { get { return parent; } set { parent = value;} }

    private int x;
    public int X { get { return x; } set { x = value; } }

    private int y;
    public int Y { get { return y; } set { y = value; } }

    private bool hasBeenCheckedBySearch;
    public bool HasBeenCheckedBySearch { get { return hasBeenCheckedBySearch; } set { hasBeenCheckedBySearch = value; } }

    private string name;
    public string Name { get { return name; } set { Name = value; } }

    void Awake()
    {
        H = h = 0;
        G = g = 0;
        F = f = 0;
        hasBeenCheckedBySearch = false;
    }

    void SetUpTile(int x, int y)
    {
        this.x = x;
        this.y = y;

        name = x + " , " +  y;
    }

    public void ResetTile()
    {
        h = g = f = 0;
        parent = null;
        hasBeenCheckedBySearch = false;
    }
}
