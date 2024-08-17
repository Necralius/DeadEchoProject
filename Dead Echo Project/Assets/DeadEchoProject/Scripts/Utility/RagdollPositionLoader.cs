using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class RagdollPositionLoader : MonoBehaviour
{
    [System.Serializable]
    public class TransformData
    {
        public string name;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }

    [System.Serializable]
    public class TransformList
    {
        public List<TransformData> transforms;
    }

    [MenuItem("Ragdoll/Load Transforms")]
    public static void LoadRagdollTransforms()
    {
        string filePath = Application.dataPath + "/ragdollTransforms.json";
        if (!File.Exists(filePath))
        {
            Debug.LogError("Transform file not found at " + filePath);
            return;
        }

        string json = File.ReadAllText(filePath);
        TransformList loadedTransforms = JsonUtility.FromJson<TransformList>(json);

        foreach (var data in loadedTransforms.transforms)
        {
            Transform bone = FindTransformInScene(data.name);
            if (bone != null)
            {
                Undo.RecordObject(bone, "Load Ragdoll Transforms");
                bone.localPosition = data.position;
                bone.localRotation = data.rotation;
                bone.localScale = data.scale;

                EditorUtility.SetDirty(bone);
            }
            else Debug.LogWarning("Bone not found: " + data.name);
        }

        Debug.Log("Transforms loaded and applied.");
    }

    private static Transform FindTransformInScene(string name)
    {
        Transform[] allTransforms = FindObjectsByType<Transform>(FindObjectsSortMode.None);
        foreach (Transform t in allTransforms)
        {
            if (t.name == name)
                return t;
        }
        return null;
    }
}