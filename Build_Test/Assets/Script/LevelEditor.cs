using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditor : MonoBehaviour
{
    [SerializeField] private VehicleEdit _vehicleEdit;
    
    public enum EditorMode
    {
        CameraMovement,
        VehiclePlacing,
        VehicleEdit,
        PlayMode
    }

    private void Start()
    {
        SwitchMode(EditorMode.CameraMovement);
    }

    public EditorMode currentMode = EditorMode.CameraMovement;

    private Vector2 touchStart;
    public float swipeSensitivity = 0.5f;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject vehiclePlacingUI;
    [SerializeField] private GameObject vehicleEditUI;

    void Update()
    {
        // Check for mode-specific behavior
        switch (currentMode)
        {
            case EditorMode.CameraMovement:
                HandleCameraMovement();
                HandleVehicleSelection();
                break;

            case EditorMode.VehiclePlacing:
                HandleVehicleSelection();
                HandleVehicleDragging();
                break;
            
            case EditorMode.VehicleEdit:
                HandleVehicleSelection();
                HandleVehicleDragging();
                break;
            
            case EditorMode.PlayMode:

                break;
        }
    }

    private void HandleCameraMovement()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStart = touch.position;
                    break;

                case TouchPhase.Moved:
                    Vector2 swipeDelta = touchStart - touch.position;
                    Vector3 move = new Vector3(swipeDelta.x, 0, swipeDelta.y) * swipeSensitivity * Time.deltaTime;
                    transform.Translate(move, Space.World);
                    touchStart = touch.position;
                    break;
            }
        }
    }
    
    private void HandleVehicleSelection()
    {
        _vehicleEdit.HandleVehicleSelection();
    }
    
    private void HandleVehicleDragging()
    {
        _vehicleEdit.HandleVehicleDragging();
    }
    

    public void SwitchMode(EditorMode mode)
    {
        // Handle deactivating current mode
        switch (currentMode)
        {
            case EditorMode.CameraMovement:
                _vehicleEdit.DeselectObject();
                vehiclePlacingUI.SetActive(false);
                vehicleEditUI.SetActive(false);
                break;

            case EditorMode.VehiclePlacing:
                vehiclePlacingUI.SetActive(true);
                vehicleEditUI.SetActive(false);
                break;
            
            case EditorMode.VehicleEdit:
                vehicleEditUI.SetActive(true);
                vehiclePlacingUI.SetActive(false);
                break;
            
            case EditorMode.PlayMode:
                vehicleEditUI.SetActive(false);
                vehiclePlacingUI.SetActive(false);
                break;
            
        }
        
        // Activate new mode
        currentMode = mode;

        switch (currentMode)
        {
            case EditorMode.CameraMovement:
                _vehicleEdit.DeselectObject();
                vehiclePlacingUI.SetActive(false);
                vehicleEditUI.SetActive(false);
                break;

            case EditorMode.VehiclePlacing:
                vehiclePlacingUI.SetActive(true);
                vehicleEditUI.SetActive(false);
                break;
            
            case EditorMode.VehicleEdit:
                vehicleEditUI.SetActive(true);
                vehiclePlacingUI.SetActive(false);
                break;
            
            case EditorMode.PlayMode:
                vehicleEditUI.SetActive(false);
                vehiclePlacingUI.SetActive(false);
                break;
            
        }
        
    }
    
    public void SetCameraMovementMode()
    {
        SwitchMode(EditorMode.CameraMovement);
    }

    public void SetVehiclePlacingMode()
    {
        SwitchMode(EditorMode.VehiclePlacing);
    }
    
    public void SetVehicleEditMode()
    {
        SwitchMode(EditorMode.VehicleEdit);
    }
    
    public void SetVehiclePlayMode()
    {
        SwitchMode(EditorMode.PlayMode);
    }
}
