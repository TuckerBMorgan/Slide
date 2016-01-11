using UnityEngine;
using UnityEditor;
using System.Collections;

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
    public string spawnAsset;
    public int lifeLength;

    public const string EventTag = "SpawnEvent";

    public static SpawnEventPrototype Builder(JSONObject Obj)
    {
        SpawnEventPrototype proto = new SpawnEventPrototype();

        RecursivePull(Obj, proto);

        return proto;
    }

    private static void RecursivePull(JSONObject obj, SpawnEventPrototype proto)
    {
        switch(obj.type)
        {
            case JSONObject.Type.OBJECT:
                JSONObject j;
                for (int i = 0; i < obj.list.Count;i++ )
                {
                    j = (JSONObject)obj.list[i];
                    RecursivePull(j, proto);
                }
                    break;

            case JSONObject.Type.STRING:
                    proto.spawnAsset = obj.str;   
                break;

            case JSONObject.Type.NUMBER:
                proto.lifeLength = (int)obj.n;
                    break;
        }
    }
}

public class DamageEventPrototype : PrototypeInterface
{

    public int amount;
    public RuneManager.DamageEvent.DamageType damageType;
    public const string EventTag = "DamageEvent";

    public static DamageEventPrototype Builder(JSONObject Obj)
    {
        DamageEventPrototype proto = new DamageEventPrototype();

        RecursivePull(null, Obj, proto);

        return proto;
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

    private static void RecursivePull(string Key, JSONObject obj, DamageEventPrototype proto)
    {
        switch (obj.type)
        {
            case JSONObject.Type.OBJECT:
                string key;
                JSONObject j;
                for (int i = 0; i < obj.list.Count; i++)
                {
                    key = (string)obj.keys[i];
                    j = (JSONObject)obj.list[i];
                    RecursivePull(key, j, proto);
                }
                break;
            case JSONObject.Type.NUMBER:
                if (Key == "Amount")
                {
                    proto.amount = (int)obj.n;
                }
                else if (Key == "DamageType")
                {
                    proto.damageType = (RuneManager.DamageEvent.DamageType)obj.n;
                }
                break;
        }
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

    public static BuffCastEventPrototype Builder(JSONObject Obj)
    {
        BuffCastEventPrototype proto = new BuffCastEventPrototype();
        RecursivePull(null, Obj, proto);
        return proto;
    }

    private static void RecursivePull(string Key, JSONObject obj, BuffCastEventPrototype proto)
    {
        switch (obj.type)
        {
            case JSONObject.Type.OBJECT:
                string key;
                JSONObject j;
                for (int i = 0; i < obj.list.Count; i++)
                {
                    key = (string)obj.keys[i];
                    j = (JSONObject)obj.list[i];
                    RecursivePull(key, j, proto);
                }
                break;
            case JSONObject.Type.NUMBER:
                if(Key == "timer")
                {
                    proto.timer = (int)obj.n;
                }
                else if(Key == "stat")
                {
                    proto.stat = (stats)obj.n;
                }
                else if(Key == "amount")
                {
                    proto.amount = (int)obj.n;
                }
                break;
        }
    }

}
