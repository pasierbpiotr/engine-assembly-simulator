using UnityEngine;
using System.Collections.Generic;

public class EngineGroup : MonoBehaviour
{
    public string GroupId; // ID of this group
    public List<EnginePart> Parts = new List<EnginePart>();
    public List<EngineSocket> Sockets = new List<EngineSocket>();

    private int assembledPartsCount = 0;

    public bool IsComplete => assembledPartsCount == Parts.Count;

    /// <summary>
    /// Unlocks all parts and sockets in this group.
    /// </summary>
    public void Unlock()
    {
        Debug.Log($"Group {GroupId} unlocked.");

        foreach (var part in Parts)
        {
            part.SetState(AssemblyState.Unlocked);
        }

        foreach (var socket in Sockets)
        {
            socket.SetSocketActive(true);
        }
    }

    /// <summary>
    /// Increases the count of assembled parts and notifies the manager if the group is complete.
    /// </summary>
    public void OnPartAssembled()
    {
        assembledPartsCount++;
        Debug.Log($"Group {GroupId}: {assembledPartsCount}/{Parts.Count} parts assembled.");

        if (IsComplete)
        {
            NotifyAssemblyManager();
        }
    }

    private void NotifyAssemblyManager()
    {
        var manager = FindObjectOfType<EngineAssemblyManager>();
        if (manager != null)
        {
            manager.OnGroupAssembled(this);
        }
    }
}
