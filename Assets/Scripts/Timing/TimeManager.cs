using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

// Główny menedżer śledzenia czasu montażu części w grupach
public class TimeManager : MonoBehaviour
{
    // Słowniki przechowujące czas startu grup i ostatniej części
    private Dictionary<string, float> groupStartTimes = new Dictionary<string, float>();
    private Dictionary<string, float> lastPartTimes = new Dictionary<string, float>();
    // Lista rekordów czasu montażu poszczególnych części
    private List<TimingEntry> timingRecords = new List<TimingEntry>();

    // Rozpoczyna pomiar czasu dla nowej grupy
    public void StartGroupTiming(string groupId)
    {
        groupStartTimes[groupId] = Time.time;
        lastPartTimes[groupId] = Time.time;
        Debug.Log($"Timing started for group: {groupId}");
    }

    // Zatrzymuje pomiar czasu dla pojedynczej części i zapisuje wynik
    public void StopPartTiming(string groupId, string partId)
    {
        if (!groupStartTimes.ContainsKey(groupId))
        {
            Debug.LogError($"Group {groupId} timing not started!");
            return;
        }

        float timeTaken = Time.time - lastPartTimes[groupId];
        timingRecords.Add(new TimingEntry(groupId, partId, timeTaken));
        
        lastPartTimes[groupId] = Time.time;
        
        Debug.Log($"Added timing entry: {groupId} - {partId} | Time: {timeTaken:F1}s");
    }

    // Pobiera wszystkie wyniki posortowane według grup i części
    public List<TimingEntry> GetAllTimings()
    {
        return timingRecords
        .OrderBy(e => {
            var match = Regex.Match(e.GroupId, @"\d+");
            return match.Success ? int.Parse(match.Value) : int.MaxValue;
            })
        .ThenBy(e => e.PartId)
        .ToList();
    }

    // Struktura przechowująca pojedynczy rekord czasu montażu
    public struct TimingEntry
    {
        public string GroupId;
        public string PartId;
        public float TimeTaken;

        public TimingEntry(string groupId, string partId, float timeTaken)
        {
            GroupId = groupId;
            PartId = partId;
            TimeTaken = timeTaken;
        }
    }
}
