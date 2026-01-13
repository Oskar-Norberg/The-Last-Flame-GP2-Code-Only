using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;

public static class SaveSystem
{
    public static float lastTimeSaved;

    public static SaveFile save;

    const string _saveFileName = "save.txt";
    const string _folderName = "LastFlame";


    private const string _resourcePath = "SaveData";
    public static AllCheckpoints allCheckpoints;
    public static AllDialogs allDialogs;
    public static AllArtifacts allArtifacts;
    //public static AllCodexEntries allCodexEnteries;

    public static void Save()
    {
        lastTimeSaved = Time.time;

        LoadResources();

        string saveData = JsonUtility.ToJson(save);

        string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string fullPath = Path.Combine(documents, _folderName, _saveFileName);

        if (!File.Exists(fullPath))
        {
            Directory.CreateDirectory(Path.Combine(documents, _folderName));
            File.Create(fullPath).Close();
        }

        File.WriteAllText(fullPath, saveData);
    }
    public static void Load()
    {
        LoadResources();
        string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string fullPath = Path.Combine(documents, _folderName, _saveFileName);

        if (!File.Exists(fullPath))
        {
            ResetSave();
        }
        else
        {
            try
            {
                string saveData = File.ReadAllText(fullPath);
                save = JsonUtility.FromJson<SaveFile>(saveData);
            }
            catch(Exception e)
            { 
                Debug.LogError("Save file invalid");
                Debug.LogError(e);
                ResetSave();
            }
        }


        if (!IsValid())
        {
            Debug.LogError("Save file is invalid and should not be loaded. A new savefile will be made.");
            ResetSave();
        }

    }

    public static void ResetSave()
    {
        LoadResources();
        save = new SaveFile();
        Save();
    }

    public static async Awaitable LoadLevel()
    {
        if (save == null) Load();

        //ADD IN PROPPER SCENE LOADING LATER!!! - milo
        
        // TODO: /NEW: I had to comment this out because it causes some error with the EventSystem.
        // im sorry😭😭😭😭
        // Its fixed with the old one... i have no clue what the original problem was 🐐
        //await SceneManager.LoadSceneAsync(save.current_scene);
        await SceneTransition.TransitionScene(save.current_scene);

        CheckPoint[] checkPoints = GameObject.FindObjectsByType<CheckPoint>(FindObjectsSortMode.InstanceID);


        CheckPoint respawnPoint = null;
        foreach(var c in checkPoints)
        {
            if(c.saveIndex == save.current_checkpoint)
            {
                respawnPoint = c;
                break;
            }
        }

        var player = GameObject.FindFirstObjectByType<General.Player.Player>();

        Collider col = player.GetComponent<Collider>();
           
        //player.gameObject.SetActive(false);

        player.transform.position = respawnPoint.transform.position + 
            (Vector3)UnityEngine.Random.insideUnitCircle + 
            new Vector3(0, col.bounds.extents.y);

        await Awaitable.NextFrameAsync();

        //player.gameObject.SetActive(true);
    }

    public static void SaveCheckpoint(int scene, int checkpoint)
    {
        save.current_scene = scene;
        save.current_checkpoint = checkpoint;
        save.unlock_checkpoints[checkpoint] = true;

        Save();
    }

    public static bool IsValid()
    {
        return save.unlock_checkpoints.Length == allCheckpoints.checkpoints.Count &&
            save.unlock_dialog.Length == allDialogs.cutscenes.Length;
    }
    public static bool IsValid(SaveFile save)
    {
        return save.unlock_checkpoints.Length == allCheckpoints.checkpoints.Count &&
            save.unlock_dialog.Length == allDialogs.cutscenes.Length;
    }

    public static void LoadResources()
    {
        if (allCheckpoints == null) allCheckpoints = Resources.LoadAll<AllCheckpoints>(_resourcePath).First();
        if (allDialogs == null) allDialogs = Resources.LoadAll<AllDialogs>(_resourcePath).First();
        if (allArtifacts == null) allArtifacts = Resources.LoadAll<AllArtifacts>(_resourcePath).First();
    }



    public static bool HasSaveFile()
    {
        LoadResources();
        string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string fullPath = Path.Combine(documents, _folderName, _saveFileName);

        SaveFile save = null;

        if (!File.Exists(fullPath))
        {
            return false;
        }
        else
        {
            try
            {
                string saveData = File.ReadAllText(fullPath);
                save = JsonUtility.FromJson<SaveFile>(saveData);
            }
            catch (Exception e)
            {
                Debug.LogError("Save file invalid");
                Debug.LogError(e);
                return false;
            }
        }

        return IsValid(save);
    }
}
