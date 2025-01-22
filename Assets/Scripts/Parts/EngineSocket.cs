using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EngineSocket : XRSocketInteractor
{
    public string groupId; // Group this socket belongs to
    private bool isPartAssembled = false;

    /// <summary>
    /// Override CanSelect to enforce validation for compatible parts.
    /// </summary>
public override bool CanSelect(IXRSelectInteractable interactable)
{
    if (isPartAssembled) return false; // Prevent interaction if the part is already in place

    var enginePart = interactable.transform.GetComponent<EnginePart>();
    if (enginePart != null && enginePart.GroupId == groupId)
    {
        var grabInteractable = interactable as XRGrabInteractable;
        if (grabInteractable != null && grabInteractable.isSelected)
        {
            return false; // Prevent the socket from selecting the part while it is still being held.
        }

        return true; // The part is valid and can be selected by the socket.
    }

    return false; // Invalid part
}



    /// <summary>
    /// When a valid part is placed, handle snapping after letting go of the grab.
    /// </summary>
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        if (isPartAssembled) return;

        var part = args.interactableObject.transform.GetComponent<EnginePart>();
        if (part != null)
        {
            Debug.Log($"Socket {name}: Part {part.name} entered the socket.");

            var grabInteractable = part.GetComponent<XRGrabInteractable>();
            if (grabInteractable != null)
            {
                // Ensure we do not add duplicate listeners
                grabInteractable.selectExited.RemoveListener(OnGrabReleased);
                grabInteractable.selectExited.AddListener(OnGrabReleased);

                Debug.Log($"Socket {name}: Added OnGrabReleased listener for {part.name}.");
            }
        }
    }


    private float exitCooldown = 0.5f; // Half a second cooldown
    private float lastExitTime = -1f;

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if (isPartAssembled) return;

        // Check for cooldown
        if (Time.time - lastExitTime < exitCooldown)
        {
            Debug.Log($"Socket {name}: Ignoring rapid OnSelectExited calls.");
            return;
        }
        lastExitTime = Time.time;

        var part = args.interactableObject.transform.GetComponent<EnginePart>();
        if (part != null)
        {
            Debug.Log($"Socket {name}: Part {part.name} exited the socket.");
            var grabInteractable = part.GetComponent<XRGrabInteractable>();
            if (grabInteractable != null)
            {
                grabInteractable.selectExited.RemoveListener(OnGrabReleased);
                Debug.Log($"Socket {name}: Removed OnGrabReleased listener for {part.name}.");
            }
        }
    }




private void OnGrabReleased(SelectExitEventArgs args)
{
    var part = args.interactableObject.transform.GetComponent<EnginePart>();
    if (part == null || isPartAssembled) return;

    // Snap the part to the socket
    SnapPartToSocket(part);
    part.OnPartAssembled();

    // Disable part's interactivity
    var grabInteractable = part.GetComponent<XRGrabInteractable>();
    if (grabInteractable != null)
    {
        grabInteractable.enabled = false;
        Debug.Log($"Part {part.name}: Grabbing interaction disabled.");
    }

    // Change the socket's interaction layers to a non-interactive state
    interactionLayers = LayerMask.GetMask("AssembledSocket");
    Debug.Log($"Socket {name}: Interaction layers set to Assembled.");

    isPartAssembled = true; // Mark the socket as assembled

    Debug.Log($"Socket {name}: Part {part.name} successfully assembled.");
}






    /// <summary>
    /// Snaps the part to the socket's attach transform.
    /// </summary>
    private void SnapPartToSocket(EnginePart part)
    {
        var attachTransform = this.attachTransform;

        if (attachTransform != null)
        {
            part.transform.position = attachTransform.position;
            part.transform.rotation = attachTransform.rotation;
            part.transform.SetParent(attachTransform); // Parent it to prevent falling

            var rb = part.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
        else
        {
            Debug.LogWarning($"Socket {name} is missing an attach transform. Snapping skipped.");
        }
    }
}
