using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterClassSelectSingleUI : MonoBehaviour
{


    [SerializeField] private int classId;
    // [SerializeField] private Image image;
    [SerializeField] private GameObject selectedGameObject;


    private void Awake()
    {

    }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log("Change class to " + classId);
            GameMultiplayerManager.Instance.ChangePlayerClass(classId);
        });
        GameMultiplayerManager.Instance.OnPlayerDataNetworkListChanged += GameMultiplayer_OnPlayerDataNetworkListChanged;
        // image.color = GameMultiplayerManager.Instance.GetPlayerClass(classId);
        UpdateIsSelected();
    }

    private void GameMultiplayer_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        UpdateIsSelected();
    }

    private void UpdateIsSelected()
    {
        if (GameMultiplayerManager.Instance.GetPlayerData().classId == classId)
        {
            selectedGameObject.SetActive(true);
        }
        else
        {
            selectedGameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        GameMultiplayerManager.Instance.OnPlayerDataNetworkListChanged -= GameMultiplayer_OnPlayerDataNetworkListChanged;
    }
}