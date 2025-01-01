using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ButtonHaptics : MonoBehaviour
{
    public XRBaseController leftController;
    public XRBaseController rightController;

    public void TriggerHaptics()
    {
        // Log that the method was triggered
        Debug.Log("TriggerHaptics called.");

        // Find the interactor (controller) currently interacting with the button
        XRRayInteractor interactor = GetInteractor();
        if (interactor != null)
        {
            Debug.Log($"Interactor found: {interactor.name}");

            // Determine which controller is associated with the interactor
            XRBaseController activeController = interactor.name.Contains("Left") ? leftController : rightController;

            if (activeController != null)
            {
                // Log which controller is being used
                Debug.Log($"Sending haptics to: {(interactor.name.Contains("Left") ? "Left Controller" : "Right Controller")}");

                // Send haptic feedback
                activeController.SendHapticImpulse(0.5f, 0.1f); // Amplitude and duration
            }
            else
            {
                Debug.LogWarning("Active controller is null. Haptics not sent.");
            }
        }
        else
        {
            Debug.LogWarning("No interactor found. Cannot send haptics.");
        }
    }

    private XRRayInteractor GetInteractor()
    {
        // Log the raycasting process
        Debug.Log("Checking for interactor via raycasting.");

        // Find an active XRRayInteractor hitting this object
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.forward, out hit))
        {
            Debug.Log($"Raycast hit: {hit.collider.name}");
            return hit.collider.GetComponent<XRRayInteractor>();
        }

        Debug.LogWarning("Raycast did not hit an interactor.");
        return null;
    }
}
