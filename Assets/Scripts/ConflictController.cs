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
    public Material pulseTest;
    public SlideCharacter selectedCharacter;

    public int TurnOrder;

    public static ConflictController Instance;

    void Awake()
    {
        Instance = this;
    }


	// Use this for initialization
	void Start () {
	    
        ControllersInGame =  new List<Controller>();
	    TurnOrder = -1;

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



        /*
        var moves = GridController.Singelton.GetRunedPath(CharactersInGame[spaw.guid],
            GridController.Singelton.GetTile(0, 0), GridController.Singelton.GetTile(3, 7));

        for (var index = 0; index < moves.Count; index++)
        {
            var t = moves[index];
            RuneManager.Singelton.ExecuteRune(t);
        }
        */
        var en = new RuneManager.SpawnEvent(1, new Vector2(9, 9), "TestAI", Guid.NewGuid());
        RuneManager.Singelton.ExecuteRune(en);
        CurrentController = ControllersInGame[0];
        var firstTurn = new RuneManager.RotateTurnForController(CurrentController);
        RuneManager.Singelton.ExecuteRune(firstTurn);

        UIController.Singelton.Setup();

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void TileSelected(Tile tile)
    {
        if (CurrentController != null)
        {
            CurrentController.TileSelected(tile);
        }
    }


    public void AddNewControllerToGame(Controller newControl)
    {
        ControllersInGame.Add(newControl);
    }

    public void CharacterSelected(SlideCharacter character)
    {
        if (CurrentController && CurrentController.SelectedCharacter != character)
        {
            AddPulseMaterial(character.GetComponent<Renderer>());
            CurrentController.CharacterSelected(character);
        }
    }

    public void AddPulseMaterial(Renderer rend)
    {
        if(selectedCharacter != null)
        {
            RemovePulseMaterial(selectedCharacter.GetComponent<Renderer>());
        }
        Material[] mats = rend.sharedMaterials;
        List<Material> newMats = new List<Material>();
        for (int i = 0; i < mats.Length; i++)
        {
            newMats.Add(mats[i]);
        }
        newMats.Add(pulseTest);
        rend.GetComponent<Renderer>().sharedMaterials = newMats.ToArray();
        selectedCharacter = rend.GetComponent<SlideCharacter>();
    }

    public void RemovePulseMaterial(Renderer rend)
    {
        Material[] mats = rend.sharedMaterials;
        List<Material> newMats = new List<Material>();
        for (int i = 0; i < mats.Length;i++ )
        {
            newMats.Add(mats[i]);
        }
        
        newMats.Remove(pulseTest);
        rend.GetComponent<Renderer>().sharedMaterials = newMats.ToArray(); 
    }
}
