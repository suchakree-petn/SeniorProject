using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader 
{
    public enum Scene{
        LobbyScene,
        GameScene,
        LoadingScene,
        CharacterSelectScene
    }
    static string TransScene(string scene){
        string returnScene = "LoadingScene";
        if(scene == "LobbyScene"){
            returnScene = "MainMenu";
        }else if(scene == "GameScene"){
            returnScene = "InGame_Map_1_PressurePad";
        }else if(scene == "CharacterSelectScene"){
            returnScene = "InLobby";
        }
        return returnScene;
    }
    private static Scene targetScene;

    public static void Load(Scene _targetScene){
        Loader.targetScene = _targetScene;

        SceneManager.LoadScene(TransScene(Scene.LoadingScene.ToString()));
    }
    public static void LoadNetwork(Scene targetScene){
        NetworkManager.Singleton.SceneManager.LoadScene(TransScene(targetScene.ToString()),LoadSceneMode.Single);
    }
    public static void LoaderCallback(){
        SceneManager.LoadScene(TransScene(targetScene.ToString()));
    }
}
