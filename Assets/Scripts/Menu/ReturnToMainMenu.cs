using UnityEngine;
using UnityEngine.SceneManagement;

// Klasa odpowiedzialna za powrót do menu głównego z dowolnej sceny
public class ReturnToMainMenu : MonoBehaviour
{
    // Ładuje scenę głównego menu
    public void ReturnToMain()
    {
        SceneManager.LoadScene("IntroScene");
    }
}
