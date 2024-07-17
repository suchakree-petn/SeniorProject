using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Services.Vivox;
using UnityEngine;

public class VoiceChatUIManager : Singleton<VoiceChatUIManager>
{
    private Dictionary<VivoxParticipant, GameObject> vcIconDict = new();

    [SerializeField] private Transform vcIconsParent;
    [SerializeField] private Transform vcIcon_prf;


    protected override void InitAfterAwake()
    {

    }

    private void Start()
    {
        if (!VivoxService.Instance.IsLoggedIn) return;

        foreach (VivoxParticipant vivoxParticipant in VoiceChatManager.Instance.VoiceChatMembers)
        {
            vivoxParticipant.ParticipantSpeechDetected += () =>
            {
                ShowVoiceChatIcon(vivoxParticipant);
            };
        }

    }

    private void ShowVoiceChatIcon(VivoxParticipant participant)
    {
        if (participant.SpeechDetected)
        {
            Transform vcIcon = Instantiate(vcIcon_prf, vcIconsParent);

            int classId = GameMultiplayerManager.Instance.GetPlayerDataFromPlayerId(participant.PlayerId).classId;
            string playerClass = classId == 0 ? "Tank" : classId == 1 ? "Archer" : "Caster";
            vcIcon.GetComponentInChildren<TextMeshProUGUI>().text = $"{participant.DisplayName} ({playerClass})";
            Debug.Log(participant.DisplayName + " " + participant.AudioEnergy);

            vcIconDict.TryAdd(participant, vcIcon.gameObject);

        }
        else
        {
            Destroy(vcIconDict[participant]);
            vcIconDict.Remove(participant);
        }


    }
}
