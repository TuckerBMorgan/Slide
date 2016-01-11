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
    public GameObject healthBar;
    // allowedActions["Move"] and allowedActions["Basic"] as members all characters have
    private Dictionary<string, CharacterAction> allowedActions;
    public List<SpellAction> offensiveActions;
    private CharacterAction currentAction;


    public Tile currentTile;

	// Use this for initialization
	void Start ()
	{
	    totalActionPoints = actionPoints = 3;     
	}
	
    public void OnStartTurn()
    {
        actionPoints = totalActionPoints;
        currentAction = allowedActions["Move"];
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

    public void Setup(float Health, float Armour, int Actions, Guid guid, int Team)
    {
        this.Health = Health;
        this.Armour = Armour;
        actions = Actions;
        this.guid = guid;
        team = Team;


        allowedActions = new Dictionary<string, CharacterAction>();
        offensiveActions = new List<SpellAction>();
        Damage = 10;
        var moveAct = new MoveAction();
        moveAct.Setup(this);
        currentAction = moveAct;
        allowedActions.Add("Move", moveAct);

        var bs = SpellAction.ParseAndCreateSpell("Spells/basic");
        bs.character = this;
        allowedActions.Add("Basic", bs);
        offensiveActions.Add(bs);

        var fireball = SpellAction.ParseAndCreateSpell("Spells/fireball");
        fireball.character = this;
        allowedActions.Add("Fireball", fireball);
        offensiveActions.Add(fireball);
    }

    public void SetTile(Tile tile)
    { 
        currentTile = tile;
    }

    public bool OnEntitySelection(Entity entity)
    {
        if (entity.GetEntityType() == "Tile")
            currentAction = allowedActions["Move"];

        if (currentAction.ValidateSelection(entity) == false) 
        {
            return false;
        }
        currentAction.PreformAction(entity);

        currentAction = allowedActions["Basic"];
        return true;
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

