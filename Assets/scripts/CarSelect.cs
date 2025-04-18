using UnityEngine;
using UnityEngine.SceneManagement;

public class CarSelect : MonoBehaviour
{
    public void SelectCar(string carName)
    {
        PlayerPrefs.SetString("SelectedCar", carName);
        SceneManager.LoadScene("LevelSelectScene");
    }
}