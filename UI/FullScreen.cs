using UnityEngine;

public class FullScreen : MonoBehaviour
{
public void ChangeFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        print("change screen mode");
    }
}
