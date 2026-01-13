using System.Linq;
using UnityEngine;

public class SaveFile
{
    public SaveFile()
    {
        current_scene = 1;
        current_checkpoint = 0;
        maxOrbCount = 1;
        orbCount = 0;


        SaveSystem.LoadResources();

        unlock_checkpoints = new bool[SaveSystem.allCheckpoints.total];
        unlock_dialog = new bool[SaveSystem.allDialogs.cutscenes.Length];
        unlock_artifacts = new bool[SaveSystem.allArtifacts.total];
    }

    //Player resources
    //health
    //other resoureces? idk :P

    //Level progress
    public int current_scene;
    public int current_checkpoint;
    public int orbCount;
    public int maxOrbCount;

    public float[] campfireDurations;
    public float[] torchDurations;
    public float[] berryDurations;


    //Story progress

    public bool[] unlock_checkpoints;
    public bool[] unlock_dialog;
    public bool[] unlock_artifacts;
}