using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ButtonHaptics : MonoBehaviour
{
    public XRBaseController leftController;
    public XRBaseController rightController;

    public void TriggerHaptics()
    {
        Debug.Log("TriggerHaptics called.");

        XRRayInteractor interactor = GetInteractor();
        if (interactor != null)
        {
            Debug.Log($"Interactor found: {interactor.name}");

            XRBaseController activeController = interactor.name.Contains("Left") ? leftController : rightController;

            if (activeController != null)
            {
                Debug.Log($"Sending haptics to: {(interactor.name.Contains("Left") ? "Left Controller" : "Right Controller")}");

                activeController.SendHapticImpulse(0.5f, 0.1f);
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
        Debug.Log("Checking for interactor via raycasting.");

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
