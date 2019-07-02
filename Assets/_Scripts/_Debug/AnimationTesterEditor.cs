using UnityEngine;
using System.Collections;
using UnityEditor;
#if (UNITY_EDITOR) 
[CustomEditor(typeof(TaikoCharacter))]
public class AnimationTesterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TaikoCharacter myScript = (TaikoCharacter)target;
        if (GUILayout.Button("PlaySelectedAnimation"))
        {
            myScript.EditorPlayAction();
        }
    }
}
#endif