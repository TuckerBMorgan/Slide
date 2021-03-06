﻿using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour, Entity {
    public float H { get; set; }

    public float G { get; set; }

    public float F { get; set; }

    public Tile Parent { get; set; }

    public int X { get; set; }

    public int Y { get; set; }

    public Vector2 position { get { return new Vector2(X, Y); } }

    public bool HasBeenCheckedBySearch { get; set; }

    public string Name { get; set; }

    public bool Occupied;

    public bool Walkable;

    public bool SpawnTile = false;

    public int Team = -1;

    public Color keepColor;

    public GameObject fog;

    public Entity objectOn;

    void Awake()
    {
        H = 0;
        G = 0;
        F = 0;
        HasBeenCheckedBySearch = false;
        Occupied = false;
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

    void OnMouseDown()
    {
        if (Input.GetMouseButton(0))
        {
            ConflictController.Instance.OnSelectionAction(this);
        }
    }
    bool rightClick = false;
    void OnMouseOver()
    {
        if (Input.GetMouseButton(1) == false)
        {
            rightClick = false;
        }
        if (rightClick) return;

        if (Input.GetMouseButton(1))
        {
            ConflictController.Instance.OnSecondaryAction(this);
            rightClick = true;
        }

    }

    public void SetObjectOn(Entity on)
    {
        objectOn = on;
    }

    public string GetEntityType()
    {
        return "Tile";
    }

    public Tile getCurrentTile()
    {
        return this;
    }

    public MonoBehaviour GetUnityObject()
    {
        return this;
    }

    public void SetToWhite()
    {
        GetComponent<Renderer>().material.color = Color.white;
    }

    public void SetToColor()
    {
        GetComponent<Renderer>().material.color = keepColor;
    }
}
