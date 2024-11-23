using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using static NekraByte.Core.DataTypes;

[Serializable]
public class SaveData
{
    public string   saveName        = string.Empty;
    public int      ID              = 0;
    public int      saveSceneIndex  = 0;

    public DateTimeSerialized   saveTime        = null;
    public SaveDirectory        saveDirectory   = null;

    public SaveBaseData         saveBaseData    = null;

    public SaveData(string saveName, int ID)
    {
        this.saveName   = saveName + ID.ToString();
        saveTime        = new DateTimeSerialized(DateTime.Now);
        this.ID         = ID;

        saveBaseData    = new SaveBaseData();
        saveDirectory   = new SaveDirectory();
    }

    public Texture2D GetImage()
    {
        Texture2D newText = new Texture2D(2, 2);

        if (File.Exists(saveDirectory.screenshotPath))
            newText.LoadImage(File.ReadAllBytes(saveDirectory.screenshotPath));

        return newText;
    }

    public PlayerData GetPlayerData() => saveBaseData.playerSave;

    public void UpdateSaveHour() => saveTime = new DateTimeSerialized(DateTime.Now);
}

[Serializable]
public class SaveBaseData
{
    [Header("Player Data")]
    public PlayerData       playerSave      = new PlayerData();

    [Header("Main Saves Data")]
    public List<ObjectSave> objectsSaves    = new List<ObjectSave>();
}

[Serializable]
public class PlayerData
{
    public ObjectSave transformSave;
    public ObjectSave cameraSave;

    public float playerHealth = 0f;

    [Header("Gun Data")]
    public int GunID = 99;

    public List<GunDataConteiner.AmmoData> _guns = new List<GunDataConteiner.AmmoData>();
}

[Serializable]
public class SaveDirectory
{
    public string savePath          = string.Empty;
    public string saveFolderPath    = string.Empty;
    public string screenshotPath    = string.Empty;
    public int    saveID            = 0;
}

[Serializable]
public struct ObjectSave
{
    public int          InstanceID;
    public string       Name;
    public Vector3      Velocity;
    public Vector3      Position;
    public Quaternion   Rotation;
    public bool         Active;

    public ObjectSave(Transform objectToSave)
    {
        InstanceID  = objectToSave.GetInstanceID();
        Name        = objectToSave.gameObject.name;

        Position = objectToSave.position;
        Rotation = objectToSave.rotation;

        Active      = objectToSave.gameObject.activeInHierarchy;
        Velocity    = Vector3.zero;
    }

    public ObjectSave(GameObject objectToSave, Vector3 Velocity)
    {
        InstanceID  = objectToSave.GetInstanceID();
        Name        = objectToSave.gameObject.name;

        Position = objectToSave.transform.position;
        Rotation = objectToSave.transform.rotation;

        Active          = objectToSave.gameObject.activeInHierarchy;
        this.Velocity   = Velocity;
    }

    public ObjectSave(Transform objectToSave, bool saveLocalPosition)
    {
        InstanceID = objectToSave.GetInstanceID();
        Name = objectToSave.gameObject.name;

        Position = objectToSave.position;
        Rotation = objectToSave.rotation;

        Active = objectToSave.gameObject.activeInHierarchy;
        Velocity = Vector3.zero;
    }
}