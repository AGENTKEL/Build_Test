using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    public void PlayRegular()
    {
        GameManager.instance.playMode = true;
        GameManager.instance.shouldLoad = false;
        SceneManager.LoadScene("Terrain");
    }
    
    public void PlayLoad()
    {
        GameManager.instance.playMode = true;
        GameManager.instance.shouldLoad = true;
        SceneManager.LoadScene("Terrain");
    }
    
    public void EditorRegular()
    {
        GameManager.instance.playMode = false;
        GameManager.instance.shouldLoad = false;
        SceneManager.LoadScene("Terrain");
    }
    
    public void EditorLoad()
    {
        GameManager.instance.playMode = false;
        GameManager.instance.shouldLoad = true;
        SceneManager.LoadScene("Terrain");
    }
}
