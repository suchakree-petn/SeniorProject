using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Gamekit3D
{
    public class InteractOnButton : InteractOnTrigger
    {

        public KeyCode Button = KeyCode.F;
        public UnityEvent OnButtonPress;

        bool canExecuteButtons = false;

        protected override void ExecuteOnEnter(Collider other)
        {
            base.ExecuteOnEnter(other);
            canExecuteButtons = true;
        }

        protected override void ExecuteOnExit(Collider other)
        {
            base.ExecuteOnExit(other);
            canExecuteButtons = false;
        }

        void Update()
        {
            if (canExecuteButtons && Input.GetKeyDown(Button))
            {
                OnButtonPress.Invoke();
            }
        }

    }
}
