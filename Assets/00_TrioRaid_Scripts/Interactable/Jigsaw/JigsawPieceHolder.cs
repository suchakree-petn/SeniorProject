using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class JigsawPieceHolder : MonoBehaviour
{
    [MinValue(1)]
    [MaxValue(9)]
    public uint slotIndex = 1;

    private Image jigsawImage;
    private Transform restAreaParent;

    private void Awake()
    {
        jigsawImage = GetComponent<Image>();
        restAreaParent = GameObject.FindGameObjectWithTag("RestArea").transform;
    }

    public void PlaceJigsawPiece()
    {
        jigsawImage.DOColor(Color.white, 1f);
    }

    private void Update()
    {
        Transform temp = restAreaParent;
        foreach (Transform child in temp)
        {
            float distance = Vector3.Distance(child.localPosition, transform.localPosition);
            float spriteRadius = jigsawImage.sprite.bounds.extents.magnitude;
            Debug.Log("Sprite rad: " + spriteRadius);
            if (distance < spriteRadius)
            {
                JigsawUIPointerEvent jigsawUIPointerEvent = child.GetComponent<JigsawUIPointerEvent>();
                if (!jigsawUIPointerEvent
                && jigsawUIPointerEvent.JigsawPiece.JigsawId == slotIndex
                && jigsawUIPointerEvent.JigsawPiece.PieceDirection == Direction.Top)
                {
                    PlaceJigsawPiece();
                    Destroy(restAreaParent.GetChild(child.GetSiblingIndex()).gameObject);
                    enabled = false;
                }
            }
        }
    }

}
