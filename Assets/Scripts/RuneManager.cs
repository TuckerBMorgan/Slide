using System;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
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

        for (var i = 0; i < runeEvents[rune.GetType()].Count; i++)
        {
            CallbackNumberUp();
            runeEvents[rune.GetType()][i].Invoke(rune, CallbackNumberDown);
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

        public override void OnGUI()
        {
            EditorGUILayout.LabelField("RotateTurnForController, " + currentController.name + "\n");
        }
    }

    public class RotateTurnForCharacter : Rune
    {
        public Controller currentController;
        public SlideCharacter character;

        

        public override void OnGUI()
        {
            EditorGUILayout.LabelField("RotateTurnForCharacter, CurrentController "  + currentController.name + " Character Starting Turn " + character + "\n");   
        }
    }


    public class WaitForTileInput : Rune
    {
        public Controller controller;

        public WaitForTileInput(Controller controller)
        {
            this.controller = controller;
        }

        public override void OnGUI()
        {
            EditorGUILayout.LabelField("WaitForTileInput," + controller.name + "\n");
        }
    }


}
