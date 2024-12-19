using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EngineSocket : MonoBehaviour
{
    public string groupId; // Change this line to focus on groupId
    private XRSocketInteractor socketInteractor;
    private Renderer socketRenderer;
    private bool isPartAssembled = false;

    private void Awake()
    {
        socketInteractor = GetComponent<XRSocketInteractor>();
        socketRenderer = GetComponent<Renderer>();
        socketInteractor.selectExited.AddListener(OnPartAttemptedRelease);
    }

    private void Start()
    {
        SetSocketActive(false);
        socketInteractor.selectEntered.AddListener(OnPartPlaced);
    }

    private void OnDestroy()
    {
        socketInteractor.selectEntered.RemoveListener(OnPartPlaced);
        socketInteractor.selectExited.RemoveListener(OnPartAttemptedRelease);
    }

    public void SetSocketActive(bool isActive)
    {
        socketInteractor.enabled = true;

        if (isActive)
        {
            socketInteractor.interactionLayers = InteractionLayerMask.GetMask("UnlockedPart");
        }
        else
        {
            socketInteractor.interactionLayers = InteractionLayerMask.GetMask("None");
        }

        if (socketRenderer != null)
        {
            socketRenderer.enabled = true;
        }
    }

    private void OnPartPlaced(SelectEnterEventArgs args)
    {
        EnginePart part = args.interactableObject.transform.GetComponent<EnginePart>();

        if (part != null)
        {
            if (isPartAssembled)
            {
                Debug.Log($"Part {part.name} is already assembled.");
                return;
            }

            if (part.GroupId == groupId) // Check groupId
            {
                Debug.Log($"Correct part placed in socket: {part.name}");
                part.OnPartAssembled();
                isPartAssembled = true;

                Transform attachTransform = socketInteractor.attachTransform;

                if (attachTransform != null)
                {
                    part.transform.position = attachTransform.position;
                    part.transform.rotation = attachTransform.rotation;
                }
                else
                {
                    Debug.LogWarning("Attach Transform is not assigned to the socket. Part will not snap correctly.");
                }

                XRGrabInteractable grabInteractable = part.GetComponent<XRGrabInteractable>();
                if (grabInteractable != null)
                {
                    grabInteractable.enabled = false;
                }

                part.gameObject.layer = LayerMask.NameToLayer("AssembledPart");

                socketInteractor.enabled = false;
                socketInteractor.selectEntered.RemoveListener(OnPartPlaced);
                socketInteractor.selectExited.RemoveListener(OnPartAttemptedRelease);

                EngineAssemblyManager manager = FindObjectOfType<EngineAssemblyManager>();
                manager.OnPartAssembled(part);
            }
            else
            {
                Debug.LogError($"Incorrect part placed in socket: {part?.name}");

                if (socketInteractor.firstInteractableSelected != null)
                {
                    var interactable = socketInteractor.firstInteractableSelected;
                    socketInteractor.interactionManager.SelectExit(socketInteractor, interactable);
                }
            }
        }
    }

    private void OnPartAttemptedRelease(SelectExitEventArgs args)
    {
        EnginePart part = args.interactableObject.transform.GetComponent<EnginePart>();

        if (part != null && part.State == AssemblyState.Assembled)
        {
            Debug.Log($"Attempted release of locked part: {part.name}");

            if (!socketInteractor.hasSelection)
            {
                socketInteractor.interactionManager.SelectEnter(socketInteractor, args.interactableObject);

                Debug.Log($"Re-selected locked part: {part.name}");
            }
        }
    }
}