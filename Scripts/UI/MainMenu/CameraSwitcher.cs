using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private CinemachineCamera mainCamera;
    [SerializeField] private CinemachineCamera optionsCamera;
    [SerializeField] private CinemachineCamera playGameCamera;
    [SerializeField] private CinemachineCamera startingCamera;
    [SerializeField] private CinemachineCamera transitionCamera;
    [SerializeField] private float blendTime = 2f;
    private CinemachineCamera activeCamera;

    [SerializeField] private GameObject titlePanel;

    public void SwitchToNewCamera(CinemachineCamera currentCamera, CinemachineCamera newCamera)
    {
        StartCoroutine(BlendCameras(newCamera, currentCamera));
    }

    public void SwitchFromStartingCamera()
    {
        SwitchToNewCamera(mainCamera, startingCamera);
        titlePanel.SetActive(false);
    }

    public void SwitchToPlayGameCamera()
    {
        SwitchToNewCamera(playGameCamera, mainCamera);
    }
    
    public void SwitchAwayFromPlayGameCamera()
    {
        SwitchToNewCamera(mainCamera, playGameCamera);
    }
    
    public void SwitchToOptionsCamera()
    {
        SwitchToNewCamera(optionsCamera, mainCamera);
    }
    public void SwitchAwayFromOptionsCamera()
    {
        SwitchToNewCamera(mainCamera, optionsCamera);
    }

    public void SwitchToTransitionCamera()
    {
        SwitchToNewCamera(transitionCamera, playGameCamera);
    }
    
    private IEnumerator BlendCameras(CinemachineCamera fromCam, CinemachineCamera toCam)
    {
        fromCam.gameObject.SetActive(false);
        toCam.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(blendTime);
    }
}