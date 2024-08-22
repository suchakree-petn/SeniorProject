using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class JigsawPieceHolder : MonoBehaviour
{
    [MinValue(1)]
    [MaxValue(9)]
    public uint SlotIndex = 1;

    private Image jigsawImage;

    private void Awake()
    {
        jigsawImage = GetComponent<Image>();
    }

    public void PlaceJigsawPiece()
    {
        jigsawImage.DOColor(Color.white, 1f);
    }


}
