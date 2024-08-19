using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PingObjectUI : MonoBehaviour
{
    [SerializeField] SpriteRenderer pingImage;
    [SerializeField] SpriteRenderer bgImage;
    [SerializeField] ulong id;
    [SerializeField] int _classId;
    [SerializeField] TextMeshPro _classNameText;
    [SerializeField] TextMeshPro _detailText;
    [SerializeField] float delayPingDestroy;

    [SerializeField] List<Color> classColor;
    [SerializeField] List<string> classList;
    float timer = 0;
    private void Update() {
        timer += Time.deltaTime;
        if (timer > delayPingDestroy)
        {
            PingMenuManager.Instance.DeletePingInScene();
        }
    }
    public void SetPingUI(Sprite sprite, ulong ownPing, int classId, string detailText)
    {
        id = ownPing;
        pingImage.sprite = sprite;
        _detailText.text = detailText;
        _classId = classId;
        SetClassById(classId);
    }
    void SetClassById(int classId)
    {
        Color color = Color.white;
        string className = "";
        for (int i = 0; i < 3; i++)
        {
            if (classId == i)
            {
                color = classColor[i];
                className = classList[i];
            }
        }
        _classNameText.text = className;
        _classNameText.color = color;
        bgImage.color = color;
    }
    public ulong GetIDPing()
    {
        return this.id;
    }
    public int GetClassID()
    {
        return this._classId;
    }
}
