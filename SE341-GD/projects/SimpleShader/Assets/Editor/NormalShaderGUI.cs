using UnityEngine;
using UnityEditor;
using System;

public class NormalShaderGUI : ShaderGUI
{
    MaterialEditor editor;
    MaterialProperty[] properties;
    Material target;

    enum NormalChoice { True, False }
    string normKW = "USE_NORMAL";

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        this.editor = materialEditor;
        this.properties = properties;
        this.target = editor.target as Material;

        //----------------------------------------------

        MaterialProperty mainColor = FindProperty("_MainColor", properties);
        GUIContent mainColorLabel = new GUIContent(mainColor.displayName);
        editor.ColorProperty(mainColor, mainColorLabel.text);

        //----------------------------------------------

        NormalChoice normalChoice = NormalChoice.False;
        if (target.IsKeywordEnabled(normKW))
            normalChoice = NormalChoice.True;

        EditorGUI.BeginChangeCheck();
        normalChoice = (NormalChoice)EditorGUILayout.EnumPopup(
            new GUIContent("Show Normal"), normalChoice);
        if (EditorGUI.EndChangeCheck())
        {
            if (normalChoice == NormalChoice.True)
                target.EnableKeyword(normKW);
            else
                target.DisableKeyword(normKW);
        }
    }
}