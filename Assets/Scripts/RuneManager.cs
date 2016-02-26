﻿using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;


public class RuneManager : MonoBehaviour
{
    
    public List<Rune> gameRunes;

    public Queue<Rune> runePump; 

    private Dictionary<Type, List<Action<Rune, Action>>> runeEvents;

    public static RuneManager Singelton;

    public int currentCallbacks;

    void Awake()
    {
        Singelton = this;
        runeEvents = new Dictionary<Type, List<Action<Rune, Action>>>();
        gameRunes = new List<Rune>();
        currentCallbacks = 0;
        runePump = new Queue<Rune>();
    }

    public void ExecuteRune(Rune rune)
    {
        runePump.Enqueue(rune);
        if (currentCallbacks == 0)
        {
            PumpRuneQue();
        }
    }

    void PumpRuneQue()
    {
        var rune = runePump.Dequeue();
        CallbackNumberUp();
        rune.Execute(CallbackNumberDown);
        if (runeEvents.ContainsKey(rune.GetType()))
        {
            for (var i = 0; i < runeEvents[rune.GetType()].Count; i++)
            {
                CallbackNumberUp();
                runeEvents[rune.GetType()][i].Invoke(rune, CallbackNumberDown);
            }
        }
        RecordAllRune(rune);
    }



    public void AddListener(Type type, Action<Rune, Action> action)
    {
        if (!runeEvents.ContainsKey(type))
            runeEvents.Add(type, new List<Action<Rune, Action>>());

        runeEvents[type].Add(action);
    }

    public void CallbackNumberUp()
    {
        currentCallbacks++;
    }

    public void CallbackNumberDown()
    {
        currentCallbacks--;
        if (currentCallbacks != 0) return;
        if (runePump.Count > 0)
        {
            PumpRuneQue();   
        }
    }

    public void RecordAllRune(Rune rune)
    {
        gameRunes.Add(rune);
    }

    public abstract class Rune
    {
        public string name;
        public abstract void OnGUI();
        public abstract void Execute(System.Action action);
    }

    

    public class NewController : Rune
    {
        public int team;
        public Guid guid;
        public Controller.ControllerType type;
        public string[] teamList;

        public NewController(int Team, Guid guid, Controller.ControllerType type, string[] teamList)
        {
            name = "NewController";
            team = Team;
            this.guid = guid;
            this.type = type;
            this.teamList = teamList;
        }

        public override void Execute(System.Action action)
        {
            Debug.Log(team);
            GameObject go;
            switch(type)
            {
                case Controller.ControllerType.Player:
                    go = Resources.Load("Prefabs/PlayerController") as GameObject;
                    go = Instantiate(go);
                    ConflictController.Instance.Players.Add(team, go.GetComponent<PlayerController>());
                    go.GetComponent<PlayerController>().Setup(team, guid);
                    ConflictController.Instance.AddNewControllerToGame(go.GetComponent<PlayerController>());
                    break;
                case Controller.ControllerType.AI:
                    go = Resources.Load("Prefabs/AIController") as GameObject;
                    go = Instantiate(go);
                    ConflictController.Instance.Players.Add(team, go.GetComponent<AiController>());
                    go.GetComponent<AiController>().Setup(team, guid);
                    ConflictController.Instance.AddNewControllerToGame(go.GetComponent<AiController>());
                    break;
            }
            TextAsset text;
            JSONObject js;
            JSONObject setup;
            JSONObject actions;
            for (int i = 0; i < teamList.Length;i++ )
            {
                text = Resources.Load("Characters/" + teamList[i]) as TextAsset;
                js = new JSONObject(text.text);
                setup = js["Setup"];
                actions = js["Actions"];

                List<string> abilites = new List<string>();
                for(int x = 0;x<actions.list.Count;x++)
                {
                    abilites.Add(actions.list[x].str);
                }
                var newCharacter = new SpawnCharacterEvent(setup["Name"].str + ":" + team, (int)setup["BaseHealth"].i, (int)setup["BaseArmour"].i, (int)setup["BaseActionPoints"].i, System.Guid.NewGuid(), abilites, GridController.Singelton.GetTeamSpawnPoints(team).position, team);
                RuneManager.Singelton.ExecuteRune((Rune)newCharacter);
            }
                action();
        }

