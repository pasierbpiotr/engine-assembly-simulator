using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Klasa reprezentująca gniazdo montażowe dla części silnika.
/// Odpowiada za interakcje z częściami i zarządzanie ich stanem.
/// </summary>
public class EngineSocket : MonoBehaviour
{
    /// <summary>
    /// Identyfikator grupy, który określa, które części są odpowiednie dla tego gniazda.
    /// </summary>
    public string groupId;

    private XRSocketInteractor socketInteractor;
    private Renderer socketRenderer;
    private bool isPartAssembled = false;

    private void Awake()
    {
        // Pobieranie komponentów gniazda montażowego.
        socketInteractor = GetComponent<XRSocketInteractor>();
        socketRenderer = GetComponent<Renderer>();

        // Dodanie nasłuchiwania na zdarzenie opuszczenia przez część.
        socketInteractor.selectExited.AddListener(OnPartAttemptedRelease);
    }

    private void Start()
    {
        // Ustawienie domyślnego stanu gniazda jako nieaktywnego.
        SetSocketActive(false);

        // Dodanie nasłuchiwania na zdarzenie umieszczenia części w gnieździe.
        socketInteractor.selectEntered.AddListener(OnPartPlaced);
    }

    private void OnDestroy()
    {
        // Usunięcie nasłuchiwania na zdarzenia, aby zapobiec błędom podczas niszczenia obiektu.
        socketInteractor.selectEntered.RemoveListener(OnPartPlaced);
        socketInteractor.selectExited.RemoveListener(OnPartAttemptedRelease);
    }

    /// <summary>
    /// Ustawia aktywność gniazda montażowego.
    /// </summary>
    /// <param name="isActive">Czy gniazdo ma być aktywne?</param>
    public void SetSocketActive(bool isActive)
    {
        socketInteractor.enabled = true;

        // Ustawianie warstwy interakcji w zależności od aktywności.
        if (isActive)
        {
            socketInteractor.interactionLayers = InteractionLayerMask.GetMask("UnlockedPart");
        }
        else
        {
            socketInteractor.interactionLayers = InteractionLayerMask.GetMask("None");
        }

        // Włączenie renderowania gniazda, jeśli istnieje renderer.
        if (socketRenderer != null)
        {
            socketRenderer.enabled = true;
        }
    }

    /// <summary>
    /// Obsługuje zdarzenie, gdy część zostaje umieszczona w gnieździe.
    /// </summary>
    /// <param name="args">Argumenty zdarzenia.</param>
    private void OnPartPlaced(SelectEnterEventArgs args)
    {
        EnginePart part = args.interactableObject.transform.GetComponent<EnginePart>();

        if (part != null)
        {
            // Sprawdzenie, czy część została już zamontowana.
            if (isPartAssembled)
            {
                Debug.Log($"Part {part.name} is already assembled.");
                return;
            }

            // Sprawdzenie, czy część należy do odpowiedniej grupy.
            if (part.GroupId == groupId)
            {
                Debug.Log($"Correct part placed in socket: {part.name}");
                part.OnPartAssembled();
                isPartAssembled = true;

                // Dopasowanie pozycji i rotacji części do gniazda.
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

                // Wyłączenie interakcji z zamontowaną częścią.
                XRGrabInteractable grabInteractable = part.GetComponent<XRGrabInteractable>();
                if (grabInteractable != null)
                {
                    grabInteractable.enabled = false;
                }

                part.gameObject.layer = LayerMask.NameToLayer("AssembledPart");

                // Wyłączenie gniazda i usunięcie nasłuchiwania na zdarzenia.
                socketInteractor.enabled = false;
                socketInteractor.selectEntered.RemoveListener(OnPartPlaced);
                socketInteractor.selectExited.RemoveListener(OnPartAttemptedRelease);

                // Powiadomienie menedżera o zamontowaniu części.
                EngineAssemblyManager manager = FindObjectOfType<EngineAssemblyManager>();
                manager.OnPartAssembled(part);
            }
            else
            {
                Debug.LogError($"Incorrect part placed in socket: {part?.name}");

                // Wyjęcie niewłaściwej części z gniazda.
                if (socketInteractor.firstInteractableSelected != null)
                {
                    var interactable = socketInteractor.firstInteractableSelected;
                    socketInteractor.interactionManager.SelectExit(socketInteractor, interactable);
                }
            }
        }
    }

    /// <summary>
    /// Obsługuje zdarzenie, gdy użytkownik próbuje zwolnić zamontowaną część.
    /// </summary>
    /// <param name="args">Argumenty zdarzenia.</param>
    private void OnPartAttemptedRelease(SelectExitEventArgs args)
    {
        EnginePart part = args.interactableObject.transform.GetComponent<EnginePart>();

        if (part != null && part.State == AssemblyState.Assembled)
        {
            Debug.Log($"Attempted release of locked part: {part.name}");

            // Upewnienie się, że zamontowana część pozostaje w gnieździe.
            if (!socketInteractor.hasSelection)
            {
                socketInteractor.interactionManager.SelectEnter(socketInteractor, args.interactableObject);

                Debug.Log($"Re-selected locked part: {part.name}");
            }
        }
    }
}
