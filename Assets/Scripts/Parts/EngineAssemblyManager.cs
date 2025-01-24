using UnityEngine;
using System.Collections.Generic;

public class EngineAssemblyManager : MonoBehaviour
{
    public List<EngineGroup> GroupsOrder; // Ordered list of groups
    private Queue<EngineGroup> groupQueue = new Queue<EngineGroup>();

    [Header("Dependencies")]
    public InstructionScreen instructionScreen;   // Reference to the InstructionScreen
    public InstructionManager instructionManager; // Reference to the InstructionManager

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
            Debug.Log($"AssemblyManager: Group {group.GroupId} added to the queue.");
        }

        UnlockNextGroup();
    }

    /// <summary>
    /// Called when a group is fully assembled. Unlocks the next group.
    /// </summary>
    public void OnGroupAssembled(EngineGroup group)
    {
        Debug.Log($"AssemblyManager: Group {group.GroupId} is fully assembled.");
        UnlockNextGroup();
    }

    private void UnlockNextGroup()
    {
        if (groupQueue.Count > 0)
        {
            var nextGroup = groupQueue.Dequeue();
            if (nextGroup != null)
            {
                Debug.Log($"AssemblyManager: Unlocking next group {nextGroup.GroupId}.");
                nextGroup.Unlock();
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

    /// <summary>
    /// Fetches the instruction for the group and updates the instruction screen.
    /// </summary>
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
