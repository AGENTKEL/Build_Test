using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTypeHandler : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject editorUI;
    [SerializeField] private VehicleEdit _vehicleEdit;
    private void Start()
    {
        if (GameManager.instance.playMode)
        {
            _player.SetActive(true);
            playerUI.SetActive(true);
            editorUI.SetActive(false);
            if (GameManager.instance.shouldLoad)
            {
                _vehicleEdit.LoadVehicles();
            }
        }

        else
        {
            _player.SetActive(false);
            playerUI.SetActive(false);
            editorUI.SetActive(true);
        }
        
        if (GameManager.instance.shouldLoad)
        {
            _vehicleEdit.LoadVehicles();
        }
    }

    public void SpawnPlayer()
    {
        _player.SetActive(true);
        playerUI.SetActive(true);
        editorUI.SetActive(false);
    }
    
    public void DespawnPlayer()
    {
        _player.SetActive(false);
        playerUI.SetActive(false);
        editorUI.SetActive(true);
    }
    
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    
}
