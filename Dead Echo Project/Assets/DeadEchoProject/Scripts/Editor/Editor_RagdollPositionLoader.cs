using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RagdollPositionLoader))]
public class Editor_RagdollPositionLoader : Editor
{
    public override void OnInspectorGUI()
    {
        RagdollPositionLoader ragdollSaver = (RagdollPositionLoader)target;

        if (GUILayout.Button("Load Ragdoll State"))
            RagdollPositionLoader.LoadRagdollTransforms();

        DrawDefaultInspector();
    }
}