using UnityEngine;
using UnityEngine.Events;

namespace Gamekit3D.GameCommands
{

    public class SendOnTriggerExit : TriggerCommand
    {
        public LayerMask layers;
        public UnityEvent OnExit;

        void OnTriggerExit(Collider other)
        {
            if (Time.time - lastSendTime < coolDown) return;

            if (0 != (layers.value & 1 << other.gameObject.layer))
            {
                Send();
                OnExit?.Invoke();
            }
        }
    }

}
