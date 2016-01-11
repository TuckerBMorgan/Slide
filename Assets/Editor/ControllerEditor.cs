using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SlideCharacter))]

public class ControllerEditor : Editor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ((SlideCharacter)target).DrawInspector();

        EditorUtility.SetDirty(target);
    }
}
