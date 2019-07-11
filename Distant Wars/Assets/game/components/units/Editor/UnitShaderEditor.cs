using System.Linq;
using Boo.Lang;
using UnityEditor;
using UnityEngine;

public class UnitShaderEditor : MaterialEditor
{
    public override void OnInspectorGUI ()
    {
        // Draw the default inspector.
        base.OnInspectorGUI ();
 
        // If we are not visible, return.
        if (!isVisible)
            return;
 
        // Get the current keywords from the material
        var mat = target as Material;
        var keyWords = mat.shaderKeywords;
 
        // Check to see if the keyword NORMALMAP_ON is set in the material.
        var style = 
            keyWords.Contains("HIGHLIGHTED") ? Style.Highlighted :
            keyWords.Contains("SELECTED") ? Style.Selected :
            Style.Default
        ;
        
        EditorGUI.BeginChangeCheck();
        // Draw a checkbox showing the status of normalEnabled
        style = (Style) EditorGUILayout.EnumPopup(style);
        // If something has changed, update the material.
        if (EditorGUI.EndChangeCheck())
        {
            // If our normal is enabled, add keyword NORMALMAP_ON, otherwise add NORMALMAP_OFF
            var keywords = new List<string>
            {
                style == Style.Highlighted ? "HIGHLIGHTED" : 
                style == Style.Selected ? "SELECTED" : 
                "DEFAULT"
            };
            mat.shaderKeywords = keywords.ToArray ();
            EditorUtility.SetDirty (mat);
        }
    }

    enum Style
    {
        Default,
        Highlighted,
        Selected
    }
}