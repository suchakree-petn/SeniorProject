using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundButton : MonoBehaviour
{
    public void PlaySFXButton(){
        SoundSource.Instance.PlaySfxButton();
    }
}
