using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Klasa reprezentująca część silnika z logiką interakcji i stanów montażu.
/// </summary>
public class EnginePart : MonoBehaviour
{
    // Aktualny stan montażu części.
    public AssemblyState State { get; private set; }

    // Identyfikator grupy, do której należy część.
    public string GroupId;

    // Komponent XRGrabInteractable odpowiedzialny za interakcję z częścią.
    private XRGrabInteractable grabInteractable;

    /// <summary>
    /// Metoda wywoływana przy inicjalizacji obiektu. Pobiera komponent XRGrabInteractable.
    /// </summary>
    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    /// <summary>
    /// Ustawia nowy stan montażu dla części i odpowiednio zmienia warstwy oraz warunki interakcji.
    /// </summary>
    /// <param name="newState">Nowy stan montażu.</param>
    public void SetState(AssemblyState newState)
    {
        State = newState;

        // Zmiana warstwy i interakcji w zależności od stanu montażu.
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
            grabInteractable.enabled = false; // Wyłączenie interakcji z zamontowaną częścią.
        }

        // Debugowanie informacji o stanie i warstwie części.
        Debug.Log($"Część {name}: Stan={State}, Warstwa={gameObject.layer}");
    }

    /// <summary>
    /// Wywoływana podczas montażu części. Informuje menedżera montażu o zmontowaniu części.
    /// </summary>
    public void OnPartAssembled()
    {
        if (State == AssemblyState.Unlocked)
        {
            // Znajduje menedżera montażu i zgłasza zmontowanie części.
            EngineAssemblyManager manager = FindObjectOfType<EngineAssemblyManager>();
            manager.AssemblePart(this);
        }
    }
}
