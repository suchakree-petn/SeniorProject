using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PressurePadTriggerRoleRestrictController : PressurePadTriggerController
{

    [Title("Restrict Role")]
    [EnumToggleButtons]
    public PlayerRole RestrictRole = PlayerRole.Tank;

    [FoldoutGroup("Reference")]
    [SerializeField] private Image roleIcon;

    [FoldoutGroup("Reference")]
    [PreviewField(60)]
    [SerializeField] private Sprite tankIcon;

    [FoldoutGroup("Reference")]
    [PreviewField(60)]
    [SerializeField] private Sprite archerIcon;

    [FoldoutGroup("Reference")]
    [PreviewField(60)]
    [SerializeField] private Sprite casterIcon;

    private Vector3 originRoleIconLocalPosition;

    protected override void Awake()
    {
        base.Awake();
        originRoleIconLocalPosition = transform.localPosition;
    }

 
    protected override void OnTriggerEnter(Collider other)
    {
        if (!other.transform.root.TryGetComponent(out PlayerController playerController)) return;

        if (playerController.PlayerCharacterData.PlayerRole == RestrictRole)
        {
            roleIcon.gameObject.SetActive(false);
            base.OnTriggerEnter(other);
        }
        else
        {
            transform.localPosition = originRoleIconLocalPosition;
            roleIcon.transform.DOShakePosition(0.7f, new Vector2(0.3f, 0.3f)).SetEase(Ease.OutSine);
            Debug.Log("Your role can not ACTIVATE this pad");
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (!other.transform.root.TryGetComponent(out PlayerController playerController)) return;

        if (playerController.PlayerCharacterData.PlayerRole == RestrictRole)
        {
            roleIcon.gameObject.SetActive(true);
            base.OnTriggerExit(other);
        }
        else
        {
            Debug.Log("Your role can not DEACTIVATE this pad");
        }
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        roleIcon.sprite = RestrictRole switch
        {
            PlayerRole.Tank => tankIcon,
            PlayerRole.Archer => archerIcon,
            PlayerRole.Caster => casterIcon,
            _ => null
        };
    }
#endif
}
