using UnityEngine;
using Unity.Cinemachine;

public class SelectableCar : MonoBehaviour
{
    [Header("Camera Settings")]
    public GameObject cameraObject; // Assign in Inspector
    public string carName = "DefaultCar";

    [Header("Selection Settings")]
    public float doubleClickTime = 0.3f;
    private float lastClickTime;

    private void Start()
    {
        // Ensure proper tagging
        gameObject.tag = "Car";

        // Auto-assign camera if not set
        if (cameraObject == null)
        {
            cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
            Debug.LogWarning($"Camera not assigned to {carName}, attempting auto-assign");
        }
    }

    private void OnMouseDown()
    {
        // Handle double-click prevention
        if (Time.time - lastClickTime < doubleClickTime)
        {
            return;
        }
        lastClickTime = Time.time;

        Debug.Log($"Selected car: {carName}");
        PlayerPrefs.SetString("SelectedCar", carName);
        PlayerPrefs.Save(); // Ensure selection is saved immediately

        SetupCamera();
        CleanupOtherCars();
    }

    private void SetupCamera()
    {
        if (cameraObject == null)
        {
            Debug.LogError("No camera object assigned!");
            return;
        }

        var freeLook = cameraObject.GetComponent<CinemachineFreeLook>();
        if (freeLook != null)
        {
            freeLook.Follow = transform;
            freeLook.LookAt = transform;

            // Reset camera orbits to default position
            freeLook.m_RecenterToTargetHeading.m_enabled = true;
        }
        else
        {
            Debug.LogError("No CinemachineFreeLook component found on camera object!");
        }
    }

    private void CleanupOtherCars()
    {
        GameObject[] allCars = GameObject.FindGameObjectsWithTag("Car");
        foreach (GameObject otherCar in allCars)
        {
            if (otherCar != gameObject)
            {
                // Optional: Add destruction effect here
                Destroy(otherCar);
            }
        }
    }

    // Optional visual feedback
    private void OnMouseEnter()
    {
        // Highlight car when hovered
        GetComponent<Renderer>().material.color = Color.yellow;
    }

    private void OnMouseExit()
    {
        // Return to normal color
        GetComponent<Renderer>().material.color = Color.white;
    }
}