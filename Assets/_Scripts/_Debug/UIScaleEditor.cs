using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if (UNITY_EDITOR) 
[CustomEditor(typeof(UIScale))]

public class UIScaleEditor : Editor
{
    // Start is called before the first frame update

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UIScale myScript = (UIScale)target;
        if (GUILayout.Button("ScaleUp"))
        {
            myScript.OnEditorClick();
        }
    }

}
#endif