using System;
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

        public NewController(int Team, Guid guid, Controller.ControllerType type)
        {
            name = "NewController";
            team = Team;
            this.guid = guid;
            this.type = type;
        }

        public override void Execute(System.Action action)
        {
            switch(type)
            {
                case Controller.ControllerType.Player:
                    var go = Resources.Load("Prefabs/PlayerController") as GameObject;
                    go = Instantiate(go);
                    ConflictController.Instance.Players.Add(team, go.GetComponent<PlayerController>());
                    go.GetComponent<PlayerController>().Setup(team, guid);
                    ConflictController.Instance.AddNewControllerToGame(go.GetComponent<PlayerController>());
                    break;
                case Controller.ControllerType.AI:
                    var aiGo = Resources.Load("Prefabs/AIController") as GameObject;
                    aiGo = Instantiate(aiGo);
                    ConflictController.Instance.Players.Add(team, aiGo.GetComponent<AiController>());
                    aiGo.GetComponent<AiController>().Setup(team, guid);
                    ConflictController.Instance.AddNewControllerToGame(aiGo.GetComponent<AiController>());
                    break;
            }
            action();
        }

        public override void OnGUI()
        {
            EditorGUILayout.LabelField("NewController, Team: " + team + " GUID: " + guid + " Type : " +  type + "\n");
        }
    }

    public class SpawnEvent : Rune
    {
        public string characterName;
        public int team;
        public Vector2 spawnPosition;
        public Guid guid;
        
        public SpawnEvent(int Team, Vector2 SpawnPosition, string CharacterName, Guid guid)
        {
            name = "SpawnEvent";
            team = Team;
            spawnPosition = SpawnPosition;
            characterName = CharacterName;
            this.guid = guid;
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
            go.GetComponent<SlideCharacter>().Setup(100, 100, 2, guid, team);
            ConflictController.Instance.Players[team].AddCrewMember(go.GetComponent<SlideCharacter>());
            ConflictController.Instance.CharactersInGame.Add(guid, go.GetComponent<SlideCharacter>());
            go.GetComponent<SlideCharacter>().SetTile(GridController.Singelton.GetTile((int)spawnPosition.x, (int)spawnPosition.y));
            GridController.Singelton.GetTile((int)spawnPosition.x, (int)spawnPosition.y).Occupied = true;
            action();
        }

        public override void OnGUI()
        {
            EditorGUILayout.LabelField("SpawnEvent, CharacterName: " + characterName +" Team: " + team + " GUID: " + guid + "\n");
        }
    }

    public class DamageEvent : Rune
    {
        public SlideCharacter origin;
        public SlideCharacter target;
        public float amount;

        public DamageEvent(SlideCharacter Origin, SlideCharacter Target, float Amount)
        {
            name = "DamageEvent";
            origin = Origin;
            target = Target;
            amount = Amount;
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
            EditorGUILayout.LabelField("CheckActionPoints," + "Tea:" + team + "\n");
        }
    }


}
