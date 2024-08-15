using System;
using System.Collections.Generic;
using System.Linq;
using TMPro.Examples;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private List<CameraConteiner> _conteiners;
    [SerializeField] private int _maxPriority = 100;

    private void Awake()
    {
        List<CinemachineCamera> _allCams = FindObjectsByType<CinemachineCamera>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();

        _allCams.ForEach(e =>
        {
            _conteiners.Add(new CameraConteiner()
            {
                Camera = e,
                Tag = e.name
            });
        });
    }

    /// <summary>
    /// This method select a camera and set its priority to the higher possible, this method always overrides the current camera priority.
    /// </summary>
    /// <param name="camTag"> The tag related to the camera. </param>
    public void SetCameraPriority(string camTag) => SetCameraPriority(camTag, true);

    /// <summary>
    /// This method select a camera and set its priority to the higher possible.
    /// </summary>
    /// <param name="camTag"> The tag related to the camera.</param>
    /// <param name="overrideCurrentCamera"> If the current camera will be overrided or not, in this case all the other cameras priority will be seted to 0.</param>
    public void SetCameraPriority(string camTag, bool overrideCurrentCamera)
    {
        CameraConteiner selectedConteiner = _conteiners.Find(e => e.Tag == camTag);

        if (overrideCurrentCamera) 
            _conteiners.ForEach(e => e.Camera.Priority = 0);
        selectedConteiner.Camera.Priority = _maxPriority;
    }
}

[Serializable]
public class CameraConteiner
{
    public CinemachineCamera Camera;
    public string Tag;


}

[Serializable]
public class CameraTransition
{
    

}