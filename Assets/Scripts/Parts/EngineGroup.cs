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

        // Unlock all parts in the group
        foreach (var part in Parts)
        {
            part.SetState(AssemblyState.Unlocked);
        }

        // Enable sockets associated with this group
        foreach (var socket in Sockets)
        {
            EnableSocket(socket, true); // Activate sockets
        }
    }

    /// <summary>
    /// Increases the count of assembled parts and notifies the manager if the group is complete.
    /// </summary>
    public void OnPartAssembled()
    {
        assembledPartsCount++;
        Debug.Log($"Group {GroupId}: {assembledPartsCount}/{Parts.Count} parts assembled.");

        if (assembledPartsCount > Parts.Count)
        {
            Debug.LogWarning($"Group {GroupId}: Assembled parts count exceeded total parts!");
            assembledPartsCount = Parts.Count; // Safety check
        }

        if (IsComplete)
        {
            Debug.Log($"Group {GroupId}: All parts assembled. Notifying assembly manager.");
            NotifyAssemblyManager();
        }
    }



    private void NotifyAssemblyManager()
    {
        var manager = FindObjectOfType<EngineAssemblyManager>();
        if (manager != null)
        {
            Debug.Log($"Group {GroupId}: Notifying assembly manager.");
            manager.OnGroupAssembled(this);
        }
        else
        {
            Debug.LogError($"Group {GroupId}: No assembly manager found in the scene.");
        }
    }


    /// <summary>
    /// Activates or deactivates a given socket for this group.
    /// </summary>
    private void EnableSocket(EngineSocket socket, bool isActive)
    {
        socket.enabled = isActive;

        if (isActive)
        {
            // Shared layer for both parts and sockets
            socket.interactionLayers = LayerMask.GetMask("Unlocked");
        }
        else
        {
            socket.interactionLayers = LayerMask.GetMask("LockedSocket");
        }

        Debug.Log($"Socket {socket.name} is now {(isActive ? "active (Unlocked)" : "inactive (LockedSocket)")}");
    }
}
