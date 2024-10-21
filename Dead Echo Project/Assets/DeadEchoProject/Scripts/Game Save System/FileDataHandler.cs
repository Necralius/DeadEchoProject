using System;
using System.IO;
using UnityEngine;
using static NekraByte.Core.DataTypes;

public class FileDataHandler
{
    private string dataDirPath      = "";
    private string dataFileName     = "";

    private string fullPath = "";

    public SaveData data;

    private bool _debug = true;

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath    = dataDirPath;
        this.dataFileName   = dataFileName;
    }

    public FileDataHandler(string fullPath)
    {
        this.fullPath = fullPath;
    }

    public FileDataHandler()
    {
        dataDirPath     = Application.persistentDataPath;
        dataFileName    = "";
    }

    public SaveDirectory EncapsulateData(SaveData data)
    {
        if (_debug)
            Debug.Log("Encapsulating data.");

        if (data == null) return null;
        SaveDirectory directoryData = data.saveDirectory;

        string fullPath = string.Empty;

        if (this.fullPath == string.Empty)
            fullPath = Path.Combine(dataDirPath, dataFileName);
        else fullPath = this.fullPath;

        if (!Directory.Exists(fullPath))
        {
            if (_debug)
                Debug.Log("Directory do not exists, creating file!");
            Directory.CreateDirectory(fullPath);
        }
        directoryData.saveFolderPath = fullPath;

        if (_debug)
            Debug.Log("Saving screenshot file!");
        directoryData.screenshotPath = ScreenshotTaker.Instance.SaveScreenshot(fullPath, dataFileName);

        fullPath = Path.Combine(fullPath, dataFileName + ".NBSV");

        directoryData.savePath = fullPath;

        data.UpdateSaveHour();
        data.saveDirectory = directoryData;
        string dataToStore = JsonUtility.ToJson(data, true);

        using (FileStream stream = new FileStream(fullPath, FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(stream)) writer.Write(dataToStore);
        }

        return directoryData;
    }

    public SaveData LoadGameState(string fullPath)
    {
        SaveData loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using(FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream)) dataToLoad = reader.ReadToEnd();
                }

                loadedData = JsonUtility.FromJson<SaveData>(dataToLoad);
            }
            catch(Exception e)
            {
                Debug.LogWarning(e.ToString());
            }
        }

        return loadedData;
    }

    #region - Application Data Handler -
    public ApplicationData LoadApplicationData()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        ApplicationData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream)) dataToLoad = reader.ReadToEnd();
                }

                loadedData = JsonUtility.FromJson<ApplicationData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        return loadedData;
    }

    public void EncapsulateApplicationData(ApplicationData data)
    {
        if (_debug)
            Debug.Log("Encapsulating application data!");
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream)) writer.Write(dataToStore);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }
    #endregion
}