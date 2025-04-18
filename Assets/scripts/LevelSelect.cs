using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    public void SelectLevel(string levelName)
    {
        // Optional: destroy AudioManager if it exists
        GameObject music = GameObject.Find("AudioManager"); // Or whatever your music object is named
        if (music != null)
        {
            Destroy(music);
        }

        PlayerPrefs.SetString("SelectedLevel", levelName);
        SceneManager.LoadScene("GameScene"); // or use levelName if you're loading by name
    }
}
