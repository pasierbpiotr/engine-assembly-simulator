using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewGroupInstruction", menuName = "Assembly/Group Instruction")]
public class GroupInstruction : ScriptableObject
{
    public string groupId;            // The ID of the group this instruction is for
    public string instructionTitle;   // Title of the instruction
    [TextArea(3, 5)]
    public string instructionText;    // Description of what to do for the group
}
