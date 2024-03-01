using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class UIHPBar : MonoBehaviour
{
    [SerializeField] private Slider fillHPBar;
    [SerializeField] private TextMeshProUGUI hpValueText;
    public void SetHP()
    {
        // For test
        if (!PlayerManager.Instance.PlayerGameObjects.TryGetValue(0, out GameObject playerGO))
        {
            return;
        }
        if(playerGO==null) return;
        PlayerCharacterData playerCharData = playerGO.GetComponent<PlayerController>().PlayerCharacterData;

        int maxHp = (int)playerCharData.GetMaxHp();
        int currentHp = (int)playerGO.GetComponent<PlayerController>().GetCurrentHp();
        fillHPBar.maxValue = maxHp;
        fillHPBar.value = currentHp;
        hpValueText.SetText(currentHp + " / " + maxHp);
    }

    private void Update()
    {
        SetHP();
    }
    // private void OnEnable() {
    //     GameController.OnBeforeStart += SetHPDefault;
    //     GameController.WhileInGame += ShowHPValue;
    // }
    // private void OnDisable() {
    //     GameController.OnBeforeStart -= SetHPDefault;
    //     GameController.WhileInGame -= ShowHPValue;
    // }
}
