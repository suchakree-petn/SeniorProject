using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUIManager : NetworkSingleton<AbilityUIManager>
{
    [SerializeField] private Image abilityIcon_E;
    [SerializeField] private Image abilityIcon_Q;
    [SerializeField] private Image abilityCD_E;
    [SerializeField] private Image abilityCD_Q;

    [SerializeField] private bool isCD_Q;
    [SerializeField] private bool isCD_E;

    [SerializeField] private float CD_Q;
    [SerializeField] private float maxCD_Q;
    [SerializeField] private float CD_E;
    [SerializeField] private float maxCD_E;

    public Action<Sprite> OnSetAbilityIcon_E;
    public Action<Sprite> OnSetAbilityIcon_Q;
    public Action<float> OnUseAbility_E;
    public Action<float> OnUseAbility_Q;
    protected override void InitAfterAwake()
    {
    }

    private void Update()
    {
        if (isCD_E)
        {
            abilityCD_E.fillAmount = CD_E / maxCD_E;
            CD_E -= Time.deltaTime;

            if (CD_E <= 0)
            {
                isCD_E = false;
            }
        }

        if (isCD_Q)
        {
            abilityCD_Q.fillAmount = CD_Q / maxCD_Q;
            CD_Q -= Time.deltaTime;

            if (CD_Q <= 0)
            {
                isCD_Q = false;
            }
        }
    }


    private void SetAbilityIcon_E(Sprite iconSprite)
    {
        abilityIcon_E.sprite = iconSprite;
    }
    private void SetAbilityIcon_Q(Sprite iconSprite)
    {
        abilityIcon_Q.sprite = iconSprite;
    }
    private void StartCDAbility_E(float duration)
    {
        abilityCD_E.fillAmount = 1;
        CD_E = duration;
        maxCD_E = duration;
        isCD_E = true;
    }

    private void StartCDAbility_Q(float duration)
    {
        abilityCD_Q.fillAmount = 1;
        CD_Q = duration;
        maxCD_Q = duration;
        isCD_Q = true;
    }

    private void OnEnable()
    {
        OnSetAbilityIcon_E += SetAbilityIcon_E;
        OnSetAbilityIcon_Q += SetAbilityIcon_Q;

        OnUseAbility_E += StartCDAbility_E;
        OnUseAbility_Q += StartCDAbility_Q;
    }

    private void OnDisable()
    {
        OnSetAbilityIcon_E -= SetAbilityIcon_E;
        OnSetAbilityIcon_Q -= SetAbilityIcon_Q;

        OnUseAbility_E -= StartCDAbility_E;
        OnUseAbility_Q -= StartCDAbility_Q;
    }

}
