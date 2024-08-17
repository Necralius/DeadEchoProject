using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RagdollPositionSaver))]
public class Editor_RagdollPositionSaver : Editor
{
    public override void OnInspectorGUI()
    {
        RagdollPositionSaver ragdollSaver = (RagdollPositionSaver)target;

        if (GUILayout.Button("Save Ragdoll State")) 
            ragdollSaver.SaveRagdollTransforms();

        DrawDefaultInspector();
    }
}