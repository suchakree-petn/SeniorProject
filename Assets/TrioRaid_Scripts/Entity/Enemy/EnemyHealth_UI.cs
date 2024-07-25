using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth_UI : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Image hpFill;

    public void SetHpBar(float hpPercentage)
    {
        hpFill.fillAmount = hpPercentage;
    }
}
