using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EnginePart : MonoBehaviour
{
    public AssemblyState State { get; private set; } // Part assembly state
    public string GroupId; // ID of the group this part belongs to
    public string PartId; // Unique identifier for this part
    private XRGrabInteractable grabInteractable;
    private TimeManager timeManager;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        timeManager = FindObjectOfType<TimeManager>();
    }

    /// <summary>
    /// Sets the assembly state and handles interactivity accordingly.
    /// </summary>
    /// <param name="newState">The new assembly state.</param>
    public void SetState(AssemblyState newState)
    {
        State = newState;

        switch (State)
        {
            case AssemblyState.Locked:
                grabInteractable.interactionLayers = InteractionLayerMask.GetMask("LockedPart");
                break;

            case AssemblyState.Unlocked:
                grabInteractable.interactionLayers = InteractionLayerMask.GetMask("UnlockedPart");

                // Start timing as soon as the part becomes available to the player
                if (timeManager != null)
                {
                    timeManager.StartTiming(PartId);
                }
                break;

            case AssemblyState.Assembled:
                grabInteractable.enabled = false; // Disable grabbing interactions
                grabInteractable.interactionLayers = InteractionLayerMask.GetMask("Assembled"); // Change to assembled layer

                Debug.Log($"Part {PartId} is now assembled and no longer interactive.");

                // Stop timing when the part is assembled
                if (timeManager != null)
                {
                    timeManager.StopTiming(GroupId, PartId);
                }
                break;
        }

        Debug.Log($"Part {PartId}: State changed to {State}");
    }

    /// <summary>
    /// Notifies the parent group when the part is fully assembled.
    /// </summary>
    public void OnPartAssembled()
    {
        if (State == AssemblyState.Unlocked)
        {
            SetState(AssemblyState.Assembled); // Transition the part to assembled state

            // Stop the part from being affected by physics
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; // Make it non-physical
                rb.useGravity = false; // Disable gravity
            }

            // Notify the parent group
            var parentGroup = GetComponentInParent<EngineGroup>();
            if (parentGroup != null) parentGroup.OnPartAssembled();
            else Debug.LogWarning($"Part {PartId}: No parent group found to notify.");
        }
        else Debug.LogWarning($"Part {PartId}: Cannot assemble because it is not unlocked.");

    }
}
