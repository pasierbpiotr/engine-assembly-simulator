using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Główna klasa zarządzająca gniazdami montażowymi części silnika w VR
public class EngineSocket : XRSocketInteractor
{
    public string groupId; // ID grupy dla dopasowania części
    private bool isPartAssembled = false; // Flaga złożenia części

    // Sprawdza możliwość umieszczenia części w gnieździe
    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        if (isPartAssembled) return false;

        var enginePart = interactable.transform.GetComponent<EnginePart>();
        if (enginePart != null && enginePart.GroupId == groupId)
        {
            var grabInteractable = interactable as XRGrabInteractable;
            if (grabInteractable != null && grabInteractable.isSelected)
            {
                return false;
            }

            return true;
        }

        return false;
    }

    // Obsługa wejścia części do gniazda
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
                grabInteractable.selectExited.RemoveListener(OnGrabReleased);
                grabInteractable.selectExited.AddListener(OnGrabReleased);

                Debug.Log($"Socket {name}: Added OnGrabReleased listener for {part.name}.");
            }
        }
    }

    // Zabezpieczenie przed szybkimi wyjściami
    private float exitCooldown = 0.5f;
    private float lastExitTime = -1f;

    // Obsługa próby wyjęcia części z gniazda
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if (isPartAssembled) return;

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

    // Logika finalnego montażu części
    private void OnGrabReleased(SelectExitEventArgs args)
    {
        var part = args.interactableObject.transform.GetComponent<EnginePart>();
        if (part == null || isPartAssembled) return;

        SnapPartToSocket(part);
        part.OnPartAssembled();

        var grabInteractable = part.GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.enabled = false;
            Debug.Log($"Part {part.name}: Grabbing interaction disabled.");
        }

        interactionLayers = LayerMask.GetMask("AssembledSocket");
        Debug.Log($"Socket {name}: Interaction layers set to Assembled.");

        isPartAssembled = true;

        Debug.Log($"Socket {name}: Part {part.name} successfully assembled.");
    }

    // Metoda przyciągania i konfiguracji części
    private void SnapPartToSocket(EnginePart part)
    {
        var attachTransform = this.attachTransform;

        if (attachTransform != null)
        {
            part.transform.position = attachTransform.position;
            part.transform.rotation = attachTransform.rotation;
            part.transform.SetParent(attachTransform);

            var braceSocket = GetComponent<CamshaftSocketConfig>();
            if (braceSocket != null && braceSocket.isCamshaftBraceSocket)
            {
                if (!part.HasOriginalScaleStored)
                {
                    part.StoreOriginalScale();
                }
                
                part.transform.localScale = Vector3.Scale(
                    part.OriginalScale,
                    braceSocket.sizeMultiplier
                );
            }

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
