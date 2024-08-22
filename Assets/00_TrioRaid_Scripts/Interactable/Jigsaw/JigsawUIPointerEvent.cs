using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JigsawUIPointerEvent : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public JigsawPiece JigsawPiece;
    [SerializeField] private bool isOnJigsawPanel = false;
    public JigsawBoardStandController JigsawBoardStandController;

    [SerializeField] private float dragScaleMultiplier = 1;
    private Vector3 originalScale;
    private Image image;
    private Transform restAreaParent;
    private Transform collectedParent;
    private Transform jigsawPanelParent;

    private void Awake()
    {
        originalScale = transform.localScale;
        image = GetComponent<Image>();
        restAreaParent = GameObject.FindGameObjectWithTag("RestAreaParent").transform;
        jigsawPanelParent = GameObject.FindGameObjectWithTag("JigsawPanel").transform.parent;
        collectedParent = transform.parent;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.SetParent(restAreaParent);
        transform.position = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isOnJigsawPanel)
        {
            image.raycastTarget = false;
            transform.localScale *= dragScaleMultiplier;
        }
        else
        {
            image.raycastTarget = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isOnJigsawPanel)
        {
            image.raycastTarget = true;
            foreach (GameObject go in eventData.hovered)
            {
                if (go.CompareTag("JigsawPanel"))
                {
                    isOnJigsawPanel = true;
                    transform.SetParent(restAreaParent);
                    CheckCondition(eventData);
                    return;
                }
            }
            transform.SetParent(collectedParent);
            transform.localScale = originalScale;
        }
        else
        {
            isOnJigsawPanel = false;

            image.raycastTarget = true;
            foreach (GameObject go in eventData.hovered)
            {
                if (go.CompareTag("JigsawPanel"))
                {
                    isOnJigsawPanel = true;
                    return;
                }
            }

            if (!isOnJigsawPanel)
            {
                transform.SetParent(collectedParent);
                transform.localScale = originalScale;
            }
        }

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
            CheckCondition(eventData);
        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    private void CheckCondition(PointerEventData eventData)
    {
        image.raycastTarget = false;

        foreach (Transform child in jigsawPanelParent)
        {
            if (child.TryGetComponent(out JigsawPieceHolder jigsawPieceHolder))
            {
                if (JigsawPiece.JigsawId == jigsawPieceHolder.SlotIndex)
                {
                    Vector3[] worldCorner = new Vector3[4];
                    jigsawPieceHolder.transform.GetComponent<RectTransform>().GetWorldCorners(worldCorner);
                    Rect rectB = new Rect(worldCorner[0], worldCorner[2] - worldCorner[0]);

                    bool isInBound = rectB.Contains(GetComponent<RectTransform>().position);
                    if (isInBound && JigsawPiece.PieceDirection == Direction.Top)
                    {
                        jigsawPieceHolder.PlaceJigsawPiece();
                        this.JigsawBoardStandController.CollectedJigsawDict[JigsawPiece] = false;
                        this.JigsawBoardStandController.PlacedJigsaw[JigsawPiece.JigsawId] = true;
                        this.JigsawBoardStandController.OnJigsawPlaced?.Invoke();
                        Destroy(gameObject);
                    }
                }
            }
        }
        image.raycastTarget = true;
    }
}