        public override void OnGUI()
        {
            EditorGUILayout.LabelField("NewController, Team: " + team + " GUID: " + guid + " Type : " +  type + "\n");
        }
    }

    public class SpawnCharacterEvent : Rune
    {
        public string characterName;
        public int team;
        public int baseHealth;
        public int baseArmour;
        public int baseActionPoints;
        public Vector2 spawnPosition;
        public Guid guid;
        public List<string> abilites;

        public SpawnCharacterEvent(string name, int baseHealth, int baseArmour, int baseActionPoints, Guid guid, List<string> abilites, Vector2 spawnPosition, int team)
        {
            this.characterName = name;
            this.baseHealth = baseHealth;
            this.baseArmour = baseArmour;
            this.baseActionPoints = baseActionPoints;
            this.abilites = abilites;
            this.guid = guid;
            this.spawnPosition = spawnPosition;
            this.team = team;
        }

        public override void Execute(System.Action action)
        {
            var go = Resources.Load("Prefabs/Sphere") as GameObject;
            var spawnPos = new Vector3
            {
                x = spawnPosition.x,
                y = 0,
                z = spawnPosition.y
            };
            
            go = Instantiate(go);
            go.transform.position = spawnPos;
            go.GetComponent<SlideCharacter>().Setup(characterName, baseHealth, baseArmour, baseActionPoints, guid, team);
            go.name = characterName;
            go.GetComponent<SlideCharacter>().SetTile(GridController.Singelton.GetTile((int)spawnPosition.x, (int)spawnPosition.y));

            for (int i = 0; i < abilites.Count;i++ )
            {
                var grantAction = new GrantAction(go.GetComponent<SlideCharacter>(), abilites[i]);
                RuneManager.Singelton.ExecuteRune(grantAction);
            }

            ConflictController.Instance.Players[team].AddCrewMember(go.GetComponent<SlideCharacter>());
            ConflictController.Instance.CharactersInGame.Add(guid, go.GetComponent<SlideCharacter>());
            GridController.Singelton.GetTile((int)spawnPosition.x, (int)spawnPosition.y).Occupied = true;

            var healthBar = Resources.Load("Prefabs/Health") as GameObject;
            healthBar = Instantiate(healthBar);
            healthBar.transform.SetParent( UIController.Singelton.transform);
            healthBar.GetComponent<HealthBarController>().character = go.GetComponent<SlideCharacter>();

            action();
        }

        public override void OnGUI()
        {
            EditorGUILayout.LabelField("SpawnEvent, CharacterName: " + characterName +" Team: " + team + " GUID: " + guid + "\n");
        }
    }

    public class SpawnCastEvent : Rune
    {
        public string spawnAsset;
        public int lifeLength;
        public SpawnEventPrototype.spawnableAIType controllerType;
        public Tile target;

        public SpawnCastEvent(SlideCharacter origin, Tile target, string spawnAsset, int lifeLength, SpawnEventPrototype.spawnableAIType controllerType)
        {
            this.spawnAsset = spawnAsset;
            this.lifeLength = lifeLength;
            this.target = target;
            this.controllerType = controllerType;
        }

        public override void Execute(Action action)
        {
            var go = Resources.Load("Prefabs/Cylinder") as GameObject;
            var spawnPos = new Vector3
            {
                x = target.GetUnityObject().transform.position.x,
                y = 0,
                z = target.GetUnityObject().transform.position.z
            };
            go = Instantiate(go);

            go.transform.position = spawnPos;
            



        }

        public override void OnGUI()
        {
            
        }

    }

    public class DamageEvent : Rune
    {
        
        public enum DamageType
        {
            physical,
            magic,
            pure
        }
        public SlideCharacter origin;
        public SlideCharacter target;
        public float amount;
        public DamageType damageType;

        public DamageEvent(SlideCharacter Origin, SlideCharacter Target, float Amount, DamageType damageType)
        {
            name = "DamageEvent";
            origin = Origin;
            target = Target;
            amount = Amount;
            this.damageType = damageType;
        }

        public override void Execute(System.Action action)
        {
            var armour = target.Armour;

            var damageAmount = amount*(armour/(armour + 100));

            target.Health -= damageAmount;

            action();
        }

        public override void OnGUI()
        {
            EditorGUILayout.LabelField("Damage Events, Origin" + origin.name + " target: " + target.name + " Amount: " + amount + "\n");
        }
    }

