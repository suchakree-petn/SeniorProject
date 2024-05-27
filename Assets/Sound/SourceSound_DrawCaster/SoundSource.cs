using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundSource : MonoBehaviour
{
    [SerializeField] private AudioSource sfx_button;
    [SerializeField] private AudioClip mainMenu, normal, boss;
    [SerializeField] private AudioSource mainMenu_source, normal_source;
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
        if (scenename == "MainMenu" || scenename == "InLobby")
        {
            mainMenu_source.Play();
            normal_source.Pause();
        }
        else if (scenename == "InGame")
        {
            mainMenu_source.Pause();
            normal_source.Play();
        }
    }
    public string GetSceneName()
    {
        Scene sceneName = SceneManager.GetActiveScene();
        return sceneName.name;
    }
    public void PlaySfxButton()
    {
        sfx_button.Play();
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
