using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Klasa zarządzająca stanem i interakcjami części silnika w środowisku VR
public class EnginePart : MonoBehaviour
{
 // Właściwości stanu części
    public AssemblyState State { get; private set; }
    public string GroupId;         // ID grupy, do której należy część
    public string PartId;          // Unikalne ID części
    private XRGrabInteractable grabInteractable; // Komponent do interakcji chwytania w VR
    private TimeManager timeManager;             // Menadżer czasu montażu
    public Vector3 OriginalScale { get; private set; } // Początkowa skala obiektu
    public bool HasOriginalScaleStored { get; private set; } // Flaga przechowywania skali

    // Inicjalizacja komponentów przy tworzeniu obiektu
    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        timeManager = FindObjectOfType<TimeManager>();
    }

    // Metoda zmieniająca stan części i dostosowująca jej interaktywność
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

            case AssemblyState.Assembled:
                grabInteractable.enabled = false;
                grabInteractable.interactionLayers = InteractionLayerMask.GetMask("AssembledPart");

                if (timeManager != null)
                {
                    timeManager.StopPartTiming(GroupId, PartId);
                }
                break;
        }

        Debug.Log($"Part {PartId}: State changed to {State}");
    }

    // Metoda wywoływana po prawidłowym złożeniu części
    public void OnPartAssembled()
    {
        if (State == AssemblyState.Unlocked)
        {
            SetState(AssemblyState.Assembled);
            Debug.Log($"Part {PartId}: State set to Assembled.");
            FindObjectOfType<TimingDisplayUI>()?.RefreshDisplay();

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

    // Metoda zapisująca początkową skalę obiektu - używana do CrankshaftBrace
    public void StoreOriginalScale()
    {
        OriginalScale = transform.localScale;
        HasOriginalScaleStored = true;
    }
}
