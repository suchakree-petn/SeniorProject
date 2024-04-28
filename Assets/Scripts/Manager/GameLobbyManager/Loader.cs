using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader 
{
    public enum Scene{
        Thanva_MainMenu_UserDataPersistence,
        Thanva_Map_Tester,
        LoadingScene,
        Thanva_InLobby
    }

    private static Scene targetScene;

    public static void Load(Scene targetScene){
        Loader.targetScene = targetScene;

        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }
    public static void LoadNetwork(Scene targetScene){
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(),LoadSceneMode.Single);
    }
    public static void Loadercallback(){
        SceneManager.LoadScene(targetScene.ToString());
    }
}
