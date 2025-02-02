using UnityEngine;
using System.Collections.Generic;

// Klasa zarządzająca grupą części silnika i ich procesem montażu
public class EngineGroup : MonoBehaviour
{
    public string GroupId; // Unikalne ID grupy
    public List<EnginePart> Parts = new List<EnginePart>(); // Lista części w grupie
    public List<EngineSocket> Sockets = new List<EngineSocket>(); // Lista gniazd w grupie
    private int assembledPartsCount = 0; // Licznik złożonych części
    public bool IsComplete => assembledPartsCount == Parts.Count; // Flaga zakończenia grupy

    // Odblokowuje całą grupę do montażu
    public void Unlock()
    {
        Debug.Log($"Group {GroupId} unlocked.");

        foreach (var part in Parts)
        {
            part.SetState(AssemblyState.Unlocked);
        }

        foreach (var socket in Sockets)
        {
            EnableSocket(socket, true);
        }
    }

    // Wywoływane przy złożeniu każdej części w grupie
    public void OnPartAssembled()
    {
        assembledPartsCount++;
        Debug.Log($"Group {GroupId}: {assembledPartsCount}/{Parts.Count} parts assembled.");

        if (assembledPartsCount > Parts.Count)
        {
            Debug.LogWarning($"Group {GroupId}: Assembled parts count exceeded total parts!");
            assembledPartsCount = Parts.Count;
        }

        if (IsComplete)
        {
            Debug.Log($"Group {GroupId}: All parts assembled. Notifying assembly manager.");
            NotifyAssemblyManager();
        }
    }

    // Powiadamia główny menedżer montażu o ukończeniu grupy
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

    // Kontroluje aktywność gniazd montażowych
    private void EnableSocket(EngineSocket socket, bool isActive)
    {
        socket.enabled = isActive;

        if (isActive)
        {
            socket.interactionLayers = LayerMask.GetMask("Unlocked");
        }
        else
        {
            socket.interactionLayers = LayerMask.GetMask("LockedSocket");
        }

        Debug.Log($"Socket {socket.name} is now {(isActive ? "active (Unlocked)" : "inactive (LockedSocket)")}");
    }
}
