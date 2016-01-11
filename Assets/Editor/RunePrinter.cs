using UnityEngine;
using UnityEditor;
using System.Collections;

public class RunePrinter : EditorWindow {
    [MenuItem("Window/TuckerWindows")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof (RunePrinter));
    }

    void OnGUI()
    {
        if(!Application.isPlaying) return;
        foreach (var t in RuneManager.Singelton.gameRunes)
        {
            t.OnGUI();
        }
        Repaint();
    }
}
