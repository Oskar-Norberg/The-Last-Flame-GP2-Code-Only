using UnityEngine;
using System;

[Serializable]
public class VolumeSettings
{
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float dialogueVolume = 1f;
    [Range(0f, 1f)] public float ambienceVolume = 1f;

    [Range(0f, 1f)] public float masterVolumePreview = 1f;
    [Range(0f, 1f)] public float musicVolumePreview = 1f;
    [Range(0f, 1f)] public float sfxVolumePreview = 1f;
    [Range(0f, 1f)] public float dialogueVolumePreview = 1f;
    [Range(0f, 1f)] public float ambienceVolumePreview = 1f;
}