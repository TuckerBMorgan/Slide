using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameEngine : MonoBehaviour {

    public Dictionary<int, Controller> Players;
    public Dictionary<Guid, SlideCharacter> CharactersInGame;

	// Use this for initialization
	void Start () {
        RuneManager.Singelton.AddListener(typeof(RuneManager.SpawnEvent), SpawnCharacter);
        RuneManager.Singelton.AddListener(typeof(RuneManager.NewController), NewController);
        RuneManager.Singelton.AddListener(typeof(RuneManager.MoveEvent), MoveCharacter);

        Players = new Dictionary<int, Controller>();
        CharactersInGame = new Dictionary<Guid, SlideCharacter>();

        var newC = new RuneManager.NewController(0, System.Guid.NewGuid(), Controller.ControllerType.Player);
        var newC2 = new RuneManager.NewController(1, System.Guid.NewGuid(), Controller.ControllerType.AI);

        RuneManager.Singelton.ExecuteRune(newC);
        RuneManager.Singelton.ExecuteRune(newC2);

        var spaw = new RuneManager.SpawnEvent(0, new Vector2(0, 0), "Test", System.Guid.NewGuid());

        RuneManager.Singelton.ExecuteRune(spaw);

	    var moves = GridController.Singelton.GetRunedPath(CharactersInGame[spaw.guid],
	        GridController.Singelton.GetTile(0, 0), GridController.Singelton.GetTile(3, 7));

	    for (var index = 0; index < moves.Count; index++)
	    {
	        var t = moves[index];
	        RuneManager.Singelton.ExecuteRune(t);
	    }

        var en = new RuneManager.SpawnEvent(1, new Vector2(9, 9), "Test", System.Guid.NewGuid());
        RuneManager.Singelton.ExecuteRune(en);
    }
	
	// Update is called once per frame
	void Update () {
	
	}



    public void NewController(RuneManager.Rune rune, System.Action action)
    {
        var controller = ((RuneManager.NewController)rune);
        switch(controller.type)
        {
            case Controller.ControllerType.Player:
               var go = (GameObject)Resources.Load("Prefabs/PlayerController");
                go = Instantiate(go);
                Players.Add(controller.team, go.GetComponent<PlayerController>());
                go.GetComponent<PlayerController>().Setup(controller.team, controller.guid);
                break;

            case Controller.ControllerType.AI:
                var aiGo = (GameObject)Resources.Load("Prefabs/AIController");
                aiGo = Instantiate(aiGo);
                Players.Add(controller.team, aiGo.GetComponent<AiController>());
                aiGo.GetComponent<AiController>().Setup(controller.team, controller.guid);
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
        action();
    }

    public void MoveCharacter(RuneManager.Rune rune, System.Action action)
    {
        var moveEvent = ((RuneManager.MoveEvent) rune);
        moveEvent.mover.GetComponent<CharacterMovementController>().SetMoveTargetAndGo(moveEvent.endTile, action);
    }


}
