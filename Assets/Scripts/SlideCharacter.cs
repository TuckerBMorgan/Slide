using System;
using UnityEngine;
using System.Collections.Generic;

public class SlideCharacter : MonoBehaviour {
    
    public float Health { get; set; }
    public float Armour { get; set; }
    private int actions;
    public  int Damage { get; set; }
    private Guid guid;
    private int team;
    public int Team { get { return team; } }
    private int actionPoints;
    private int totalActionPoints;
    public GameObject healthBar;
    private Dictionary<string, CharacterAction> allowedActions;
    public CharacterAction currentAction;

    public Tile currentTile;

	// Use this for initialization
	void Start ()
	{
	    totalActionPoints = actionPoints = 3;
	    allowedActions = new Dictionary<string, CharacterAction>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnStartTurn()
    {
        actionPoints = totalActionPoints;
    }

    public void AddAction(CharacterAction newAction)
    { 
        allowedActions.Add(newAction.name, newAction);    
    }


    public void Setup(float Health, float Armour, int Actions, Guid guid, int Team)
    {
        this.Health = Health;
        this.Armour = Armour;
        actions = Actions;
        this.guid = guid;
        team = Team;

        Damage = 10;
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
}