    public class MoveEvent : Rune
    {
        public SlideCharacter mover;
        public Tile startTile;
        public Tile endTile;

        public MoveEvent(SlideCharacter Mover, Tile StartTile, Tile EndTile)
        {
            name = "MoveEvent";
            mover = Mover;
            startTile = StartTile;
            endTile = EndTile;
        }

        public override void Execute(System.Action action)
        {
            mover.GetComponent<CharacterMovementController>().SetMoveTargetAndGo(endTile, action);
            startTile.Occupied = false;
            endTile.Occupied = true;
        }

        public override void OnGUI()
        {
            EditorGUILayout.LabelField("MoveEvent, Mover" + mover.name + "Start Position:" + startTile.name + " End Position:" + endTile.name + "\n");
        }
    }

    public class RotateTurnForController : Rune
    {
        public Controller currentController;

        public RotateTurnForController(Controller controller)
        {
            currentController = controller;
        }

        public override void Execute(System.Action action)
        {

            if(ConflictController.Instance.TurnOrder == ConflictController.Instance.ControllersInGame.Count)
            {
                ConflictController.Instance.TurnOrder = 0;
            }
            else
            {
                ConflictController.Instance.TurnOrder++;
            }

            ConflictController.Instance.CurrentController.EndTurn();
            ConflictController.Instance.CurrentController = ConflictController.Instance.ControllersInGame[ConflictController.Instance.TurnOrder];
            ConflictController.Instance.CurrentController.StartTurn();
            action();
        }

        public override void OnGUI()
        {
            EditorGUILayout.LabelField("RotateTurnForController, " + currentController.name + "\n");
        }
    }

    public class WaitForSelection : Rune
    {
        public Controller controller;

        public WaitForSelection(Controller controller)
        {
            this.controller = controller;
        }

        public override void Execute(System.Action action)
        {
            action();
        }

        public override void OnGUI()
        {
            EditorGUILayout.LabelField("WaitForSelection," + controller.name + "\n");
        }
    }

    public class SetActionPoint : Rune
    {
        public int amount;
        public SlideCharacter character;

        public SetActionPoint(int amount, SlideCharacter character)
        {
            this.amount = amount;
            this.character = character;
        }

        public override void Execute(System.Action action)
        {
            character.SetActionPoints(amount);
            action();
            var checkAp = new CheckActionPoints(character.Team);
            RuneManager.Singelton.ExecuteRune(checkAp);
        }

        public override void OnGUI()
        {
            EditorGUILayout.LabelField("SetActionPoints," + character.name + "'s AP changed by " + amount + "\n");
        }
    }
    public class CheckActionPoints : Rune
    {
        public int team;

        public CheckActionPoints(int team)
        {
            this.team = team;
        }

        public override void Execute(System.Action action)
        {
            var controller = ConflictController.Instance.ControllersInGame[team];
            var totolAp = controller.Crew.Sum(t => t.GetActionPoints());
            if(totolAp == 0)
            {
                var rotateController = new RotateTurnForController(ConflictController.Instance.CurrentController);
                RuneManager.Singelton.ExecuteRune(rotateController);
            }
                action();
        }

        public override void OnGUI()
        {
            EditorGUILayout.LabelField("CheckActionPoints," + "Team:" + team + "\n");
        }
    }

    public class GrantAction : Rune
    {
        public SlideCharacter character;
        public string actionID;

        public GrantAction(SlideCharacter character, string actionID)
        {
            this.character = character;
            this.actionID = actionID;
        }

        public override void Execute(Action action)
        {
            SpellAction sp = SpellAction.ParseAndCreateSpell(actionID);
            sp.character = character;
            character.AddAction(sp);
            action();
        }

        public override void OnGUI()
        {
            EditorGUILayout.LabelField("GrantAbility, Character: " + character.name + "Abilty id: " + actionID);    
        }
        
    }

    public class PresentMoveTiles : Rune
    {
        public SlideCharacter character;

        public PresentMoveTiles(SlideCharacter character)
        {
            this.character = character;
        }

        public override void Execute(Action action)
        {
            GridController.DisplayMoveRange(character);

            action();
        }

        public override void OnGUI()    
        {
            EditorGUILayout.LabelField("PresentMoveTiles, Character: " + character.name + "\n");
        }
    }


}
