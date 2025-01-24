using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class TimeManager : MonoBehaviour
{
    private Dictionary<string, float> partStartTime = new Dictionary<string, float>(); // Per-part start time
    private string csvFilePath; // File path for the CSV file

    private void Start()
    {
        // Set the file path to the persistent data path
        csvFilePath = Path.Combine(Application.persistentDataPath, "PartTimings.csv");
        InitializeCSVFile();
    }

    /// <summary>
    /// Create the CSV file if it doesn't already exist.
    /// </summary>
    private void InitializeCSVFile()
    {
        // Check if file already exists
        if (!File.Exists(csvFilePath))
        {
            // Create the file with headers
            using (StreamWriter sw = File.CreateText(csvFilePath))
            {
                sw.WriteLine("GroupId;PartId;TimeTaken");
            }

            Debug.Log($"CSV file created at path: {csvFilePath}");
        }
        else
        {
            Debug.Log($"CSV file already exists at path: {csvFilePath}");
        }
    }

    /// <summary>
    /// Start timing for a specific part.
    /// </summary>
    /// <param name="partId">Unique identifier for the part (e.g., part name).</param>
    public void StartTiming(string partId)
    {
        // Record the start time for the part
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
        // Ensure that timing for this part has been started
        if (partStartTime.ContainsKey(partId))
        {
            // Calculate the elapsed time
            float timeTaken = Time.time - partStartTime[partId];

            // Save the timing data to the CSV file
            SaveTimeToCSV(groupId, partId, timeTaken);

            // Remove the part from the dictionary to stop tracking
            partStartTime.Remove(partId);

            Debug.Log($"Timing stopped for part: {partId}, Time: {timeTaken} seconds.");
        }
        else
        {
            Debug.LogWarning($"No timing found for part: {partId}. Ensure StartTiming was called.");
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
        try
        {
            // Open the file in append mode and add the data
            using (StreamWriter sw = File.AppendText(csvFilePath))
            {
                // Write the timing in CSV format
                sw.WriteLine($"{groupId};{partId};{timeTaken}");
            }

            Debug.Log($"Saved timing for GroupId: {groupId}, PartId: {partId}, TimeTaken: {timeTaken}.");
        }
        catch (IOException ex)
        {
            Debug.LogError($"Failed to save data to CSV file: {ex.Message}");
        }
    }

    /// <summary>
    /// Get the file path where timing data is saved.
    /// </summary>
    public string GetCSVFilePath()
    {
        return csvFilePath;
    }
}
