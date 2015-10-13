using System;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;


public class RuneManager : MonoBehaviour
{
    
    public List<Rune> gameRunes;

    private Dictionary<Type, List<Action<Rune>>> runeEvents;

    public static RuneManager Singelton;

    void Awake()
    {
        Singelton = this;
        runeEvents = new Dictionary<Type, List<Action<Rune>>>();
        gameRunes = new List<Rune>();

        
    }

    public void ExecuteRune(Rune rune)
    {
        var type = rune.GetType();
        for (var i = 0; i < runeEvents[type].Count; i++)
        {
            runeEvents[type][i].Invoke(rune);
        }
        RecordAllRune(rune);
    }

    public void AddListener(Type type, Action<Rune> action)
    {
        if (!runeEvents.ContainsKey(type))
            runeEvents.Add(type, new List<Action<Rune>>());

        runeEvents[type].Add(action);
    }


    public void RecordAllRune(Rune rune)
    {
        gameRunes.Add(rune);
    }

    public abstract class Rune
    {
        public string name;
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

    }

    public class MoveEvent : Rune
    {
        public SlideCharacter mover;
        public Vector2 startPosition;
        public Vector2 endPosition;

        public MoveEvent(SlideCharacter Mover, Vector2 StartPosition, Vector2 EndPosition)
        {
            name = "MoveEvent";
            mover = Mover;
            startPosition = StartPosition;
            endPosition = EndPosition;
        }
    }


}
