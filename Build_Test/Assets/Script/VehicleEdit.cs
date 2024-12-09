using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleEdit : MonoBehaviour
{

    public VehicleData[] vehicles;
    public Transform spawnPosition;
    public float spawnOffset = 5f; 

    [SerializeField] private GameObject currentVehicle;
    [SerializeField] private GameObject previewVehicle;
    [SerializeField] private List<PlacedVehicleData> placedVehicles = new List<PlacedVehicleData>();
    private Vector3 nextSpawnPosition;             
    
    public Camera editorCamera;
    [SerializeField] private LevelEditor _levelEditor;
    
    // Swipe detection
    private Vector3 lastTouchPosition;
    private bool isDragging = false;
    
    private string saveFilePath;

    private void Start()
    {
        nextSpawnPosition = spawnPosition.position;
        saveFilePath = Application.persistentDataPath + "/vehicles.json";
    }

    public void PreviewVehicle(int index)
    {
        if (index < 0 || index >= vehicles.Length)
        {
            Debug.LogError("Invalid vehicle index!");
            return;
        }

        // Destroy existing preview vehicle
        if (previewVehicle != null)
        {
            Destroy(previewVehicle);
        }

        // Spawn a new preview vehicle
        previewVehicle = Instantiate(vehicles[index].prefab, nextSpawnPosition, spawnPosition.rotation);

        MoveCamera();

        isDragging = true;
        lastTouchPosition = Input.mousePosition;

        HighlightSelectedVehicle(previewVehicle);
    }
    
    public void HandleVehicleSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = editorCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject hitObject = hit.collider.gameObject;

                // Find the corresponding PlacedVehicleData in the list
                PlacedVehicleData selectedData = placedVehicles.Find(v => v.vehicleObject == hitObject);

                if (selectedData != null)
                {
                    if (previewVehicle != null)
                        Destroy(previewVehicle);
                    DeselectObject();
                    currentVehicle = selectedData.vehicleObject;
                    Debug.Log($"Selected vehicle: {currentVehicle.name}");

                    // Highlight the selected vehicle (optional)
                    HighlightSelectedVehicle(currentVehicle);

                    isDragging = true;
                    lastTouchPosition = Input.mousePosition;

                    _levelEditor.SetVehicleEditMode();
                    MoveCamera();
                }
                else
                {
                    Debug.Log("No placed vehicle was clicked!");
                }
            }
        }
    }

    private void HighlightSelectedVehicle(GameObject vehicle)
    {
        //Change material color to indicate selection
        Renderer renderer = vehicle.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.yellow;
        }
    }

    private void ResetHighlight(GameObject vehicle)
    {
        // Reset material color
        Renderer renderer = vehicle.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.white;
        }
    }
    
    public void HandleVehicleDragging()
    {
        if (currentVehicle == null || !isDragging) 
            return;

        // Check if the left mouse button is being held down
        if (Input.GetMouseButton(0))
        {
            Vector3 currentTouchPosition = Input.mousePosition;
            Vector3 delta = currentTouchPosition - lastTouchPosition;

            // Move the object only if there's a significant drag
            if (delta.magnitude > 5f) // Adjust this threshold as needed
            {
                Ray ray = editorCamera.ScreenPointToRay(currentTouchPosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Vector3 newPosition = hit.point;
                    newPosition.y = currentVehicle.transform.position.y; // Maintain the current Y position

                    currentVehicle.transform.position = newPosition;
                }

                lastTouchPosition = currentTouchPosition;
            }
        }
        else
        {
            // Stop dragging when the mouse button is released
            isDragging = false;
        }
    }


    public void AddVehicle()
    {
        if (previewVehicle == null)
        {
            Debug.LogError("No vehicle is currently previewed!");
            return;
        }

        ResetHighlight(previewVehicle);

        // Add the preview vehicle to placedVehicles
        PlacedVehicleData vehicleData = new PlacedVehicleData(
            previewVehicle,
            previewVehicle.GetComponent<VehicleData>().name,
            previewVehicle.transform.position,
            previewVehicle.transform.rotation
        );
        placedVehicles.Add(vehicleData);

        Debug.Log($"Vehicle {previewVehicle.name} added to the placedVehicles list at position {previewVehicle.transform.position}, rotation {previewVehicle.transform.rotation.eulerAngles}.");

        // Assign the preview vehicle to currentVehicle and clear the preview
        currentVehicle = previewVehicle;
        previewVehicle = null;

        // Update the next spawn position
        nextSpawnPosition += new Vector3(spawnOffset, 0, 0);
    }

    public void DeselectObject()
    {
        if (currentVehicle != null)
        {
            ResetHighlight(currentVehicle);
            currentVehicle = null;
        }
    }

    public void DeleteCurrentVehicle()
    {
        if (previewVehicle != null)
        {
            Destroy(previewVehicle);
            return;
        }
        
        if (currentVehicle == null)
        {
            Debug.LogError("No vehicle selected or previewed to delete!");
            return;
        }

        // Find and remove the vehicle from the list
        PlacedVehicleData vehicleToDelete = placedVehicles.Find(v => v.vehicleObject == currentVehicle);

        if (vehicleToDelete != null)
        {
            placedVehicles.Remove(vehicleToDelete);
            Debug.Log($"Removed {vehicleToDelete.vehicleObject.name} from placedVehicles list.");

            // Destroy the vehicle GameObject
            Destroy(vehicleToDelete.vehicleObject);
            currentVehicle = null;

            Debug.Log("Vehicle deleted successfully.");
        }
        else
        {
            Debug.LogWarning("Could not find the selected vehicle in the list.");
        }
    }
    
    public void RotateCurrentVehicle()
    {
        if (currentVehicle != null)
        {
            // Rotate the vehicle by 10 degrees around the Y-axis
            currentVehicle.transform.Rotate(0, 10f, 0);
        }
        
        if (previewVehicle != null)
        {
            // Rotate the vehicle by 10 degrees around the Y-axis
            previewVehicle.transform.Rotate(0, 10f, 0);
        }
        else
        {
            Debug.LogError("No vehicle is currently previewed to rotate!");
        }
    }
    
    public void ApplyChanges()
    {
        if (currentVehicle == null)
        {
            Debug.LogError("No vehicle is selected to apply changes!");
            return;
        }

        // Find the corresponding entry in the placedVehicles list
        PlacedVehicleData vehicleData = placedVehicles.Find(v => v.vehicleObject == currentVehicle);

        if (vehicleData != null)
        {

            vehicleData.position = currentVehicle.transform.position;
            vehicleData.rotation = currentVehicle.transform.rotation;

            Debug.Log($"Updated {currentVehicle.name} in placedVehicles list: Position {vehicleData.position}, Rotation {vehicleData.rotation.eulerAngles}");
        }
        else
        {
            Debug.LogError("Failed to update vehicle in the list. Vehicle not found!");
        }
        
        DeselectObject();
        _levelEditor.SetCameraMovementMode();
    }

    public void MoveCamera()
    {
        if (currentVehicle != null)
        {
            Vector3 cameraNewPosition = editorCamera.transform.position;
            cameraNewPosition.x = currentVehicle.transform.position.x;
            cameraNewPosition.z = currentVehicle.transform.position.z - 30f;
            editorCamera.transform.position = cameraNewPosition;
        }
        
        if (previewVehicle != null)
        {
            Vector3 cameraNewPosition = editorCamera.transform.position;
            cameraNewPosition.x = previewVehicle.transform.position.x;
            cameraNewPosition.z = previewVehicle.transform.position.z - 30f;
            editorCamera.transform.position = cameraNewPosition;
        }
    }
    
    
    public void SaveVehiclesToJson(string filePath)
    {
        List<VehicleSaveData> saveDataList = new List<VehicleSaveData>();

        foreach (var vehicleData in placedVehicles)
        {
            VehicleSaveData saveData = new VehicleSaveData
            {
                name = vehicleData.vehicleObject.GetComponent<VehicleData>().name,
                position = vehicleData.position,
                rotation = vehicleData.rotation
            };
            saveDataList.Add(saveData);
        }

        string json = JsonUtility.ToJson(new VehicleSaveWrapper { vehicles = saveDataList }, true);
        System.IO.File.WriteAllText(filePath, json);

        Debug.Log($"Saved {saveDataList.Count} vehicles to {filePath}");
    }
    
    
    
    public void LoadVehiclesFromJson(string filePath)
    {
        if (!System.IO.File.Exists(filePath))
        {
            Debug.LogError($"File not found: {filePath}");
            return;
        }

        string json = System.IO.File.ReadAllText(filePath);
        VehicleSaveWrapper saveWrapper = JsonUtility.FromJson<VehicleSaveWrapper>(json);

        if (saveWrapper?.vehicles != null)
        {
            // Clear existing placed vehicles
            Debug.Log("Clearing existing vehicles...");
            foreach (var vehicleData in placedVehicles)
            {
                Debug.Log($"Destroying vehicle: {vehicleData.vehicleObject.name}");
                Destroy(vehicleData.vehicleObject);
            }
            placedVehicles.Clear();

            // Instantiate vehicles from the saved data
            foreach (var saveData in saveWrapper.vehicles)
            {
                Debug.Log($"Loading vehicle: {saveData.name}");

                // Load the prefab dynamically from the Resources folder
                GameObject vehiclePrefab = Resources.Load<GameObject>("Prefabs/" + saveData.name);  // Path inside Resources

                if (vehiclePrefab != null)
                {
                    // Instantiate the vehicle
                    GameObject newVehicle = Instantiate(vehiclePrefab, saveData.position, saveData.rotation);
                    Debug.Log($"Instantiating vehicle: {vehiclePrefab.name} at {saveData.position}, rotation: {saveData.rotation}");

                    // Add to the placedVehicles list with the name
                    PlacedVehicleData newVehicleData = new PlacedVehicleData(newVehicle, saveData.name, saveData.position, saveData.rotation);
                    placedVehicles.Add(newVehicleData);
                }
                else
                {
                    Debug.LogError($"Prefab not found for vehicle: {saveData.name}");
                }
            }

            Debug.Log($"Loaded {saveWrapper.vehicles.Count} vehicles from {filePath}");
        }
        else
        {
            Debug.LogError("Failed to parse vehicle save data!");
        }
    }
    
    public void SaveVehicles()
    {
        SaveVehiclesToJson(saveFilePath);
    }

    public void LoadVehicles()
    {
        LoadVehiclesFromJson(saveFilePath);
    }
}

    
[System.Serializable]
public class PlacedVehicleData
{
    public GameObject vehicleObject;
    public string name;
    public Vector3 position;
    public Quaternion rotation;

    public PlacedVehicleData(GameObject vehicle, string vehicleName, Vector3 pos, Quaternion rot)
    {
        vehicleObject = vehicle;
        name = vehicleName;
        position = pos;
        rotation = rot;
    }
}

    
[System.Serializable]
public class VehicleSaveData
{
    public string name;
    public Vector3 position;
    public Quaternion rotation;
}
    
[System.Serializable]
public class VehicleSaveWrapper
{
    public List<VehicleSaveData> vehicles;
}