using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Vivox;
using UnityEngine;

public class VoiceChatManager : Singleton<VoiceChatManager>
{
    public Action OnVivoxInit;

    public string VoiceChannelName = "DefaultVoiceChannel";
    public List<VivoxParticipant> VoiceChatMembers = new();

    protected override void InitAfterAwake()
    {
    }

    async void Start()
    {
        VivoxService.Instance.LoggedIn += OnUserLoggedIn;
        VivoxService.Instance.LoggedOut += OnUserLoggedOut;
        VivoxService.Instance.ParticipantAddedToChannel += VoiceChatManager_OnParticipantAddedToChannel;
        VivoxService.Instance.ParticipantRemovedFromChannel += VoiceChatManager_OnParticipantRemovedFromChannel;

        await VivoxService.Instance.InitializeAsync();

        OnVivoxInit?.Invoke();

    }

    private void VoiceChatManager_OnParticipantRemovedFromChannel(VivoxParticipant participant)
    {
        VoiceChatMembers.Remove(participant);
    }

    private void VoiceChatManager_OnParticipantAddedToChannel(VivoxParticipant participant)
    {
        VoiceChatMembers = VivoxService.Instance.ActiveChannels[VoiceChannelName].ToList();
    }

    public async void JoinVoiceChat()
    {
        if (VivoxService.Instance.IsLoggedIn)
        {
            Debug.Log("Already login Vivox");
            return;
        }

        PlayerData playerData = GameMultiplayerManager.Instance.GetPlayerData();
        LoginOptions loginOptions = new()
        {
            PlayerId = playerData.playerId.ToString(),
            DisplayName = playerData.playerName.ToString()
        };
        await VivoxService.Instance.LoginAsync(loginOptions);
        Debug.Log("Login Vivox");
    }

    public async void LeaveVoiceChat()
    {
        if (!VivoxService.Instance.IsLoggedIn)
        {
            Debug.Log("Not login Vivox yet");
            return;
        }

        await VivoxService.Instance.LogoutAsync();
        Debug.Log("Logout Vivox");
    }

    private async void OnUserLoggedIn()
    {
        await VivoxService.Instance.JoinGroupChannelAsync(VoiceChannelName, ChatCapability.AudioOnly);
        Debug.Log($"Join voice channel: {VoiceChannelName}");
    }

    private async void OnUserLoggedOut()
    {
        await VivoxService.Instance.LeaveAllChannelsAsync();
        Debug.Log($"Leave all voice channels");
    }


}
