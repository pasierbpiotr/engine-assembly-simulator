using System.Collections.Generic;
using UnityEngine;

public class InstructionManager : MonoBehaviour
{
    [Header("Group Instructions")]
    public List<GroupInstruction> instructionsList; // List of all group instructions

    private Dictionary<string, GroupInstruction> instructionsDictionary;

    private void Awake()
    {
        InitializeDictionary();
    }

    /// <summary>
    /// Convert the instructions list into a dictionary for fast lookup.
    /// </summary>
    private void InitializeDictionary()
    {
        instructionsDictionary = new Dictionary<string, GroupInstruction>();

        foreach (var instruction in instructionsList)
        {
            if (!instructionsDictionary.ContainsKey(instruction.groupId))
            {
                instructionsDictionary.Add(instruction.groupId, instruction);
                Debug.Log($"Instruction added for GroupId: {instruction.groupId}");
            }
            else
            {
                Debug.LogWarning($"Duplicate GroupId found: {instruction.groupId}. Only the first will be used.");
            }
        }
    }

    /// <summary>
    /// Get the instruction for a specific group.
    /// </summary>
    /// <param name="groupId">The ID of the group.</param>
    /// <returns>The instruction for the group, or null if not found.</returns>
    public GroupInstruction GetInstructionForGroup(string groupId)
    {
        if (instructionsDictionary.TryGetValue(groupId, out var instruction))
        {
            return instruction;
        }

        Debug.LogWarning($"No instruction found for GroupId: {groupId}");
        return null;
    }
}
