using System.Collections.Generic;
using UnityEngine;

// Główny menedżer odpowiedzialny za zarządzanie instrukcjami montażowymi grup
public class InstructionManager : MonoBehaviour
{
    [Header("Group Instructions")]
    public List<GroupInstruction> instructionsList; // Lista instrukcji ładowanych z inspektora
    private Dictionary<string, GroupInstruction> instructionsDictionary; // Słownik dla szybkiego dostępu

    // Inicjalizacja przy starcie obiektu
    private void Awake()
    {
        InitializeDictionary();
    }

    // Konwertuje listę instrukcji na optymalizowany słownik
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

    // Pobiera instrukcję dla danej grupy montażowej
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
