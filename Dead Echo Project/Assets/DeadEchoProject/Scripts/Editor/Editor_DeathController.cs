using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

[CustomEditor(typeof(DeathController))]
public class Editor_DeathController : Editor
{
    public override void OnInspectorGUI()
    {
        DeathController controller = (DeathController)target;

        if (GUILayout.Button("Enable Ragdoll")) 
            controller.ActivateRagdoll();
        if (GUILayout.Button("Disable Ragdoll")) 
            controller.DeactiveRagdoll();
        if (GUILayout.Button("Setup RigidBodies Settings")) 
            controller.SetUpRigidBodyConfigs();
        if (GUILayout.Button("Test Call Death"))
            controller.CallDeath();

        base.OnInspectorGUI();
    }
}
