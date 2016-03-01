using UnityEngine;
using UnityEditor;
using System.Collections;
using MoonSharp.Interpreter;

public abstract class PrototypeInterface{
    public void ProcessEvent()
    {

    }
    public virtual void DrawInspector()
    {

    }
    public virtual RuneManager.Rune CreateRune(Entity executingEntity, Entity targetEntity)
    {
        return null;
    }

    public virtual void DrawEditor()
    {

    }
    public virtual string convertToJSONString()
    {
        return " NotImplemented";
    }
}


public class SpawnEventPrototype : PrototypeInterface
{

    public enum spawnableAIType
    {
        dumb
    }
    
    public string spawnAsset;
    public int lifeLength;
    public spawnableAIType controllerType;

    public const string EventTag = "SpawnEvent";

    public static SpawnEventPrototype Builder(Table table)
    {
        SpawnEventPrototype proto = new SpawnEventPrototype();
        PullData(table, proto);
        return proto;
    }

    private static void PullData(Table table, SpawnEventPrototype proto)
    {
        proto.lifeLength = (int)table.Get("LifeLength").Number;
        proto.controllerType = (spawnableAIType)(int)table.Get("LifeLength").Number;
        proto.spawnAsset = table.Get("Spawnable").String;
    }
}

public class DamageEventPrototype : PrototypeInterface
{

    public int amount;
    public RuneManager.DamageEvent.DamageType damageType;
    public const string EventTag = "DamageEvent";

    public static DamageEventPrototype Builder(Table table)
    {
        DamageEventPrototype proto = new DamageEventPrototype();
        DataPull(table, proto);
        return proto;
    }

    private static void DataPull(Table table, DamageEventPrototype proto)
    {

        proto.amount = (int)table.Get("Amount").Number;
        proto.damageType = (RuneManager.DamageEvent.DamageType)(int)table.Get("DamgeType").Number;
    }

    public override void DrawEditor()
    {
        
        GUI.backgroundColor = Color.red;

        EditorGUILayout.LabelField("Damage Event");
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        amount = EditorGUILayout.IntField("Damage Amount", amount);
        damageType = (RuneManager.DamageEvent.DamageType)EditorGUILayout.EnumPopup("Damage Type", damageType);
        EditorGUILayout.EndVertical();
        GUI.backgroundColor = Color.white;
        
    }


    public override string convertToJSONString()
    {
        string obj = "";
        obj += "\t\t\"DamageEvent\":{\n";
        obj += "\t\t\t\"Amount\":" + amount + ",\n";
        obj += "\t\t\t\"DamageType\":" + (int)damageType + "\n";
        obj += "\t\t}";
        return obj;
    }

    public override RuneManager.Rune CreateRune(Entity executingEntity, Entity targetEntity)
    {
        RuneManager.DamageEvent de = new RuneManager.DamageEvent((SlideCharacter)executingEntity, (SlideCharacter)targetEntity, amount, damageType);
        return de;
    }

    public override void DrawInspector()
    {
        EditorGUILayout.LabelField("Damage Event");
        EditorGUILayout.LabelField("Damage Amount: " + amount);
        EditorGUILayout.LabelField("Damage Type: "  + damageType.ToString());
    }

}

public class BuffCastEventPrototype : PrototypeInterface
{
    public int timer;
    public stats stat;
    public int amount;
    public const string EventTag = "BuffCastEvent";

    public static BuffCastEventPrototype Builder(Table table)
    {
        BuffCastEventPrototype proto = new BuffCastEventPrototype();
        PullData(table, proto);    
        return null;
    }


    private static void PullData(Table table, BuffCastEventPrototype proto)
    {
        proto.timer = (int)table.Get("timer").Number;
        proto.stat = (stats)(int)table.Get("stat").Number;
        proto.amount = (int)table.Get("amount").Number;
    }


}
