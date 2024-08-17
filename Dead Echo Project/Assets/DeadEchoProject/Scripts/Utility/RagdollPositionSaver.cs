using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class RagdollPositionSaver : MonoBehaviour
{
    [Serializable]
    public class TransformData
    {
        public string name;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public TransformData(string name, Vector3 pos, Quaternion rot, Vector3 scl)
        {
            this.name = name;
            position = pos;
            rotation = rot;
            scale = scl;
        }
    }

    [SerializeField] private Transform[] _ragdollBones;
    [SerializeField] private Transform _content;
    [SerializeField] private List<TransformData> savedTransforms;
    [SerializeField] private string savePath;

    public void SaveRagdollTransforms()
    {
        _ragdollBones = _content.GetComponentsInChildren<Transform>();

        savedTransforms = new List<TransformData>();

        foreach (var bone in _ragdollBones)
        {
            var data = new TransformData(bone.name, bone.localPosition, bone.localRotation, bone.localScale);
            savedTransforms.Add(data);
        }

        // Serializar para JSON e salvar em um arquivo
        string json = JsonUtility.ToJson(new TransformList(savedTransforms), true);
        File.WriteAllText(Application.dataPath + savePath + "/ragdollTransforms.json", json);

        Debug.Log("Transforms saved to " + Application.dataPath + savePath + "/ragdollTransforms.json");
    }

    [System.Serializable]
    public class TransformList
    {
        public List<TransformData> transforms;

        public TransformList(List<TransformData> transforms)
        {
            this.transforms = transforms;
        }
    }
}
