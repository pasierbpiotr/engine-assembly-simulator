using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Klasa zarządzająca ekranem instrukcji montażu części
public class InstructionScreen : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI partTitle;        // Pole tekstowe z nazwą części
    public TextMeshProUGUI partDescription;  // Pole tekstowe z opisem czynności

    [Header("Default Values")]
    public string defaultTitle = "Gratulacje!";
    public string defaultDescription = "Udało ci się złożyć silnik!";

    // Aktualizuje wyświetlane instrukcje
    public void UpdateInstructions(string title, string description)
    {
        partTitle.text = title;
        partDescription.text = description;

        Debug.Log($"InstructionScreen updated: Title = {title}, Description = {description}");
    }

    // Resetuje instrukcje do wartości domyślnych
    public void ResetInstructions()
    {
        UpdateInstructions(defaultTitle, defaultDescription);
    }
}
