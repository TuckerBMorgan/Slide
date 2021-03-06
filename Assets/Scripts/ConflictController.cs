﻿
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;

public class ConflictController : MonoBehaviour
{
    public Controller CurrentController;
    public List<Controller> ControllersInGame;
    public Dictionary<int, Controller> Players;
    public Dictionary<Guid, SlideCharacter> CharactersInGame;
    public AbilityButtonControl abilityButtonControl;
    public Material pulseTest;
    public GridController gridController;

    public Entity selectedEntity;

    public int TurnOrder;

    public static ConflictController Instance;

    public UIController uicontroller;

    void Awake()
    {
        Instance = this;
    }



    // Use this for initialization
    void Start()
    {
        ControllersInGame = new List<Controller>();
        Players = new Dictionary<int, Controller>();
        CharactersInGame = new Dictionary<Guid, SlideCharacter>();

        GridController.Singelton.ProduceGridFromFile("test_map");

        string[] teamList = new string[2];
        teamList[0] = "test_character";
        teamList[1] = "test_character";
        var newC = new RuneManager.NewController(0, System.Guid.NewGuid(), Controller.ControllerType.Player, teamList);
        RuneManager.Singelton.ExecuteRune(newC);

        var AiCon = new RuneManager.NewController(1, System.Guid.NewGuid(), Controller.ControllerType.AI, teamList);
        RuneManager.Singelton.ExecuteRune(AiCon);

        TurnOrder = -1;
        CurrentController = ControllersInGame[0];
        TickGame();

        var firstTurn = new RuneManager.RotateTurnForController(CurrentController);
        RuneManager.Singelton.ExecuteRune(firstTurn);

        AbilityButtonControl.Instance.ChangeSelectedCharacter(CurrentController.Crew[0]);


        /*
        ControllersInGame =  new List<Controller>();
	    TurnOrder = -1;

        abilityButtonControl.Setup();

        Players = new Dictionary<int, Controller>();
        CharactersInGame = new Dictionary<Guid, SlideCharacter>();

        var newC = new RuneManager.NewController(0, System.Guid.NewGuid(), Controller.ControllerType.Player, "Test");
        var newC2 = new RuneManager.NewController(1, System.Guid.NewGuid(), Controller.ControllerType.AI, "Test");

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
        */
    }

    // Update is called once per frame
    void Update()
    {

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
        for (var i = 0; i < mats.Length; i++)
        {
            newMats.Add(mats[i]);
        }

        newMats.Remove(pulseTest);
        rend.GetComponent<Renderer>().sharedMaterials = newMats.ToArray();
    }

    public void TickGame()
    {
        PreformVisionCheck();
    }


    private void PreformVisionCheck()
    {


        Tile[][] grid = GridController.Singelton.GetGrid();

        
        for (int x = 0; x < grid.Length; x++)
        {
            for (int y = 0; y < grid[0].Length; y++)
            {

            }
        }


        List<Tile> tiles = new List<Tile>();

        for (int i = 0; i < ControllersInGame.Count; i++)
        {
            Controller cc = ControllersInGame[i];
            for (int x = 0; x < cc.Crew.Count; x++)
            {
                for (int o = 0; o < ControllersInGame.Count; o++)
                {
                    if (cc.Team == ControllersInGame[o].Team)
                        continue;

                    for (int g = 0; g < ControllersInGame[o].Crew.Count; g++)
                    {
                        tiles = GridController.Singelton.Bresenhams(cc.Crew[x].getCurrentTile(), ControllersInGame[o].Crew[g].getCurrentTile());
                        for (int a = 0; a < tiles.Count; a++)
                        {
                            if (tiles[a].Occupied)
                            {
                                if (tiles[a].objectOn == ControllersInGame[o].Crew[g])
                                {
                                    //reveal check and maybe rune
                                    if(!cc.seenEnemies.Contains(ControllersInGame[o].Crew[g]))
                                    {
                                        cc.seenEnemies.Add(ControllersInGame[o].Crew[g]);
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
