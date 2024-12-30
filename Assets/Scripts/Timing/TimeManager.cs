using UnityEngine;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Zarządzanie czasem dla grup i części w procesie montażu.
/// </summary>
public class TimeManager : MonoBehaviour
{
    // Słownik przechowujący czasy rozpoczęcia dla grup.
    private Dictionary<string, float> groupStartTime = new Dictionary<string, float>();

    // Ścieżka do pliku CSV.
    private string csvFilePath = "TimeData/AssemblyTimes.csv";

    /// <summary>
    /// Metoda uruchamiana na początku działania skryptu.
    /// </summary>
    void Start()
    {
        InitializeCSVFile();
    }

    /// <summary>
    /// Inicjalizacja pliku CSV, tworzenie pliku z nagłówkiem, jeśli nie istnieje.
    /// </summary>
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

    /// <summary>
    /// Rozpoczyna mierzenie czasu dla danej grupy.
    /// </summary>
    /// <param name="groupId">Identyfikator grupy.</param>
    public void StartTiming(string groupId)
    {
        groupStartTime[groupId] = Time.time;
        Debug.Log($"Rozpoczęto mierzenie czasu dla grupy {groupId} o {groupStartTime[groupId]} sekund.");
    }

    /// <summary>
    /// Kończy mierzenie czasu dla danej grupy i części, zapisując wynik do pliku CSV.
    /// </summary>
    /// <param name="groupId">Identyfikator grupy.</param>
    /// <param name="partId">Identyfikator części.</param>
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

    /// <summary>
    /// Zapisuje zmierzony czas do pliku CSV.
    /// </summary>
    /// <param name="groupId">Identyfikator grupy.</param>
    /// <param name="partId">Identyfikator części.</param>
    /// <param name="timeTaken">Czas trwania w sekundach.</param>
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
