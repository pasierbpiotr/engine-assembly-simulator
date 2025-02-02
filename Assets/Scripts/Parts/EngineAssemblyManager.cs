using UnityEngine;
using System.Collections.Generic;

// Główny menedżer procesu montażowego zarządzający sekwencyjną kompletacją grup części
public class EngineAssemblyManager : MonoBehaviour
{
    public List<EngineGroup> GroupsOrder; // Lista grup w kolejności montażowej
    private Queue<EngineGroup> groupQueue = new Queue<EngineGroup>(); // Kolejka grup do zmontowania

    [Header("Dependencies")]
    public InstructionScreen instructionScreen; // Referencja do interfejsu instrukcji
    public InstructionManager instructionManager; // Menadżer treści instruktażowych

    // Inicjalizacja kolejki grup na początku działania sceny
    private void Start()
    {
        InitializeGroupQueue();
    }

    // Przygotowuje kolejkę grup na podstawie zdefiniowanej kolejności
    private void InitializeGroupQueue()
    {
        foreach (var group in GroupsOrder)
        {
            groupQueue.Enqueue(group);
            Debug.Log($"AssemblyManager: Group {group.GroupId} added to the queue.");
        }

        UnlockNextGroup();
    }

    // Wywoływane po pełnym zmontowaniu grupy - rozpoczyna następny etap
    public void OnGroupAssembled(EngineGroup group)
    {
        Debug.Log($"AssemblyManager: Group {group.GroupId} is fully assembled.");
        UnlockNextGroup();
    }

    // Odblokowuje następną grupę w kolejce montażowej
    private void UnlockNextGroup()
    {
        if (groupQueue.Count > 0)
        {
            var nextGroup = groupQueue.Dequeue();
            if (nextGroup != null)
            {
                Debug.Log($"AssemblyManager: Unlocking next group {nextGroup.GroupId}.");
                nextGroup.Unlock();
                FindObjectOfType<TimeManager>().StartGroupTiming(nextGroup.GroupId);
                UpdateInstructionScreen(nextGroup.GroupId);
            }
            else
            {
                Debug.LogWarning("AssemblyManager: Next group in queue is null!");
            }
        }
        else
        {
            Debug.Log("AssemblyManager: All groups have been assembled!");
            instructionScreen.ResetInstructions();
        }
    }

    // Aktualizuje wyświetlane instrukcje dla aktualnej grupy
    private void UpdateInstructionScreen(string groupId)
    {
        if (instructionManager != null && instructionScreen != null)
        {
            var instruction = instructionManager.GetInstructionForGroup(groupId);
            if (instruction != null)
            {
                instructionScreen.UpdateInstructions(
                    instruction.instructionTitle,
                    instruction.instructionText
                );
            }
            else
            {
                instructionScreen.ResetInstructions();
            }
        }
    }
}
