using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

// Klasa odpowiedzialna za wyświetlanie statystyk czasu montażu w interfejsie użytkownika
public class TimingDisplayUI : MonoBehaviour
{
    [Header("UI References")]
    public Canvas timingCanvas; // Główny canvas wyświetlacza
    public Text timingTextPrefab; // Prefab tekstu dla pojedynczego wpisu
    public Transform contentParent; // Kontener dla elementów listy
    public Text totalTimeText; // Tekst z sumarycznym czasem
    private TimeManager timeManager; // Referencja do menedżera czasu
    private List<Text> instantiatedTexts = new List<Text>(); // Lista utworzonych tekstów

    private void Start()
    {
        timeManager = FindObjectOfType<TimeManager>();
        timingCanvas.enabled = true;
    }

    // Przełączanie widoczności panelu czasów
    public void ToggleDisplay()
    {
        timingCanvas.enabled = !timingCanvas.enabled;
        if (timingCanvas.enabled) RefreshDisplay();
    }

    // Aktualizacja całego widoku statystyk
    public void RefreshDisplay()
    {
        foreach (var text in instantiatedTexts)
        {
            if (text != null) Destroy(text.gameObject);
        }
        instantiatedTexts.Clear();

        if (timeManager == null)
        {
            Debug.LogError("TimeManager reference is missing!");
            return;
        }

        float totalTime = 0f;
        var timings = timeManager.GetAllTimings();
        
        foreach (var entry in timings)
        {
            var newText = Instantiate(timingTextPrefab, contentParent);
            newText.text = $"{entry.GroupId} - {entry.PartId}: {entry.TimeTaken:F1}s";
            instantiatedTexts.Add(newText);
            totalTime += entry.TimeTaken;
        }

        totalTimeText.text = $"Całkowity czas: {totalTime:F1}s";
    }
}