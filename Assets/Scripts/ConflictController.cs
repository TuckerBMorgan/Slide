using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class ConflictController : MonoBehaviour
{
    public Controller CurrentController;
    public List<Controller> ControllersInGame;
    public Dictionary<int, Controller> Players;
    public Dictionary<Guid, SlideCharacter> CharactersInGame;
    public AbilityButtonControl abilityButtonControl;
    public Material pulseTest;
    
    public Entity selectedEntity;

    public int TurnOrder;

    public static ConflictController Instance;

    public UIController uicontroller;

    void Awake()
    {
        Instance = this;
    }


	// Use this for initialization
	void Start () {
	    
        ControllersInGame =  new List<Controller>();
	    TurnOrder = -1;

        abilityButtonControl.Setup();

        Players = new Dictionary<int, Controller>();
        CharactersInGame = new Dictionary<Guid, SlideCharacter>();

        var newC = new RuneManager.NewController(0, System.Guid.NewGuid(), Controller.ControllerType.Player);
        var newC2 = new RuneManager.NewController(1, System.Guid.NewGuid(), Controller.ControllerType.AI);

        RuneManager.Singelton.ExecuteRune(newC);
        RuneManager.Singelton.ExecuteRune(newC2);

        var spaw = new RuneManager.SpawnEvent(0, new Vector2(0, 0), "Test", System.Guid.NewGuid());
        RuneManager.Singelton.ExecuteRune(spaw);

        var spaw2 = new RuneManager.SpawnEvent(0, new Vector2(1, 1), "TestPatrnet", System.Guid.NewGuid());
        RuneManager.Singelton.ExecuteRune(spaw2);

        var en = new RuneManager.SpawnEvent(1, new Vector2(9, 9), "TestAI", Guid.NewGuid());
        RuneManager.Singelton.ExecuteRune(en);
        CurrentController = ControllersInGame[0];
        var firstTurn = new RuneManager.RotateTurnForController(CurrentController);
        RuneManager.Singelton.ExecuteRune(firstTurn);

        AbilityButtonControl.Instance.ChangeSelectedCharacter(CurrentController.Crew[0]);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    //PC: Any Left Click on an Entity 
    public void OnSelectionAction(Entity entity)
    {

        CurrentController.OnSelctionAction(entity);
    }

    //PC: Any Right click on an Entity
    public void OnSecondaryAction(Entity entity)
    {
        CurrentController.OnSecondaryAction(entity);
    }


    public void AddNewControllerToGame(Controller newControl)
    {
        ControllersInGame.Add(newControl);
    }

    public void AddPulseMaterial(Renderer rend)
    {
        var mats = rend.sharedMaterials;
        var newMats = new List<Material>();
        for (var i = 0; i < mats.Length; i++)
        {
            newMats.Add(mats[i]);
        }
        newMats.Add(pulseTest);
        rend.GetComponent<Renderer>().sharedMaterials = newMats.ToArray();
    }

    public void RemovePulseMaterial(Renderer rend)
    {
        var mats = rend.sharedMaterials;
        var newMats = new List<Material>();
        for (var i = 0; i < mats.Length;i++ )
        {
            newMats.Add(mats[i]);
        }
        
        newMats.Remove(pulseTest);
        rend.GetComponent<Renderer>().sharedMaterials = newMats.ToArray(); 
    }
}
