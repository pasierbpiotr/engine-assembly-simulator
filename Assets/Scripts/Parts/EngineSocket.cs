using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EngineSocket : MonoBehaviour
{
    public string groupId; // Group this socket belongs to

    private XRSocketInteractor socketInteractor;
    private Renderer socketRenderer;
    private bool isPartAssembled = false;

    private void Awake()
    {
        socketInteractor = GetComponent<XRSocketInteractor>();
        socketRenderer = GetComponent<Renderer>();

        // Listen to part placement and removal
        socketInteractor.selectEntered.AddListener(OnPartPlaced);
    }

    private void OnDestroy()
    {
        // Cleanup listeners
        socketInteractor.selectEntered.RemoveListener(OnPartPlaced);
    }

    /// <summary>
    /// Activates or deactivates the socket for interactivity.
    /// </summary>
    /// <param name="isActive">Whether the socket should be active.</param>
    public void SetSocketActive(bool isActive)
    {
        socketInteractor.enabled = isActive;

        if (isActive)
        {
            socketInteractor.interactionLayers = LayerMask.GetMask("UnlockedPart");
        }
        else
        {
            socketInteractor.interactionLayers = LayerMask.GetMask("Assembled"); // Optionally, disallow future interactions
        }

        if (socketRenderer != null)
        {
            socketRenderer.enabled = isActive;
        }

        Debug.Log($"Socket {name} set active: {isActive}");
    }


    /// <summary>
    /// Called when a part is placed in the socket.
    /// </summary>
    private void OnPartPlaced(SelectEnterEventArgs args)
    {
        if (isPartAssembled)
        {
            Debug.LogWarning($"Socket {name} already has an assembled part.");
            return;
        }

        EnginePart part = args.interactableObject.transform.GetComponent<EnginePart>();

        if (part != null && part.GroupId == groupId)
        {
            Debug.Log($"Part {part.PartId} placed in socket {name}. Starting assembly...");
            isPartAssembled = true;

            part.OnPartAssembled(); // Notify the part and its group
            SnapPartToSocket(part); // Align the part to the socket

            // Disable grab interactions to prevent interference
            var grabInteractable = part.GetComponent<XRGrabInteractable>();
            if (grabInteractable != null)
            {
                grabInteractable.enabled = false; // Prevent future grabs
            }

            // Optional: Parent the part to the socket for stability
            part.transform.SetParent(this.transform);

            // Optionally, deactivate this socket
            SetSocketActive(false);

            Debug.Log($"Part {part.PartId} assembled into socket {name}.");
        }
        else
        {
            Debug.LogError($"Part {part?.name} does not belong to group {groupId} and cannot be placed in socket {name}.");
        }
    }



    private void SnapPartToSocket(EnginePart part)
    {
        var attachTransform = socketInteractor.attachTransform;

        if (attachTransform == null)
        {
            Debug.LogWarning($"Socket {name} is missing an attach transform. The part might not snap.");
            return; // Exit early
        }

        // Snap the part's position and rotation
        part.transform.position = attachTransform.position;
        part.transform.rotation = attachTransform.rotation;

        // Parent the part to the socket for stability
        part.transform.SetParent(attachTransform);

        Debug.Log($"Part {part.PartId} successfully snapped to socket {name} at position {attachTransform.position}, rotation {attachTransform.rotation}.");
    }
}
