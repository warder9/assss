using UnityEngine;
using Unity.Cinemachine; // Ensure you have this for CinemachineFreeLook

public class CarSpawner : MonoBehaviour
{
    public Transform spawnPoint; // Where to teleport the selected car

    void Start()
    {
        string selectedCarName = PlayerPrefs.GetString("SelectedCar", "Car1");

        GameObject[] allCars = GameObject.FindGameObjectsWithTag("Car");
        GameObject selectedCarObject = null;

        foreach (GameObject car in allCars)
        {
            if (car.name == selectedCarName || car.name == selectedCarName + "(Clone)")
            {
                selectedCarObject = car;
            }
            else
            {
                Destroy(car); // Remove unselected cars
            }
        }

        if (selectedCarObject != null)
        {
            // Move selected car to spawn point
            selectedCarObject.transform.position = spawnPoint.position;
            selectedCarObject.transform.rotation = spawnPoint.rotation;

            // Enable its internal FreeLook camera
            CinemachineFreeLook[] carCameras = selectedCarObject.GetComponentsInChildren<CinemachineFreeLook>(true);
            foreach (var cam in carCameras)
            {
                cam.gameObject.SetActive(true); // Enable camera object
            }

            Debug.Log("Selected car '" + selectedCarName + "' moved and camera activated.");
        }
        else
        {
            Debug.LogError("Selected car not found in scene: " + selectedCarName);
        }
    }
}
