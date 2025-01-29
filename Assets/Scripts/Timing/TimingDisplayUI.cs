using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class TimingDisplayUI : MonoBehaviour
{
    [Header("UI References")]
    public Canvas timingCanvas;
    public Text timingTextPrefab;
    public Transform contentParent;
    public Text totalTimeText;

    private TimeManager timeManager;
    private List<Text> instantiatedTexts = new List<Text>();

    private void Start()
    {
        timeManager = FindObjectOfType<TimeManager>();
        timingCanvas.enabled = true;
    }

    public void ToggleDisplay()
    {
        timingCanvas.enabled = !timingCanvas.enabled;
        if (timingCanvas.enabled) RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        // Clear existing entries
        foreach (var text in instantiatedTexts)
        {
            if (text != null) Destroy(text.gameObject);
        }
        instantiatedTexts.Clear();

        // Add new entries
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

        totalTimeText.text = $"Total Time: {totalTime:F1}s";
    }
}