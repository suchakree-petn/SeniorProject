using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
public class JigsawBoardStandController : SerializedMonoBehaviour
{
    public bool IsInUse = false;
    [ShowInInspector] public Dictionary<JigsawPiece, bool> CollectedJigsawDict = new();

    [ShowInInspector]
    public List<JigsawPiece> CollectedJigsawList
    {
        get
        {
            List<JigsawPiece> _collectedJigsawList = new();
            foreach (KeyValuePair<JigsawPiece, bool> piece in CollectedJigsawDict)
            {
                if (piece.Value)
                {
                    _collectedJigsawList.Add(piece.Key);
                }
            }
            return _collectedJigsawList;
        }
    }



    [FoldoutGroup("Reference")]
    [SerializeField] private GameObject worldSpaceCanvas;
    [FoldoutGroup("Reference")]
    [SerializeField] private GameObject overlaySpaceCanvas;

    [FoldoutGroup("Reference")]
    [SerializeField] private Transform jigsaw_prf;

    [FoldoutGroup("Reference")]
    [SerializeField] private TextMeshProUGUI interactButtonText;
    [FoldoutGroup("Reference")]
    [SerializeField] private Transform collectedJigsawParent;

    private void Start()
    {
        worldSpaceCanvas.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.root.TryGetComponent(out PlayerController _)) return;

        worldSpaceCanvas.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.transform.root.TryGetComponent(out PlayerController _)) return;

        StopUseJigsawฺBoard();
        worldSpaceCanvas.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.transform.root.TryGetComponent(out PlayerController _)) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            StartUseJigsaฺwBoard();
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            StopUseJigsawฺBoard();
        }


    }

    [HideIf("IsInUse")]
    [Button("Use")]
    public void StartUseJigsaฺwBoard()
    {
        if (!IsInUse)
        {
            IsInUse = true;
            ShowJigsawฺBoard();
            interactButtonText.gameObject.SetActive(false);
        }
    }

    [ShowIf("IsInUse")]
    [Button("Exit")]
    public void StopUseJigsawฺBoard()
    {
        if (IsInUse)
        {
            IsInUse = false;
            HideJigsawฺBoard();
            interactButtonText.gameObject.SetActive(true);
        }
    }

    private void ShowJigsawฺBoard()
    {
        foreach (JigsawPiece jigsawPiece in CollectedJigsawList)
        {
            Transform jigsaw_prf = Instantiate(this.jigsaw_prf, collectedJigsawParent);
            InitJigsawPrefab(jigsaw_prf, jigsawPiece);
        }
        overlaySpaceCanvas.SetActive(true);
    }

    private void HideJigsawฺBoard()
    {
        overlaySpaceCanvas.SetActive(false);
        foreach (Transform child in collectedJigsawParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void InitJigsawPrefab(Transform jigsaw_prf, JigsawPiece jigsawPiece)
    {
        jigsaw_prf.GetComponent<Image>().sprite = jigsawPiece.Sprite;

        float rotation = jigsawPiece.PieceDirection switch
        {
            Direction.Top => 0,
            Direction.Bottom => 180,
            Direction.Left => 90,
            Direction.Right => -90,
            _ => 0
        };

        jigsaw_prf.localEulerAngles = new(0, 0, rotation);

        JigsawUIPointerEvent jigsawUIPointerEvent = jigsaw_prf.GetComponent<JigsawUIPointerEvent>();
        jigsawUIPointerEvent.JigsawPiece = jigsawPiece;
        jigsawUIPointerEvent.JigsawBoardStandController = this;

    }
}

[Serializable]
public class JigsawPiece
{
    public uint JigsawId;
    [EnumToggleButtons]
    public Direction PieceDirection;
    public Sprite Sprite;
}
