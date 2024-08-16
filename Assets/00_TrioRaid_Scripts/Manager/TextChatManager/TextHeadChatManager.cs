using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextHeadChatManager : Singleton<TextHeadChatManager>
{
    public string playerName;

    protected override void InitAfterAwake()
    {
    }

    private void Update() {
        if (string.IsNullOrEmpty(playerName));
    }

}
