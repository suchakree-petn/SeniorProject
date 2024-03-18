using Unity.Netcode;
using UnityEngine;

public class BillboardWorldSpaceUI : MonoBehaviour
{
    bool IsInit = false;

    private void LateUpdate()
    {
        if (!IsInit)
        {
            if (transform.root.GetComponent<PlayerController>().IsOwner)
            {
                gameObject.SetActive(false);
                IsInit = true;
                return;
            }
        }
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0);
    }
}
