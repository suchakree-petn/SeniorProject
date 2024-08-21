using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
public class JigsawBoardStandController : SerializedMonoBehaviour
{
    public Action OnJigsawPlaced;

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

    [ShowInInspector] public Dictionary<uint, bool> PlacedJigsaw = new();



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
    private float freeXSpeed;
    private float freeYSpeed;

    private void Start()
    {
        worldSpaceCanvas.SetActive(false);
        OnJigsawPlaced += CheckAllJigsawPlaced;
    }



    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.root.TryGetComponent(out PlayerController _)) return;

        worldSpaceCanvas.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.transform.root.TryGetComponent(out PlayerController _)) return;

        StopUseJigsawBoard();
        worldSpaceCanvas.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.transform.root.TryGetComponent(out PlayerController _)) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            StartUseJigsawBoard();
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            StopUseJigsawBoard();
        }


    }

    private void CheckAllJigsawPlaced()
    {
        int placedCount = 0;
        foreach (var jigsaw in PlacedJigsaw)
        {
            if (jigsaw.Value)
            {
                placedCount++;
            }
        }
        if (placedCount == PlacedJigsaw.Count)
        {
            Map2_JigsawScannerManager.Instance.gameObject.SetActive(false);
            StopUseJigsawBoard();
            Map2_PuzzleManager.Instance.EncounterBoss_ServerRpc();
        }
    }

    [HideIf("IsInUse")]
    [Button("Use")]
    public void StartUseJigsawBoard()
    {
        if (!IsInUse)
        {
            IsInUse = true;
            ShowJigsawฺBoard();
            interactButtonText.gameObject.SetActive(false);
            MouseCanMove(false);
            PlayerInputManager.Instance.SwitchViewMode.Disable();
            PlayerInputManager.Instance.Attack.Disable();
            PlayerManager.Instance.LocalPlayerController.SetCanUseAbilityE(false);
            PlayerManager.Instance.LocalPlayerController.SetCanUseAbilityQ(false);
            PlayerManager.Instance.LocalPlayerController.SetCanPlayerMove(false);
        }
    }

    [ShowIf("IsInUse")]
    [Button("Exit")]
    public void StopUseJigsawBoard()
    {
        if (IsInUse)
        {
            IsInUse = false;
            HideJigsawฺBoard();
            interactButtonText.gameObject.SetActive(true);
            MouseCanMove(true);
            PlayerInputManager.Instance.SwitchViewMode.Enable();
            PlayerInputManager.Instance.Attack.Enable();
            PlayerManager.Instance.LocalPlayerController.SetCanUseAbilityE(true);
            PlayerManager.Instance.LocalPlayerController.SetCanUseAbilityQ(true);
            PlayerManager.Instance.LocalPlayerController.SetCanPlayerMove(true);
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

    void MouseCanMove(bool mouseCanMove)
    {
        if (mouseCanMove)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            CameraManager.Instance.GetThirdPersonCamera().GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = freeXSpeed;
            CameraManager.Instance.GetThirdPersonCamera().GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = freeYSpeed;
            PlayerManager.Instance.LocalPlayerController.GetMouseMovement().CanCameraMove = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            freeXSpeed = CameraManager.Instance.GetThirdPersonCamera().GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed;
            freeYSpeed = CameraManager.Instance.GetThirdPersonCamera().GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed;

            CameraManager.Instance.GetThirdPersonCamera().GetComponent<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = 0;
            CameraManager.Instance.GetThirdPersonCamera().GetComponent<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = 0;
            PlayerManager.Instance.LocalPlayerController.GetMouseMovement().CanCameraMove = false;
        }
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
