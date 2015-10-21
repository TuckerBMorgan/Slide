using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class ConflictController : MonoBehaviour
{
    public Controller CurrentController;
    private List<Controller> ControllersInGame;
    public Dictionary<int, Controller> Players;
    public Dictionary<Guid, SlideCharacter> CharactersInGame;
    public Material pulseTest;

    private int TurnOrder;

    public static ConflictController Instance;

    void Awake()
    {
        Instance = this;
    }


	// Use this for initialization
	void Start () {
	    
        ControllersInGame =  new List<Controller>();
	    TurnOrder = -1;



        RuneManager.Singelton.AddListener(typeof(RuneManager.RotateTurnForController), RotateTurnForController);
        RuneManager.Singelton.AddListener(typeof(RuneManager.MoveEvent), MoveCharacter);
        RuneManager.Singelton.AddListener(typeof(RuneManager.SpawnEvent), SpawnCharacter);
        RuneManager.Singelton.AddListener(typeof(RuneManager.NewController), NewController);


        Players = new Dictionary<int, Controller>();
        CharactersInGame = new Dictionary<Guid, SlideCharacter>();

        var newC = new RuneManager.NewController(0, System.Guid.NewGuid(), Controller.ControllerType.Player);
        var newC2 = new RuneManager.NewController(1, System.Guid.NewGuid(), Controller.ControllerType.AI);

        RuneManager.Singelton.ExecuteRune(newC);
        RuneManager.Singelton.ExecuteRune(newC2);

        var spaw = new RuneManager.SpawnEvent(0, new Vector2(0, 0), "Test", System.Guid.NewGuid());

        RuneManager.Singelton.ExecuteRune(spaw);

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

    public void RotateTurnForController(RuneManager.Rune rune, System.Action action)
    {
        if (TurnOrder == ControllersInGame.Count)
        {
            TurnOrder = 0;
        }
        else
        {
            TurnOrder++;
        }
        CurrentController.EndTurn();
        CurrentController = ControllersInGame[TurnOrder];
        CurrentController.StartTurn();
        action();
    }


    public void MoveCharacter(RuneManager.Rune rune, System.Action action)
    {
        var moveEvent = ((RuneManager.MoveEvent)rune);
        moveEvent.mover.GetComponent<CharacterMovementController>().SetMoveTargetAndGo(moveEvent.endTile, action);
    }



    public void NewController(RuneManager.Rune rune, System.Action action)
    {
        var controller = ((RuneManager.NewController)rune);
        switch (controller.type)
        {
            case Controller.ControllerType.Player:
                var go = (GameObject)Resources.Load("Prefabs/PlayerController");
                go = Instantiate(go);
                Players.Add(controller.team, go.GetComponent<PlayerController>());
                go.GetComponent<PlayerController>().Setup(controller.team, controller.guid);
                ControllersInGame.Add(go.GetComponent<PlayerController>());
                break;

            case Controller.ControllerType.AI:
                var aiGo = (GameObject)Resources.Load("Prefabs/AIController");
                aiGo = Instantiate(aiGo);
                Players.Add(controller.team, aiGo.GetComponent<AiController>());
                aiGo.GetComponent<AiController>().Setup(controller.team, controller.guid);
                ControllersInGame.Add((aiGo.GetComponent<AiController>()));
                break;

            default:
                break;
        }
        action();
    }

    Vector3 _spawnUse;
    public void SpawnCharacter(RuneManager.Rune rune, System.Action action)
    {
        var spawn = ((RuneManager.SpawnEvent)rune);

        var go = (GameObject)Resources.Load("Prefabs/Sphere");
        _spawnUse.x = spawn.spawnPosition.x;
        _spawnUse.y = 0;
        _spawnUse.z = spawn.spawnPosition.y;
        go = Instantiate(go);
        go.name = spawn.characterName;
        go.transform.position = _spawnUse;
        go.GetComponent<SlideCharacter>().Setup(100, 100, 2, spawn.guid, spawn.team);
        Players[spawn.team].AddCrewMember(go.GetComponent<SlideCharacter>());
        CharactersInGame.Add(spawn.guid, go.GetComponent<SlideCharacter>());
        go.GetComponent<SlideCharacter>().SetTile(GridController.Singelton.GetTile((int)spawn.spawnPosition.x, (int)spawn.spawnPosition.y));
        action();
    }

    public void CharacterSelected(SlideCharacter character)
    {
        Debug.Log(CurrentController.SelectedCharacter);
        if (CurrentController && CurrentController.SelectedCharacter != character)
        {
            AddPulseMaterial(character.GetComponent<Renderer>());
            CurrentController.CharacterSelected(character);
        }
    }

    public void AddPulseMaterial(Renderer rend)
    {
        Material[] mats = rend.sharedMaterials;
        List<Material> newMats = new List<Material>();
        for (int i = 0; i < mats.Length; i++)
        {
            newMats.Add(mats[i]);
        }
        newMats.Add(pulseTest);
        rend.GetComponent<Renderer>().sharedMaterials = newMats.ToArray();

    }

    public void RemovePulseMaterial(Renderer rend)
    {
        Material[] mats = rend.sharedMaterials;
        List<Material> newMats = new List<Material>();
        for (int i = 0; i < mats.Length; i++)
        {
            newMats.Add(mats[i]);
        }
        newMats.Add(pulseTest);
        rend.GetComponent<Renderer>().sharedMaterials = newMats.ToArray();
    }

}
