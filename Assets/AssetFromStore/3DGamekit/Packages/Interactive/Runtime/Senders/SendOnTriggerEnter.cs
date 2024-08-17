using UnityEngine;
using UnityEngine.Events;


namespace Gamekit3D.GameCommands
{

    public class SendOnTriggerEnter : TriggerCommand
    {
        public LayerMask layers;
        public UnityEvent OnEnter;

        void OnTriggerEnter(Collider other)
        {
            if (Time.time - lastSendTime < coolDown) return;

            if (0 != (layers.value & 1 << other.gameObject.layer))
            {
                Send();
                OnEnter?.Invoke();
            }
        }
    }
}
