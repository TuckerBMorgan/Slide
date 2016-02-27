using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;


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

public class SpellAction : CharacterAction {

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
        showSpellAction.target = EditorGUILayout.ToggleLeft(name, showSpellAction.target) ;
        

        if (EditorGUILayout.BeginFadeGroup(showSpellAction.faded))
        {
            EditorGUILayout.LabelField("TargetType: " + targetType.ToString());
            EditorGUILayout.LabelField("TargetFilter: " + targetFilter.ToString());
            EditorGUILayout.LabelField("TargetRange: " + targetRange.ToString());
            EditorGUILayout.LabelField("TargetAOE: " + targetAOE.ToString());
            EditorGUILayout.LabelField("Range: " + range.ToString());
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

        if(protoEvents.Count > 0)
        {
            obj += ",\n";
            obj += "\t\"Events\": [{\n";
            for(int i = 0;i<protoEvents.Count;i++)
            {
                if(i != 0)
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

        Debug.Log(character.name);
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
            if(total > (range * 2))
            {
                return false;
            }
        }
        Debug.Log("___");
        return true;

    }

    public override void PreformAction(Entity entity)
    {
        for(int i = 0;i<protoEvents.Count;i++)
        {
            runes.Add(protoEvents[i].CreateRune(character, entity));
        }
        for(int i = 0;i<runes.Count;i++)
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

        if (allAbilites.ContainsKey(fileName))
            return allAbilites[fileName];
        
        TextAsset text = Resources.Load("Spells/" + fileName) as TextAsset;

        JSONObject js = new JSONObject(text.text);
        SpellAction newSpell = new SpellAction();
        
        JSONObject setup = js["Setup"];

        newSpell.name = setup["Name"].str;
        int targetType = (int)setup["TargetType"].i;
        int targetFilter = (int)setup["TargetFilter"].i;
        int targetRange = (int)setup["TargetRange"].i;
        int targetAOE = (int)setup["TargetAOE"].i;

        newSpell.targetType = (TargetType)targetType;
        newSpell.targetFilter = (TargetFilter)targetFilter;
        newSpell.targetRange = (TargetRange)targetRange;
        if(newSpell.targetRange == TargetRange.Range)
        {
            newSpell.range = (int)setup["Range"].i;
        }
        else
        {
            newSpell.range = 1; 
        }
        newSpell.targetAOE = (TargetAOE)targetAOE;
       


        JSONObject arra = js["Events"];
        
        

        foreach(JSONObject key in arra.list)
        {
           switch(key.keys[0])
           {
               case DamageEventPrototype.EventTag:
                   newSpell.protoEvents.Add(DamageEventPrototype.Builder(key));
                   break;

               case SpawnEventPrototype.EventTag:
                   newSpell.protoEvents.Add(SpawnEventPrototype.Builder(key));
                   break;

               case BuffCastEventPrototype.EventTag:
                   break;
           }
        }
        allAbilites.Add(fileName, newSpell);

        return newSpell;
    }

}