using System;
using UnityEngine;
using System.Collections.Generic;

public class SlideCharacter : MonoBehaviour, Entity {
    
    public float Health { get; set; }
    public float Armour { get; set; }
    private int actions;
    public  int Damage { get; set; }
    private Guid guid;
    private int team;
    public int Team { get { return team; } }
    private int actionPoints;
    private int totalActionPoints;
    public int TotalActionPoints { get { return totalActionPoints; } set { totalActionPoints = value; } }
    public GameObject healthBar;
    // allowedActions["Move"] and allowedActions["Basic"] as members all characters have
    public Dictionary<string, CharacterAction> allowedActions;
    public List<SpellAction> offensiveActions;
    public CharacterAction currentAction;
    public SpellAction fallbackAction;


    public Tile currentTile;

	// Use this for initialization
	void Start ()
	{   
        totalActionPoints = 3;
        actionPoints = 3;    
	}
	
    public void OnStartTurn()
    {
        actionPoints = 3;
        currentAction = allowedActions["Move"];
        fallbackAction = (SpellAction)allowedActions["basic"];
    }

    public void AddAction(CharacterAction newAction)
    { 
        allowedActions.Add(newAction.name, newAction);    
    }

    public void DrawInspector()
    {
        foreach(KeyValuePair<string, CharacterAction> actions in allowedActions)
        {
            actions.Value.DrawInspector();
        }
    }
    
    public void Setup(string name, int baseHealth, int baseArmour, int actions, Guid guid, int Team)
    {
        allowedActions = new Dictionary<string, CharacterAction>();
        offensiveActions = new List<SpellAction>();

        var moveAct = new MoveAction();
        moveAct.Setup(this, 3);
        moveAct.name = "Move";
        currentAction = moveAct;
        allowedActions.Add("Move", moveAct);

        this.Health = baseHealth;
        this.Armour = baseArmour;
        actionPoints = actions;
        team = Team;
    }

    public void SetTile(Tile tile)
    { 
        currentTile = tile;
    }

    public bool OnEntitySelection(Entity entity)
    {
        if (entity.GetEntityType() == "Tile")
            currentAction = allowedActions["Move"];
        Debug.Log(name);
        Debug.Log(entity);
        if (currentAction.ValidateSelection(entity) == false) 
        {
            return false;
        }
        currentAction.PreformAction(entity);

        currentAction = fallbackAction;
        return true;
    }

    public void OnAbilityButtonPressed(string name)
    {
        if (allowedActions.ContainsKey(name))
        {
            currentAction = allowedActions[name];
            fallbackAction = (SpellAction)currentAction;
        }
        else
        {
            currentAction = allowedActions["Move"];
        }
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
        if(Input.GetMouseButton(1) == false)
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

    public string GetEntityType()
    {
        return "SlideCharacter";
    }
    public MonoBehaviour GetUnityObject()
    {
        return this;
    }
    public Tile getCurrentTile()
    {
        return currentTile;
    }
}

