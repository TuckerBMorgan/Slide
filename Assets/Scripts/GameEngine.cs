using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameEngine : MonoBehaviour {

    public Dictionary<int, Controller> players;
    public List<SlideCharacter> charactersInGame;

	// Use this for initialization
	void Start () {
        RuneManager.Singelton.AddListener(typeof(RuneManager.SpawnEvent), SpawnCharacter);
        RuneManager.Singelton.AddListener(typeof(RuneManager.NewController), NewController);

        players = new Dictionary<int, Controller>();
        charactersInGame = new List<SlideCharacter>();

        RuneManager.NewController newC = new RuneManager.NewController(0, System.Guid.NewGuid(), Controller.ControllerType.Player);
        RuneManager.NewController newC2 = new RuneManager.NewController(1, System.Guid.NewGuid(), Controller.ControllerType.AI);

        RuneManager.Singelton.ExecuteRune(newC);
        RuneManager.Singelton.ExecuteRune(newC2);

        RuneManager.SpawnEvent spaw = new RuneManager.SpawnEvent(0, new Vector2(0, 0), "Test", System.Guid.NewGuid());

        RuneManager.Singelton.ExecuteRune(spaw);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void NewController(RuneManager.Rune rune)
    {
        RuneManager.NewController controller = ((RuneManager.NewController)rune);
        switch(controller.type)
        {
            case Controller.ControllerType.Player:
                GameObject go = (GameObject)Resources.Load("Prefabs/PlayerController");
                go = Instantiate(go);
                players.Add(controller.team, go.GetComponent<PlayerController>());
                go.GetComponent<PlayerController>().Setup(controller.team, controller.guid);
                break;

            case Controller.ControllerType.AI:
                GameObject aiGo = (GameObject)Resources.Load("Prefabs/AIController");
                aiGo = Instantiate(aiGo);
                players.Add(controller.team, aiGo.GetComponent<AiController>());
                aiGo.GetComponent<AiController>().Setup(controller.team, controller.guid);
                break;

            default:
                break;
        }

    }

    Vector3 spawnUse;
    public void SpawnCharacter(RuneManager.Rune rune)
    {
        RuneManager.SpawnEvent spawn = ((RuneManager.SpawnEvent)rune);

        GameObject go = (GameObject)Resources.Load("Prefabs/Sphere");
        spawnUse.x = spawn.spawnPosition.x;
        spawnUse.y = 0;
        spawnUse.z = spawn.spawnPosition.y;
        go = Instantiate(go);
        go.name = spawn.characterName;
        go.transform.position = spawnUse;
        go.GetComponent<SlideCharacter>().Setup(100, 100, 2, spawn.guid, spawn.team);
        players[spawn.team].AddCrewMember(go.GetComponent<SlideCharacter>());
    }

}
