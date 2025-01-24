using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InstructionScreen : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI partTitle;       // Title Text UI
    public TextMeshProUGUI partDescription; // Description Text UI

    [Header("Default Values")]
    public string defaultTitle = "Awaiting Assembly";
    public string defaultDescription = "Start assembling the first part!";

    /// <summary>
    /// Updates the instruction screen with part details.
    /// </summary>
    public void UpdateInstructions(string title, string description)
    {
        // Update title and description
        partTitle.text = title;
        partDescription.text = description;

        Debug.Log($"InstructionScreen updated: Title = {title}, Description = {description}");
    }

    /// <summary>
    /// Resets the screen to default values.
    /// </summary>
    public void ResetInstructions()
    {
        UpdateInstructions(defaultTitle, defaultDescription);
    }
}
