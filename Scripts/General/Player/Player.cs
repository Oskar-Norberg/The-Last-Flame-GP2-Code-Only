using UnityEngine;

namespace General.Player
{
    [RequireComponent(typeof(PlayerEntity))]
    [RequireComponent(typeof(PlayerController))]
    public class Player : Singleton<Player>, IUpdateable
    {
        public PlayerEntity PlayerEntity { get; private set; }
        public PlayerController PlayerController { get; private set; }

        private new void Awake()
        {
            base.Awake();
            
            PlayerEntity = GetComponent<PlayerEntity>();
            PlayerController = GetComponent<PlayerController>();
        }

        public void CustomUpdate(float deltaTime)
        {
            PlayerController?.CustomUpdate(deltaTime);
        }

        public void CustomFixedUpdate(float fixedDeltaTime)
        {
            PlayerController?.CustomFixedUpdate(fixedDeltaTime);
        }
    }
}
