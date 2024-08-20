using System;
using System.Collections.Generic;
using Mono.CSharp;
using TMPro;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLobbySetting_UI : MonoBehaviour
{
    private Dictionary<VivoxParticipant, GameObject> vcIconDict = new();


    [SerializeField] private Button joinedVoiceChannel;
    [SerializeField] private Button leaveVoiceChannel;
    [SerializeField] private Transform vcIconsParent;
    [SerializeField] private Transform vcIcon_prf;
    [SerializeField] private GameObject CanvasSettingGameObject;

    private void Awake()
    {
        joinedVoiceChannel.onClick.AddListener(VoiceChatManager.Instance.JoinVoiceChat);
        joinedVoiceChannel.onClick.AddListener(() =>
        {
            joinedVoiceChannel.interactable = false;
        });
        leaveVoiceChannel.onClick.AddListener(VoiceChatManager.Instance.LeaveVoiceChat);
        leaveVoiceChannel.onClick.AddListener(() =>
        {
            leaveVoiceChannel.interactable = false;
        });

        // Show join vc but can not interact
        joinedVoiceChannel.gameObject.SetActive(true);
        joinedVoiceChannel.interactable = false;

        // Hide leave vc
        leaveVoiceChannel.gameObject.SetActive(false);
    }

    private void Start()
    {
        VoiceChatManager.Instance.OnVivoxInit -= SetJoinButtonInteract;
        VivoxService.Instance.LoggedIn -= HideJoinShowLeave;
        VivoxService.Instance.LoggedIn -= ShowVoiceChatMembers;
        VivoxService.Instance.LoggedOut -= HideLeaveShowJoin;
        VivoxService.Instance.LoggedOut -= RemoveVoiceChatMembers;
        VivoxService.Instance.ParticipantAddedToChannel -= ShowVoiceChatMember;
        VivoxService.Instance.ParticipantRemovedFromChannel -= RemoveVoiceChatMember;


        // Join vc ready to join
        VoiceChatManager.Instance.OnVivoxInit += SetJoinButtonInteract;
        VivoxService.Instance.LoggedIn += HideJoinShowLeave;
        VivoxService.Instance.LoggedIn += ShowVoiceChatMembers;
        VivoxService.Instance.LoggedOut += HideLeaveShowJoin;
        VivoxService.Instance.LoggedOut += RemoveVoiceChatMembers;
        VivoxService.Instance.ParticipantAddedToChannel += ShowVoiceChatMember;
        VivoxService.Instance.ParticipantRemovedFromChannel += RemoveVoiceChatMember;

        CanvasSettingGameObject.SetActive(false);
    }


    private void HideLeaveShowJoin()
    {
        leaveVoiceChannel.gameObject.SetActive(false);
        joinedVoiceChannel.gameObject.SetActive(true);
        joinedVoiceChannel.interactable = true;
    }

    private void HideJoinShowLeave()
    {
        joinedVoiceChannel.gameObject.SetActive(false);
        leaveVoiceChannel.gameObject.SetActive(true);
        leaveVoiceChannel.interactable = true;
    }

    private void SetJoinButtonInteract()
    {
        joinedVoiceChannel.interactable = true;
    }

    private void ShowVoiceChatMember(VivoxParticipant participant)
    {
        Transform vcIcon = Instantiate(vcIcon_prf, vcIconsParent);
        vcIcon.GetComponentInChildren<TextMeshProUGUI>().text = $"{participant.DisplayName}";
        vcIconDict.TryAdd(participant, vcIcon.gameObject);

    }

    private void ShowVoiceChatMembers()
    {
        foreach (VivoxParticipant participant in VoiceChatManager.Instance.VoiceChatMembers)
        {
            Transform vcIcon = Instantiate(vcIcon_prf, vcIconsParent);
            vcIcon.GetComponentInChildren<TextMeshProUGUI>().text = $"{participant.DisplayName}";
            vcIconDict.TryAdd(participant, vcIcon.gameObject);
        }
    }

    private void RemoveVoiceChatMembers()
    {
        foreach (Transform vcIcon in vcIconsParent)
        {
            Destroy(vcIcon.gameObject);
            vcIconDict.Clear();
        }

    }

    private void RemoveVoiceChatMember(VivoxParticipant participant)
    {
        Destroy(vcIconDict[participant]);
        vcIconDict.Remove(participant);
    }


}
