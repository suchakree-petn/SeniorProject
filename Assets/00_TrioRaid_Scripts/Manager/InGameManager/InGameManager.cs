using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class InGameManager : Singleton<InGameManager>
{
    [SerializeField, ReadOnlyGUI] private InGameState _inGameState = InGameState.Playing;
    public InGameState InGameState => _inGameState;

    public static Action<InGameState> OnStateChange;

    protected override void InitAfterAwake()
    {
        Application.targetFrameRate = Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value);

    }

    public void ChangeState(InGameState newState)
    {
        _inGameState = newState;
        OnStateChange?.Invoke(newState);
    }

}
public enum InGameState
{
    Playing,
    Pause
}
