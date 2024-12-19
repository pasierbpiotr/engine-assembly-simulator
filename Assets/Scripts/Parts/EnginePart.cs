using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EnginePart : MonoBehaviour
{
    public AssemblyState State { get; private set; }
    public string GroupId; // Add this line
    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    public void SetState(AssemblyState newState)
    {
        State = newState;

        if (State == AssemblyState.Locked)
        {
            gameObject.layer = LayerMask.NameToLayer("LockedPart");
            grabInteractable.interactionLayers = InteractionLayerMask.GetMask("LockedPart");
        }
        else if (State == AssemblyState.Unlocked)
        {
            gameObject.layer = LayerMask.NameToLayer("UnlockedPart");
            grabInteractable.interactionLayers = InteractionLayerMask.GetMask("UnlockedPart");
        }
        else if (State == AssemblyState.Assembled)
        {
            gameObject.layer = LayerMask.NameToLayer("AssembledPart");
            grabInteractable.interactionLayers = InteractionLayerMask.GetMask("AssembledPart");
            grabInteractable.enabled = false;
        }

        Debug.Log($"Part {name}: State={State}, Layer={gameObject.layer}");
    }

    public void OnPartAssembled()
    {
        if (State == AssemblyState.Unlocked)
        {
            EngineAssemblyManager manager = FindObjectOfType<EngineAssemblyManager>();
            manager.AssemblePart(this);
        }
    }
}