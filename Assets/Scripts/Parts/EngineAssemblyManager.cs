using UnityEngine;
using System.Collections.Generic;

public class EngineAssemblyManager : MonoBehaviour
{
    private Queue<string> groupQueue = new Queue<string>();
    public List<EnginePart> partsOrder;
    public List<EngineSocket> socketsOrder;
    private Dictionary<string, int> groupPartCount = new Dictionary<string, int>();
    private Dictionary<string, int> groupAssembledCount = new Dictionary<string, int>();
    private TimeManager timeManager;

    void Start()
    {
        timeManager = FindObjectOfType<TimeManager>();
        InitializeAssemblyQueue();
    }

    private void InitializeAssemblyQueue()
    {
        foreach (var part in partsOrder)
        {
            if (!groupPartCount.ContainsKey(part.GroupId))
            {
                groupPartCount[part.GroupId] = 0;
                groupAssembledCount[part.GroupId] = 0;
                groupQueue.Enqueue(part.GroupId);
            }
            groupPartCount[part.GroupId]++;
        }

        UnlockNextGroup();
    }

    public void AssemblePart(EnginePart part)
    {
        if (part.State == AssemblyState.Unlocked)
        {
            part.SetState(AssemblyState.Assembled);
        }
    }

    public void OnPartAssembled(EnginePart part)
    {
        if (groupAssembledCount.ContainsKey(part.GroupId))
        {
            groupAssembledCount[part.GroupId]++;
            timeManager.StopTiming(part.GroupId, part.name); // Stop timing for the current part

            if (groupAssembledCount[part.GroupId] == groupPartCount[part.GroupId])
            {
                Debug.Log($"All parts in group {part.GroupId} have been assembled.");
                UnlockNextGroup();
            }
            else
            {
                timeManager.StartTiming(part.GroupId); // Start timing for the next part in the group
            }
        }
    }

    private void UnlockNextGroup()
    {
        if (groupQueue.Count > 0)
        {
            string nextGroupId = groupQueue.Dequeue();
            timeManager.StartTiming(nextGroupId);
            foreach (var part in partsOrder)
            {
                if (part.GroupId == nextGroupId)
                {
                    part.SetState(AssemblyState.Unlocked);
                }
            }
            foreach (var socket in socketsOrder)
            {
                if (socket.groupId == nextGroupId)
                {
                    socket.SetSocketActive(true);
                }
            }
        }
    }
}