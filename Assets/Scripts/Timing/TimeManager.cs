using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class TimeManager : MonoBehaviour
{
    private Dictionary<string, float> partStartTime = new Dictionary<string, float>(); // Per-part start time
    private Dictionary<string, float> groupStartTime = new Dictionary<string, float>(); // Per-group start time
    private string csvFilePath = "TimeData/PartTimings.csv"; // Path for the CSV file

    private void Start()
    {
        InitializeCSVFile();
    }

    /// <summary>
    /// Create the CSV file if it doesn't already exist.
    /// </summary>
    private void InitializeCSVFile()
    {
        if (!File.Exists(csvFilePath))
        {
            // Create the file with headers
            using (StreamWriter sw = File.CreateText(csvFilePath))
            {
                sw.WriteLine("GroupId;PartId;TimeTaken");
            }
            Debug.Log("CSV file created with headers.");
        }
    }

    /// <summary>
    /// Start timing for a specific part.
    /// </summary>
    /// <param name="partId">Unique identifier for the part (e.g., part name).</param>
    public void StartTiming(string partId)
    {
        partStartTime[partId] = Time.time;
        Debug.Log($"Timing started for part: {partId}");
    }

    /// <summary>
    /// Stop timing for a specific part and save it to the CSV file.
    /// </summary>
    /// <param name="groupId">The group ID of the part.</param>
    /// <param name="partId">The unique identifier for the part.</param>
    public void StopTiming(string groupId, string partId)
    {
        if (partStartTime.ContainsKey(partId))
        {
            // Calculate the elapsed time
            float timeTaken = Time.time - partStartTime[partId];

            // Save to the CSV file
            SaveTimeToCSV(groupId, partId, timeTaken);

            // Remove the part from tracking
            partStartTime.Remove(partId);

            Debug.Log($"Timing stopped for part: {partId}, Time: {timeTaken} seconds.");
        }
        else
        {
            Debug.LogWarning($"No timing found for part: {partId}. Make sure to call StartTiming first.");
        }
    }

    /// <summary>
    /// Save the timing data to the CSV file.
    /// </summary>
    /// <param name="groupId">The group ID the part belongs to.</param>
    /// <param name="partId">The unique identifier for the part.</param>
    /// <param name="timeTaken">Time taken for the part assembly.</param>
    private void SaveTimeToCSV(string groupId, string partId, float timeTaken)
    {
        using (StreamWriter sw = File.AppendText(csvFilePath))
        {
            // Write in the format: GroupId;PartId;TimeTaken
            sw.WriteLine($"{groupId};{partId};{timeTaken}");
        }
        Debug.Log($"Saved time for GroupId: {groupId}, PartId: {partId}, TimeTaken: {timeTaken}");
    }
}
