using UnityEngine;
using System.Collections;

using MoonSharp.Interpreter;


public class LuaFile {
    
    private Script script;
    private string name;

    
    private void LoadFile(string fileName)
    {
        script = new Script();
        TextAsset ta = Resources.Load(fileName) as TextAsset;
        script.DoString(ta.text);
    }

    public void Setup(string fileName)
    {
        if(script == null)
        {
            LoadFile(fileName);
            name = fileName;
        }
    }

    public object GetValue(string value)
    {
        return script.Globals[value];
    }


    public string GetName()
    {
        return name;
    }
   	
}
