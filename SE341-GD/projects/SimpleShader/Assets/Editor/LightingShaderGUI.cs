using UnityEngine;
using UnityEditor;
using System;

public class LightingShaderGUI : ShaderGUI
{
    MaterialEditor editor;
    MaterialProperty[] properties;
    Material target;

    enum SpecularChoice { True, False }
    string specKW = "USE_SPECULAR";

    public override void OnGUI(MaterialEditor editor, MaterialProperty[] properties)
    {
        this.editor = editor;
        this.properties = properties;
        this.target = editor.target as Material;

        //----------------------------------------------------

        MaterialProperty mainTex = FindProperty("_MainTex", properties);
        GUIContent mainTexLabel = new GUIContent(mainTex.displayName);
        editor.TextureProperty(mainTex, mainTexLabel.text);

        //----------------------------------------------------

        SpecularChoice specularChoice = SpecularChoice.False;
        if (target.IsKeywordEnabled(specKW))
            specularChoice = SpecularChoice.True;

        EditorGUI.BeginChangeCheck();
        specularChoice = (SpecularChoice)EditorGUILayout.EnumPopup(
            new GUIContent("Use Specular"), specularChoice);
        if (EditorGUI.EndChangeCheck())
        {
            if (specularChoice == SpecularChoice.True)
                target.EnableKeyword(specKW);
            else
                target.DisableKeyword(specKW);
        }

        if (specularChoice == SpecularChoice.True)
        {
            MaterialProperty shininess = FindProperty("_Shininess", properties);
            GUIContent shininessLabel = new GUIContent(shininess.displayName);
            editor.FloatProperty(shininess, shininessLabel.text);
        }
    }
}
