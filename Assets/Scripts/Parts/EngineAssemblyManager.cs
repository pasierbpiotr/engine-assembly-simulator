using UnityEngine;
using System.Collections.Generic;

public class EngineAssemblyManager : MonoBehaviour
{
    public List<EngineGroup> GroupsOrder; // Ordered list of groups
    private Queue<EngineGroup> groupQueue = new Queue<EngineGroup>();

    private void Start()
    {
        InitializeGroupQueue();
    }

    /// <summary>
    /// Enqueue all groups and unlock the first one.
    /// </summary>
    private void InitializeGroupQueue()
    {
        foreach (var group in GroupsOrder)
        {
            groupQueue.Enqueue(group);
        }

        UnlockNextGroup();
    }

    /// <summary>
    /// Called when a group is fully assembled. Unlocks the next group.
    /// </summary>
    public void OnGroupAssembled(EngineGroup group)
    {
        Debug.Log($"Group {group.GroupId} completed.");
        UnlockNextGroup();
    }

    /// <summary>
    /// Unlocks the next group in the queue if available.
    /// </summary>
    private void UnlockNextGroup()
    {
        if (groupQueue.Count > 0)
        {
            var nextGroup = groupQueue.Dequeue();
            nextGroup.Unlock();
        }
        else
        {
            Debug.Log("All groups have been assembled!");
        }
    }
}
