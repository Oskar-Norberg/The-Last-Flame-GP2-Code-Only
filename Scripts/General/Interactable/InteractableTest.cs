using UnityEngine;

namespace General.Interactable
{
    public class InteractableTest : Player.Interactable
    {
        public override void PlayerProximityEnter()
        {
        
        }

        public override void PlayerProximityStay()
        {
            transform.position = new Vector3(0, Mathf.Sin(Time.time), 0);
        }

        public override void PlayerProximityExit()
        {
        
        }

        public override void Interact()
        {
            transform.localScale = new Vector3(3, 3, 3);
        }
    }
}
