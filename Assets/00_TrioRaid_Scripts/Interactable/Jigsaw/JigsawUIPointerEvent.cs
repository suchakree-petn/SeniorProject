using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JigsawUIPointerEvent : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerUpHandler
{
    public JigsawPiece JigsawPiece;
    [SerializeField] private bool isOnJigsawPanel = false;

    [SerializeField] private float dragScaleMultiplier = 1;
    private Vector3 originalScale;
    private Image image;
    private Transform restAreaParent;
    private Transform collectedParent;

    private void Awake()
    {
        originalScale = transform.localScale;
        image = GetComponent<Image>();
        restAreaParent = GameObject.FindGameObjectWithTag("RestAreaParent").transform;
        collectedParent = transform.parent;

    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.SetParent(null);
        transform.localPosition = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isOnJigsawPanel)
        {
            image.raycastTarget = false;
            transform.localScale *= dragScaleMultiplier;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        foreach (GameObject go in eventData.hovered)
        {
            if (go.CompareTag("JigsawPanel"))
            {
                isOnJigsawPanel = true;
                transform.SetParent(restAreaParent);
                return;
            }
        }
        transform.SetParent(collectedParent);
        transform.localScale = originalScale;

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isOnJigsawPanel)
        {
            JigsawPiece.PieceDirection = JigsawPiece.PieceDirection switch
            {
                Direction.Top => Direction.Right,
                Direction.Right => Direction.Bottom,
                Direction.Bottom => Direction.Left,
                Direction.Left => Direction.Top,
                _ => Direction.Top
            };
            float newRotation = JigsawPiece.PieceDirection switch
            {
                Direction.Top => 0,
                Direction.Bottom => 180,
                Direction.Left => 90,
                Direction.Right => -90,
                _ => 0
            };

            transform.localEulerAngles = new(0, 0, newRotation);
        }
    }
}
