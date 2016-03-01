using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using MoonSharp.Interpreter;
using System.Linq;
using System;
public enum TargetType
{
    Tile,
    Character,
    Either,
    Self
}

public enum TargetRange
{
    Melee,
    Range
}

public enum TargetAOE
{
    Single,
    Circle,
    Cross,
    Line
}

public enum TargetFilter
{
    Freindly,
    Enemy,
    Any
}

public class SpellAction : CharacterAction
{

    public TargetType targetType;
    public TargetFilter targetFilter;
    public TargetRange targetRange;
    public TargetAOE targetAOE;
    public SlideCharacter character;
    public int range;
    public List<PrototypeInterface> protoEvents;
    public List<RuneManager.Rune> runes;
    public AnimBool showSpellAction;

    private static Dictionary<string, SpellAction> allAbilites;

    public SpellAction()
    {
        protoEvents = new List<PrototypeInterface>();
        showSpellAction = new AnimBool(false);

        runes = new List<RuneManager.Rune>();
    }
    public override void DrawInspector()
    {
        showSpellAction.target = EditorGUILayout.ToggleLeft(name, showSpellAction.target);


        if (EditorGUILayout.BeginFadeGroup(showSpellAction.faded))
        {
            EditorGUILayout.LabelField("TargetType: " + targetType.ToString());
            EditorGUILayout.LabelField("TargetFilter: " + targetFilter.ToString());
            EditorGUILayout.LabelField("TargetRange: " + targetRange.ToString());
            EditorGUILayout.LabelField("TargetAOE: " + targetAOE.ToString());
            EditorGUILayout.LabelField("Range: " + range.ToString());
            EditorGUILayout.LabelField("Number of action events: " + protoEvents.Count.ToString());
            for (int i = 0; i < protoEvents.Count; i++)
            {
                protoEvents[i].DrawInspector();
            }
        }
        EditorGUILayout.EndFadeGroup();
    }

    public void DrawEditor()
    {

        targetType = (TargetType)EditorGUILayout.EnumPopup("Target Type", targetType);
        targetFilter = (TargetFilter)EditorGUILayout.EnumPopup("Target Filter", targetFilter);
        targetRange = (TargetRange)EditorGUILayout.EnumPopup("Target Range", targetRange);
        if (targetRange == TargetRange.Range)
        {
            range = EditorGUILayout.IntField("Range", range);
        }
        else
        {
            range = 1;
            EditorGUILayout.LabelField("Range value is 1 for melee spells");
        }
        targetAOE = (TargetAOE)EditorGUILayout.EnumPopup("Target AOE", targetAOE);
        for (int i = 0; i < protoEvents.Count; i++)
        {
            protoEvents[i].DrawEditor();
        }
    }

    public string convertToJSONString()
    {

        string obj = "";

        obj += "{\n";

        obj += "\t\"Setup\":{\n";
        obj += "\t\t\"Name\":" + name + ",\n";
        obj += "\t\t\"TargetType\":" + (int)targetType + ",\n";
        obj += "\t\t\"TargetFilter\":" + (int)targetFilter + ",\n";
        obj += "\t\t\"TargetRange\":" + (int)targetRange + ",\n";
        obj += "\t\t\"Range\":" + range + ",\n";
        obj += "\t\t\"TargetAOE\":" + (int)targetAOE + "\n";
        obj += "\t}";

        if (protoEvents.Count > 0)
        {
            obj += ",\n";
            obj += "\t\"Events\": [{\n";
            for (int i = 0; i < protoEvents.Count; i++)
            {
                if (i != 0)
                {
                    obj += ",\n";
                }
                obj += protoEvents[i].convertToJSONString();
            }
            obj += "\n\t}]\n";
        }

        obj += "}\n";

        return obj;
    }

    public override bool ValidateSelection(Entity entity)
    {
        if (character.GetActionPoints() <= 0) return false;
        switch (targetType)
        {
            case TargetType.Character:
                if (entity.GetEntityType() != "SlideCharacter")
                {
                    return false;
                }

                SlideCharacter ch = entity as SlideCharacter;

                switch (targetFilter)
                {
                    case TargetFilter.Enemy:
                        if (ch.Team == character.Team) return false;
                        break;
                    case TargetFilter.Freindly:
                        if (ch.Team != character.Team) return false;
                        break;
                    default:

                        break;
                }

                break;
            case TargetType.Tile:
                if (entity.GetEntityType() != "Tile") return false;
                break;
            case TargetType.Either:
                if (entity.GetEntityType() != "Tile" && entity.GetEntityType() != "SlideCharacter") return false;
                break;
            case TargetType.Self:
                if (entity != character) return false;
                break;
            default:
                return false;
        }



        var tile = entity.getCurrentTile();
        int xDif = Mathf.Abs(tile.X - character.currentTile.X);
        if (xDif > range)
        {
            return false;
        }
        int yDif = Mathf.Abs(tile.Y - character.currentTile.Y);
        if (yDif > range)
        {
            return false;
        }
        int total = xDif + yDif;
        if (targetRange == TargetRange.Melee)
        {
            if (total > 2)
            {
                return false;
            }
        }
        else
        {
            if (total > (range * 2))
            {
                return false;
            }
        }
        return true;

    }

    public override void PreformAction(Entity entity)
    {
        for (int i = 0; i < protoEvents.Count; i++)
        {
            runes.Add(protoEvents[i].CreateRune(character, entity));
        }
        for (int i = 0; i < runes.Count; i++)
        {
            RuneManager.Singelton.ExecuteRune(runes[i]);
        }

        var runeSetAp = new RuneManager.SetActionPoint(character.GetActionPoints() - 1, character);
        RuneManager.Singelton.ExecuteRune(runeSetAp);
    }

    public override void StartAction()
    {

    }

    public override void EndAction()
    {

    }

    public static SpellAction ParseAndCreateSpell(string fileName)
    {



        if (allAbilites == null)
            allAbilites = new Dictionary<string, SpellAction>();

        LuaFile file = new LuaFile();
        file.Setup( "Spells/" + fileName);
        SpellAction newSpell = new SpellAction();
        newSpell.name = file.GetValue("Name").ToString();
        newSpell.targetType = (TargetType)Convert.ToInt32(file.GetValue("TargetType"));
        newSpell.targetFilter = (TargetFilter)Convert.ToInt32(file.GetValue("TargetFilter"));
        newSpell.targetRange = (TargetRange)Convert.ToInt32(file.GetValue("TargetRange"));
        newSpell.targetAOE = (TargetAOE)Convert.ToInt32(file.GetValue("TargetAOE"));
        if(newSpell.targetRange == TargetRange.Range)
        {
            newSpell.range = Convert.ToInt32(file.GetValue("Range"));
        }
        else
        {
            newSpell.range = 1; 
        }

        Table table = (Table)file.GetValue("Events");
        Table insideTable;
        foreach(DynValue dy in table.Values)
        {
            insideTable = dy.Table;
            switch (insideTable.Get("Type").String)
            {
                case "DamageEvent":
                    newSpell.protoEvents.Add(DamageEventPrototype.Builder(insideTable));
                    break;
                case "SpawnEvent":
                    newSpell.protoEvents.Add(SpawnEventPrototype.Builder(insideTable));
                    break;
                case "BuffCastEvent":
                    newSpell.protoEvents.Add(BuffCastEventPrototype.Builder(insideTable));
                    break;
                default:
                    break;
            }
        
        }
            return newSpell;
    }

}