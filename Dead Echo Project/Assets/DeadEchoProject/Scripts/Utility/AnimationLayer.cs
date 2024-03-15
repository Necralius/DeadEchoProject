using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NekraByte.Core.Enumerators;

public class AnimationLayer : MonoBehaviour
{
    public LayerBehavior    layerBehavior   = LayerBehavior.None;
    public string           layerName       = "layer_Name";
    public GameObject       layerObject     => gameObject;
    public Animator         animator        = null;

    private void Start() => TryGetComponent(out animator);
    public static AnimationLayer GetAnimationLayer(string layerName, List<AnimationLayer> layers)
    {
        return layers.Find(e => e.layerName == layerName);
    }
}