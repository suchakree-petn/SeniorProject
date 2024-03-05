using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class GameLobbyManager
{
    [Header("Reference UI")]
    [SerializeField] Button createLobbyButton;
    [SerializeField] Button quickJoinLobbyButton;
}
