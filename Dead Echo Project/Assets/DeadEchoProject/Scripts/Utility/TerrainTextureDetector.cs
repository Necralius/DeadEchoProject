using System;
using System.Collections.Generic;
using UnityEngine;
using static NekraByte.Core.DataTypes.FloorData;

[Serializable]
public class TerrainTextureDetector
{
    //Dependencies
    private Transform   _interactorTransform    = null;
    private Terrain     _terrainObject          = null;
    private Collider    _collider               = null;

    //Private Data
    private int posX = 0;
    private int posZ = 0;

    //Player State
    private bool isOnTerrain   = false;

    public List<TextureArea> _textureAreas = new List<TextureArea>();

    //Constructor
    public TerrainTextureDetector(GameObject objectOrigin, Collider collider)
    {
        _interactorTransform    = objectOrigin.transform;
        _collider               = collider;
    }

    // ----------------------------------------------------------------------
    // Name: UpdateData (Method)
    // Desc: This method verify if has any terrain object underneath the
    //       player, updating the player state.
    // ----------------------------------------------------------------------
    private void UpdateData()
    {

        if (Physics.Raycast(_interactorTransform.position, Vector3.down, out RaycastHit hit, _collider.bounds.extents.y + 0.5f))
        {
            if (hit.transform.CompareTag("Terrain"))
            {
                _terrainObject  = hit.transform.gameObject.GetComponent<Terrain>();
                isOnTerrain     = true;

                _textureAreas.Clear();

                foreach (var layer in _terrainObject.terrainData.terrainLayers)
                    _textureAreas.Add(new TextureArea(layer.name, 0));
            }
            else isOnTerrain = false;
        }
        else isOnTerrain = false;
    }

    // ----------------------------------------------------------------------
    // Name: UpdatePosition (Method)
    // Desc: This method updates the player position on the terrain alpha
    //       map.
    // ----------------------------------------------------------------------
    private void UpdatePosition()
    {
        UpdateData();

        if (!isOnTerrain)           return;
        if (_terrainObject == null) return;

        Vector3 terrainPosition = _interactorTransform.position - _terrainObject.transform.position;
        Vector3 mapPosition     = new Vector3(terrainPosition.x / _terrainObject.terrainData.size.x, 0, terrainPosition.z / _terrainObject.terrainData.size.z);

        float xCoord = mapPosition.x * _terrainObject.terrainData.alphamapWidth;
        float zCoord = mapPosition.z * _terrainObject.terrainData.alphamapHeight;

        posX = (int)xCoord;
        posZ = (int)zCoord;

        float[,,] splatMap  = _terrainObject.terrainData.GetAlphamaps(posX, posZ, 1, 1);

        for (int i = 0; i < _terrainObject.terrainData.terrainLayers.Length; i++)
        {
            string layerName = _terrainObject.terrainData.terrainLayers[i].name;
     
            _textureAreas[i] = new TextureArea(layerName, splatMap[0, 0, i]);
        }
    }

    // ----------------------------------------------------------------------
    // Name: GetCurrentTexture (Method)
    // Desc: This method returns the terrain texture that the player is
    //       stepping.
    // ----------------------------------------------------------------------
    public string GetCurrentTexture()
    {
        UpdatePosition();

        if (!isOnTerrain)           return "None";
        if (_terrainObject == null) return "None";

        string  greaterLayer        = _textureAreas[0].textureLayerName;
        float   greaterLayerValue   = 0f;

        for (int i = 0; i < _textureAreas.Count; i++)
        {
            if (_textureAreas[i].textureValue > greaterLayerValue) 
                greaterLayer = _textureAreas[i].textureLayerName;
        }

        return greaterLayer;
    }

    // ----------------------------------------------------------------------
    // Name: TextureArea (Struct)
    // Desc: This struct represents an terrain layer data holder, mainly it
    //       storages the layer name and the influence value.
    // ----------------------------------------------------------------------
    [Serializable]
    public struct TextureArea
    {
        public string   textureLayerName;
        public float    textureValue;

        public TextureArea(string name, float value)
        {
            textureLayerName    = name;
            textureValue        = value;
        }
    }
}