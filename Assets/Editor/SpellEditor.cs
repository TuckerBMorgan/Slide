using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class SpellEditor : EditorWindow {



    public static SpellAction workingSpell;
    public static List<string> currentSpells;
    public static string[] currentSpellsAsArray;
    
    [MenuItem("Editors/Spell")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SpellEditor));
        DirectoryInfo d = new DirectoryInfo("Assets/Resources/Spells");
        currentSpells = new List<string>();
        FileInfo[] files = d.GetFiles("*.json");
        foreach(FileInfo file in files)
        {
            currentSpells.Add(file.Name.Substring(0, file.Name.LastIndexOf('.')));
        }

        currentSpells.Insert(0, "-");
        currentSpellsAsArray = currentSpells.ToArray();

    }
    public int popupselction = 0;
    void OnGUI()
    {
        Repaint();
        EditorUtility.SetDirty(this);

        popupselction = EditorGUILayout.Popup(popupselction, currentSpellsAsArray);
        if (workingSpell == null)
        {
            if (popupselction != 0)
            {
                try
                {
                    workingSpell = SpellAction.ParseAndCreateSpell("Spells/" + currentSpellsAsArray[popupselction]);
                }
                catch
                {
                    workingSpell = null;
                }
            }
        }
        else
        {
            workingSpell.DrawEditor();

            if (GUILayout.Button("Save"))
            {
                FileStream f = File.Open("Assets/Resources/Spells/" + currentSpellsAsArray[popupselction] + ".json", FileMode.OpenOrCreate);
                string contents = workingSpell.convertToJSONString();
                f.Write(Encoding.ASCII.GetBytes(contents), 0, Encoding.ASCII.GetByteCount(contents));
                f.Close();
            }
        }

    }


    void OnDestroy()
    {
        workingSpell = null;

    }

}
public class SpellLoader : EditorWindow
{
    void OnGUI()
    {
        for(int i = 0;i<SpellEditor.currentSpells.Count;i++)
        {
            if(GUILayout.Button(SpellEditor.currentSpells[i]))
            {
                SpellEditor.workingSpell = SpellAction.ParseAndCreateSpell(("Spells/" + SpellEditor.currentSpells[i]).Replace(".txt",""));
                GUI.FocusWindow(0);
                Close();
            }
        }
    }
}