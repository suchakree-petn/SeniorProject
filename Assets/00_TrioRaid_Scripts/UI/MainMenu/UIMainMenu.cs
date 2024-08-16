using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private TimelineAsset MtoB;
    [SerializeField] private TimelineAsset BtoM;
    [SerializeField] private CinemachineVirtualCamera cam1;
    [SerializeField] private CinemachineVirtualCamera cam2;
    [SerializeField] private PlayableDirector playableDirector;
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private LobbyUI lobbyUI;
    [SerializeField] private Image trioRaidLogo;
    public bool mainMenu = true;
    private void Awake()
    {
        playButton.onClick.AddListener(() =>
        {
            ChangeSection();
        });
        settingButton.onClick.AddListener(() =>
        {
            //settingWindow
        });
        exitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        mainMenuButton.onClick.AddListener(() =>
        {
            ChangeSection();
        });
    }

    private void Start()
    {
        SetActiveUIMainMenu(true);
    }

    public void SetActiveUIMainMenu(bool active)
    {
        // playButton.gameObject.SetActive(active);
        // settingButton.gameObject.SetActive(active);
        // exitButton.gameObject.SetActive(active);
        if (active)
        {
            UnFadeButton(playButton);
            UnFadeButton(settingButton);
            UnFadeButton(exitButton);
            trioRaidLogo.DOFade(1, 0.3f);
        }
        else
        {
            FadeButton(playButton);
            FadeButton(settingButton);
            FadeButton(exitButton);
            trioRaidLogo.DOFade(0, 0.3f);
        }
    }

    private void UnFadeButton(Button button)
    {
        button.interactable = true;
        button.GetComponent<Image>().DOFade(1, 0.3f);
        button.GetComponentInChildren<TextMeshProUGUI>().DOFade(1, 0.3f);
    }

    private void FadeButton(Button button)
    {
        button.interactable = false;
        button.GetComponent<Image>().DOFade(0, 0.3f);
        button.GetComponentInChildren<TextMeshProUGUI>().DOFade(0, 0.3f);
    }

    private void ChangeSection()
    {
        if (mainMenu)
        {
            playableDirector.playableAsset = MtoB;
            playableDirector.Play();
            ChangeCameraPriority(2);
            mainMenu = false;
            UpdateUISection();

        }
        else
        {
            playableDirector.playableAsset = BtoM;
            playableDirector.Play();
            ChangeCameraPriority(1);
            mainMenu = true;
            UpdateUISection();
        }
    }
    private void UpdateUISection()
    {
        if (mainMenu)
        {
            lobbyUI.SetActiveUILobby(false);
        }
        else
        {
            SetActiveUIMainMenu(false);
        }
    }
    private void WhenCamStop(PlayableDirector aDirector)
    {
        if (mainMenu)
        {
            SetActiveUIMainMenu(true);
        }
        else
        {
            lobbyUI.SetActiveUILobby(true);
        }
    }
    private void ChangeCameraPriority(int cam)
    {
        if (cam == 1)
        {
            cam1.Priority = cam2.Priority + 1;
        }
        else
        {
            cam2.Priority = cam1.Priority + 1;
        }
    }
    private void OnEnable()
    {
        playableDirector.stopped += WhenCamStop;
    }
    private void OnDisable()
    {
        playableDirector.stopped -= WhenCamStop;
    }
}
