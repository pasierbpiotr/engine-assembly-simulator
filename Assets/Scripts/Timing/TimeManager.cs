using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class TimeManager : MonoBehaviour
{

    private void Start()
    {
    }


    private Dictionary<string, float> groupStartTimes = new Dictionary<string, float>();
    private Dictionary<string, float> lastPartTimes = new Dictionary<string, float>();
    private List<TimingEntry> timingRecords = new List<TimingEntry>();

    /// <summary>
    /// Record when a group becomes available
    /// </summary>
    public void StartGroupTiming(string groupId)
    {
        groupStartTimes[groupId] = Time.time;
        lastPartTimes[groupId] = Time.time;
        Debug.Log($"Timing started for group: {groupId}");
    }

    /// <summary>
    /// Record time between parts in the same group
    /// </summary>
    public void StopPartTiming(string groupId, string partId)
    {
        if (!groupStartTimes.ContainsKey(groupId))
        {
            Debug.LogError($"Group {groupId} timing not started!");
            return;
        }

        float timeTaken = Time.time - lastPartTimes[groupId];
        timingRecords.Add(new TimingEntry(groupId, partId, timeTaken));
        
        // Update last part time for next calculation
        lastPartTimes[groupId] = Time.time;
        
        Debug.Log($"Added timing entry: {groupId} - {partId} | Time: {timeTaken:F1}s");
    }

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
