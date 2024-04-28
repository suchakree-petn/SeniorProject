using UnityEngine;

public class BillboardWorldSpaceUI : MonoBehaviour
{
    bool IsInit = false;

    private void LateUpdate()
    {
        if (transform.root.TryGetComponent(out PlayerController playerController))
        {
            if (!IsInit)
            {
                if (playerController.IsOwner)
                {
                    gameObject.SetActive(false);
                    IsInit = true;
                    return;
                }
            }
        }

        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0);
    }
}
