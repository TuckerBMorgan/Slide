using System;
using UnityEngine;
using System.Collections;

public class SlideCharacter : MonoBehaviour {
    
    private float health;
    private float armour;
    private int actions;
    private Guid guid;
    private int team;
    public int Team { get { return team; } }
    private int actionPoints;
    private int totalActionPoints;
    public GameObject healthBar;

    public Tile currentTile;

	// Use this for initialization
	void Start ()
	{
	    totalActionPoints = actionPoints = 3;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Setup(float Health, float Armour, int Actions, Guid guid, int Team)
    {
        health = Health;
        armour = Armour;
        actions = Actions;
        this.guid = guid;
        team = Team;
    }

    public void SetTile(Tile tile)
    {
        currentTile = tile;
    }

    void OnMouseDown()
    {
        if (Input.GetMouseButton(0))
        {
            ConflictController.Instance.CharacterSelected(this);
        }
    }
    bool rightClick = false;
    void OnMouseOver()
    {
        if(Input.GetMouseButton(1) == false)
        {
            rightClick = false;
        }
        if (rightClick) return;

        if (Input.GetMouseButton(1))
        {
            rightClick = true;
        }
        
    }

    public int GetActionPoints()
    {
        return actionPoints;
    }

    public void DecrementActionPoints()
    {
        actionPoints--;
    }

    public void SetActionPoints(int value)
    {
        actionPoints = value;
    }

    public bool CanAttack(SlideCharacter target)
    {
        return true;
    }
}

