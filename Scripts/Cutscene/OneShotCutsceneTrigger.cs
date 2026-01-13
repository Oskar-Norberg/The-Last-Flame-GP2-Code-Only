using General.Player;
using UnityEngine;

public class OneShotCutsceneTrigger : OneShotCutscene
{

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<PlayerController>(out var playerMovement))
            return;

        PlayCutscene();
    }
}
