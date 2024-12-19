using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class TimeManager : MonoBehaviour
{
    private Dictionary<string, float> groupStartTime = new Dictionary<string, float>();
    private string csvFilePath = "TimeData/AssemblyTimes.csv";

    void Start()
    {
        InitializeCSVFile();
    }

    private void InitializeCSVFile()
    {
        if (!File.Exists(csvFilePath))
        {
            using (StreamWriter sw = File.CreateText(csvFilePath))
            {
                sw.WriteLine("GroupId;PartId;TimeTaken");
            }
            Debug.Log("CSV file created and header written.");
        }
    }

    public void StartTiming(string groupId)
    {
        groupStartTime[groupId] = Time.time;
        Debug.Log($"Started timing for group {groupId} at {groupStartTime[groupId]} seconds.");
    }

    public void StopTiming(string groupId, string partId)
    {
        if (groupStartTime.ContainsKey(groupId))
        {
            float timeTaken = Time.time - groupStartTime[groupId];
            if (timeTaken > 0)
            {
                SaveTimeToCSV(groupId, partId, timeTaken);
            }
            groupStartTime[groupId] = Time.time;
            Debug.Log($"Stopped timing for group {groupId}, part {partId}. Time taken: {timeTaken} seconds.");
        }
        else
        {
            Debug.LogWarning($"Attempted to stop timing for group {groupId}, but no start time was found.");
        }
    }

    private void SaveTimeToCSV(string groupId, string partId, float timeTaken)
    {
        using (StreamWriter sw = File.AppendText(csvFilePath))
        {
            sw.WriteLine($"{groupId};{partId};{timeTaken}");
        }
        Debug.Log($"Saved time for group {groupId}, part {partId} to CSV: {timeTaken} seconds.");
    }
}