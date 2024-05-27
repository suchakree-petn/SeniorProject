using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundSource : MonoBehaviour
{
    [SerializeField] private AudioSource sfx_back, sfx_go, sfx_cast, sfx_castFail, sfx_c1, sfx_c2, sfx_c3;
    [SerializeField] private AudioClip mainMenu, normal, boss;
    [SerializeField] private AudioSource mainMenu_source, normal_source, boss_source;
    [SerializeField] private GameObject loopPlayer;
    [SerializeField] private LoadSceneMode loadSceneMode;

    public static SoundSource Instance;
    private void Awake()
    {
        GameObject[] soundSourceObj = GameObject.FindGameObjectsWithTag("Sound Soucre");
        if (soundSourceObj.Length > 1)
        {
            Destroy(gameObject);
        }
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public void ChangeSoundLoop(Scene scene, LoadSceneMode loadSceneMode)
    {
        string scenename = scene.name;
        // AudioSource sourcePlayer = loopPlayer.GetComponent<AudioSource>();
        if (scenename == "MainMenu" || scenename == "SpellSelect" || scenename == "resultScene" || scenename == "DimensionSelect")
        {
            mainMenu_source.Play();
            normal_source.Pause();
            boss_source.Pause();
        }
        else if (scenename == "Tutorial 1" || scenename == "Dimension 1_1" || scenename == "Dimension 2_1" || scenename == "Dimension 3_1")
        {
            mainMenu_source.Pause();
            normal_source.Play();
            boss_source.Pause();
        }
        else if (scenename == "Dimension 1_4" || scenename == "Dimension 2_4" || scenename == "Dimension 3_4")
        {
            mainMenu_source.Pause();
            normal_source.Pause();
            boss_source.Play();
        }
    }
    public string GetSceneName()
    {
        Scene sceneName = SceneManager.GetActiveScene();
        return sceneName.name;
    }
    public void PlaySfxGo()
    {
        sfx_go.Play();
    }
    public void PlaySfxBack()
    {
        sfx_back.Play();
    }
    public void PlaySfxCastSpell()
    {
        sfx_cast.Play();
    }
    public void PlaySfxCastSpellFail()
    {
        sfx_castFail.Play();
    }
    public void PlaySfxCast1()
    {
        sfx_c1.Play();
    }
    public void PlaySfxCast2()
    {
        sfx_c2.Play();
    }
    public void PlaySfxCast3()
    {
        sfx_c3.Play();
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += ChangeSoundLoop;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= ChangeSoundLoop;
    }
}
