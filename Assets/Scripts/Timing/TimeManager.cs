using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

/// <summary>
/// Zarządzanie czasem dla grup i części w procesie montażu.
/// </summary>
public class TimeManager : MonoBehaviour
{
    // Słownik przechowujący czasy rozpoczęcia dla grup.
    private Dictionary<string, float> groupStartTime = new Dictionary<string, float>();

    // Ścieżka do pliku CSV.
    private string csvFilePath = "TimeData/AssemblyTimes.csv";

    private float startTime;

    private void Awake()
    {
        // Register the sceneLoaded event to detect scene loads
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Unregister to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BuildingScene")
        {
            StartTimer();
        }
    }

    private void StartTimer()
    {
        startTime = Time.time;
        Debug.Log("Timer started for building scene!");
    }

    public float GetElapsedTime()
    {
        return Time.time - startTime;
    }

    void Start()
    {
        InitializeCSVFile();
    }

    private void InitializeCSVFile()
    {
        if (!File.Exists(csvFilePath))
        {
            // Tworzenie pliku CSV z nagłówkiem.
            using (StreamWriter sw = File.CreateText(csvFilePath))
            {
                sw.WriteLine("GroupId;PartId;TimeTaken");
            }
            Debug.Log("Plik CSV został utworzony i zapisano nagłówek.");
        }
    }

    public void StartTiming(string groupId)
    {
        groupStartTime[groupId] = Time.time;
        Debug.Log($"Rozpoczęto mierzenie czasu dla grupy {groupId} o {groupStartTime[groupId]} sekund.");
    }

    public void StopTiming(string groupId, string partId)
    {
        if (groupStartTime.ContainsKey(groupId))
        {
            // Obliczanie czasu trwania.
            float timeTaken = Time.time - groupStartTime[groupId];
            if (timeTaken > 0)
            {
                SaveTimeToCSV(groupId, partId, timeTaken);
            }

            // Resetowanie czasu rozpoczęcia dla grupy.
            groupStartTime[groupId] = Time.time;
            Debug.Log($"Zakończono mierzenie czasu dla grupy {groupId}, części {partId}. Czas: {timeTaken} sekund.");
        }
        else
        {
            Debug.LogWarning($"Próba zakończenia mierzenia czasu dla grupy {groupId}, ale brak czasu początkowego.");
        }
    }

    private void SaveTimeToCSV(string groupId, string partId, float timeTaken)
    {
        using (StreamWriter sw = File.AppendText(csvFilePath))
        {
            // Zapis do pliku CSV w formacie GroupId;PartId;TimeTaken.
            sw.WriteLine($"{groupId};{partId};{timeTaken}");
        }
        Debug.Log($"Zapisano czas dla grupy {groupId}, części {partId} do pliku CSV: {timeTaken} sekund.");
    }
}
