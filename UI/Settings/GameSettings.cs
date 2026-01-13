using UnityEngine;
using System;

[Serializable]
public class GameSettings
{
    public VolumeSettings volumeSettings = new VolumeSettings();
    public GeneralSettings generalSettings = new GeneralSettings();
}