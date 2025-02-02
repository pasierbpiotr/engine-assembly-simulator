using UnityEngine;
using UnityEngine.SceneManagement;

// Główna klasa zarządzająca menu VR w aplikacji montażowej
public class MainMenuVR : MonoBehaviour
{
    // Rozpoczyna nową sesję montażową poprzez załadowanie sceny budowy
    public void StartGame()
    {
        SceneManager.LoadScene("BuildingScene");
    }
    
    // Bezpiecznie zamyka aplikację z uwzględnieniem trybu edycyjnego Unity
    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
