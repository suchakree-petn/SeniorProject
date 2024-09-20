using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

[SelectionBase]
public class HornController : MonoBehaviour
{
    [SerializeField] ulong hornId;
    public ulong HornId => hornId;
    public bool IsActive;

    [SerializeField] PlayerController usingPlayer;

    [FoldoutGroup("Config")][SerializeField] private string interact_text;
    [FoldoutGroup("Config")][SerializeField] private string using_text;

    [FoldoutGroup("Reference")][SerializeField] private TextMeshProUGUI interactButton;
    Canvas canvas;

    private void Awake()
    {
        canvas = interactButton.transform.parent.GetComponent<Canvas>();
        canvas.gameObject.SetActive(true);
        interactButton.gameObject.SetActive(true);

        interactButton.text = "";
    }


    private void Start()
    {
        Map5_HornManager.Instance.ActiveHorns.Add(hornId, this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && usingPlayer)
        {
            StartUsing();
        }

        if (Input.GetKeyUp(KeyCode.F) && usingPlayer)
        {

            StopUsing();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.root.TryGetComponent(out PlayerController player)) return;

        if (!player.IsLocalPlayer) return;

        interactButton.text = interact_text;


        usingPlayer = player;

    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.transform.root.TryGetComponent(out PlayerController player)) return;

        if (!player.IsLocalPlayer) return;

        interactButton.text = "";

        StopUsing();

        usingPlayer = null;


    }



    public void StartUsing()
    {
        // IsActive = true;
        interactButton.text = using_text;

        // StartUsing_ServerRpc();
        Map5_HornManager.Instance.UseHorn_ServerRpc(hornId);


    }
    public void StopUsing()
    {
        // IsActive = false;

        if (usingPlayer)
        {
            interactButton.text = interact_text;
        }
        else
        {
            interactButton.text = "";
        }
        // StopUsing_ServerRpc();

        Map5_HornManager.Instance.StopUseHorn_ServerRpc(hornId);


    }

    // [ServerRpc(RequireOwnership = false)]
    // public void StartUsing_ServerRpc()
    // {
    //     isActive.Value = true;
    //     StartUsing_ClientRpc();
    // }

    // [ServerRpc(RequireOwnership = false)]
    // public void StopUsing_ServerRpc()
    // {
    //     isActive.Value = false;
    //     StopUsing_ClientRpc();

    // }

    // [ClientRpc]
    // public void StartUsing_ClientRpc()
    // {
    //     Map5_HornManager.Instance.CurrentActiveHorns.Add(this);
    // }

    // [ClientRpc]
    // public void StopUsing_ClientRpc()
    // {
    //     Map5_HornManager.Instance.CurrentActiveHorns.Remove(this);
    // }

}
