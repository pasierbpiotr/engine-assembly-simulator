using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMainMenu : MonoBehaviour
{
    public void ReturnToMain()
    {
        SceneManager.LoadScene("IntroScene");
    }
}
