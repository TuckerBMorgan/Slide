using System;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

public class RuneManager : MonoBehaviour
{

    private Dictionary<Type, List<Action<Rune>>> runeEvents; 

    private delegate void HoldingEvent();

    private HoldingEvent e;

    void Awake()
    {
        runeEvents = new Dictionary<Type, List<Action<Rune>>>();
   

        
    }

    public void ExectureRune(Rune rune)
    {
        var type = rune.GetType();
        for (var i = 0; i < runeEvents[type].Count; i++)
        {
            runeEvents[type][i].Invoke(rune);
        }

    }

    public void AddListener(Type type, Action<Rune> action)
    {
        if(runeEvents[type] == null)
            runeEvents[type] = new List<Action<Rune>>();

        runeEvents[type].Add(action);
    }


    public void RecordAllRune(Rune rune)
    {
       
    }

    public abstract class Rune
    {
        public string name;
    }

    public class NewCharacter : Rune
    {
        public string characterName;

        public int team;

        public NewCharacter(string CharacterName, int Team)
        {
            characterName = CharacterName;
            team = Team;
            name = "NewCharacter";
        }

        public NewCharacter()
        {
            name = "NewCharacter";
        }
    }

    public class DamageEvent : Rune
    {
        public SlideCharacter origin;
        public SlideCharacter damaged;
        public float amount;

        public DamageEvent(SlideCharacter Origin, SlideCharacter Damaged, float Amount)
        {
            
        }

    }



}
