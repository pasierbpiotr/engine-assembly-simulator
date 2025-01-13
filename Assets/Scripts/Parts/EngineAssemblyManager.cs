using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Zarządza procesem montażu części silnika, zapewniając, że części i gniazda są odblokowywane 
/// i montowane w określonej kolejności na podstawie grup.
/// </summary>
public class EngineAssemblyManager : MonoBehaviour
{
    // Kolejka zarządzająca kolejnością odblokowywania grup.
    private Queue<string> groupQueue = new Queue<string>();

    // Lista części w kolejności montażu.
    public List<EnginePart> partsOrder;

    // Lista gniazd odpowiadających częściom.
    public List<EngineSocket> socketsOrder;

    // Słownik śledzący liczbę części w każdej grupie.
    private Dictionary<string, int> groupPartCount = new Dictionary<string, int>();

    // Słownik śledzący liczbę zamontowanych części w każdej grupie.
    private Dictionary<string, int> groupAssembledCount = new Dictionary<string, int>();

    // Odwołanie do TimeManager do śledzenia czasu montażu.
    private TimeManager timeManager;

    /// <summary>
    /// Inicjuje manager montażu, odnajdując TimeManager i ustawiając kolejkę grup.
    /// </summary>
    private void Start()
    {
        timeManager = FindObjectOfType<TimeManager>();
        InitializeAssemblyQueue();
    }

    /// <summary>
    /// Ustawia kolejkę montażową poprzez grupowanie części i inicjalizację liczników dla każdej grupy.
    /// </summary>
    private void InitializeAssemblyQueue()
    {
        foreach (var part in partsOrder)
        {
            // Inicjalizacja liczników grup, jeśli jeszcze nie zostały ustawione.
            if (!groupPartCount.ContainsKey(part.GroupId))
            {
                groupPartCount[part.GroupId] = 0;
                groupAssembledCount[part.GroupId] = 0;
                groupQueue.Enqueue(part.GroupId); // Dodanie grupy do kolejki w celu odblokowania.
            }
            groupPartCount[part.GroupId]++;
        }

        // Odblokowanie pierwszej grupy w kolejce.
        UnlockNextGroup();
    }

    /// <summary>
    /// Oznacza część jako zamontowaną poprzez zmianę jej stanu.
    /// </summary>
    /// <param name="part">Część do zamontowania.</param>
    public void AssemblePart(EnginePart part)
    {
        if (part.State == AssemblyState.Unlocked)
        {
            part.SetState(AssemblyState.Assembled);
        }
    }

    /// <summary>
    /// Obsługuje zdarzenie, gdy część została prawidłowo zamontowana.
    /// Aktualizuje licznik zamontowanych części dla grupy i odblokowuje kolejną grupę, jeśli wszystkie części zostały zamontowane.
    /// </summary>
    /// <param name="part">Zamontowana część.</param>
    public void OnPartAssembled(EnginePart part)
    {
        if (!groupAssembledCount.ContainsKey(part.GroupId))
        {
            Debug.LogError($"GroupId {part.GroupId} nie znaleziono w liczniku zamontowanych części.");
            return;
        }

        // Zwiększenie licznika zamontowanych części dla grupy.
        groupAssembledCount[part.GroupId]++;
        timeManager.StopTiming(part.GroupId, part.name); // Use GetElapsedTime() if necessary
        Debug.Log($"Elapsed Time for {part.name}: {timeManager.GetElapsedTime()} seconds");


        // Jeśli wszystkie części w grupie zostały zamontowane, odblokuj następną grupę.
        if (groupAssembledCount[part.GroupId] == groupPartCount[part.GroupId])
        {
            Debug.Log($"Wszystkie części w grupie {part.GroupId} zostały zamontowane.");
            UnlockNextGroup();
        }
        else
        {
            // Rozpocznij pomiar czasu dla następnej części w bieżącej grupie.
            timeManager.StartTiming(part.GroupId);
        }
    }

    /// <summary>
    /// Odblokowuje następną grupę części i gniazd do montażu.
    /// </summary>
    private void UnlockNextGroup()
    {
        if (groupQueue.Count > 0)
        {
            string nextGroupId = groupQueue.Dequeue();
            Debug.Log($"Odblokowywanie grupy {nextGroupId} do montażu.");

            // Rozpocznij pomiar czasu dla grupy.
            timeManager.StartTiming(nextGroupId);

            // Odblokowanie wszystkich części w grupie.
            foreach (var part in partsOrder)
            {
                if (part.GroupId == nextGroupId)
                {
                    part.SetState(AssemblyState.Unlocked);
                }
            }

            // Aktywacja wszystkich gniazd odpowiadających grupie.
            foreach (var socket in socketsOrder)
            {
                if (socket.groupId == nextGroupId)
                {
                    socket.SetSocketActive(true);
                }
            }
        }
        else
        {
            Debug.Log("Wszystkie grupy zostały zamontowane.");
        }
    }
}
