using UnityEngine;

// Klasa przechowująca konfigurację instrukcji dla grupy montażowej
[System.Serializable]
[CreateAssetMenu(fileName = "NewGroupInstruction", menuName = "Assembly/Group Instruction")]
public class GroupInstruction : ScriptableObject
{
    public string groupId;  // Unikalny ID grupy montażowej
    public string instructionTitle; // Nagłówek instrukcji
    [TextArea(3, 5)]
    public string instructionText; // Szczegółowy opis kroków montażowych
}
