using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EnginePart : MonoBehaviour
{
    public AssemblyState State { get; private set; } // Part assembly state
    public string GroupId; // ID of the group this part belongs to
    public string PartId; // Unique identifier for this part
    private XRGrabInteractable grabInteractable;
    private TimeManager timeManager;
    public Vector3 OriginalScale { get; private set; }
    public bool HasOriginalScaleStored { get; private set; }

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
                grabInteractable.interactionLayers = InteractionLayerMask.GetMask("Unlocked");
                break;

            // Change the StopTiming call in SetState
            case AssemblyState.Assembled:
                grabInteractable.enabled = false;
                grabInteractable.interactionLayers = InteractionLayerMask.GetMask("AssembledPart");

                // Update timing call
                if (timeManager != null)
                {
                    timeManager.StopPartTiming(GroupId, PartId);
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
            SetState(AssemblyState.Assembled); // Transition to Assembled state
            Debug.Log($"Part {PartId}: State set to Assembled.");
            FindObjectOfType<TimingDisplayUI>()?.RefreshDisplay();

            // Notify the parent group
            var parentGroup = GetComponentInParent<EngineGroup>();
            if (parentGroup != null)
            {
                Debug.Log($"Part {PartId}: Notifying parent group {parentGroup.GroupId}.");
                parentGroup.OnPartAssembled();
            }
            else
            {
                Debug.LogWarning($"Part {PartId}: No parent group found.");
            }
        }
        else
        {
            Debug.LogWarning($"Part {PartId}: Cannot assemble because it is not unlocked.");
        }
    }

    public void StoreOriginalScale()
    {
        OriginalScale = transform.localScale;
        HasOriginalScaleStored = true;
    }
}
