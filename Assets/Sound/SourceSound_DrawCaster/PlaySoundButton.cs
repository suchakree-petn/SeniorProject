using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundButton : MonoBehaviour
{
    public void PlaySFXGo(){
        SoundSource.Instance.PlaySfxGo();
    }
    public void PlaySfxBack(){
        SoundSource.Instance.PlaySfxBack();
    }
}
