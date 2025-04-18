using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    void Start()
    {
        string selectedCar = PlayerPrefs.GetString("SelectedCar", "DefaultCar"); // Get selected car
        string selectedLevel = PlayerPrefs.GetString("SelectedLevel", "DefaultLevel"); // Get selected level

        Debug.Log("Selected Car: " + selectedCar);
        Debug.Log("Selected Level: " + selectedLevel);

        // Load the level dynamically here based on the selectedLevel
        LoadLevel(selectedLevel);
        SceneManager.LoadScene(selectedLevel);
    }

    void LoadLevel(string levelName)
    {
        // Based on levelName, load the corresponding environment, objects, etc.
        // You can use this string to enable level-specific content or spawn objects
        Debug.Log("Loading level: " + levelName);
        // Example: Spawn level environment or handle level-specific logic
    }
}
