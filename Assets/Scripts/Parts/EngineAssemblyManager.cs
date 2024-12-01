using UnityEngine;
using System.Collections.Generic;

public class EngineAssemblyManager : MonoBehaviour
{
    private Queue<EnginePart> assemblyQueue = new Queue<EnginePart>();
    public List<EnginePart> partsOrder;
    public List<EngineSocket> socketsOrder;

    void Start()
    {
        InitializeAssemblyQueue();
    }

    private void InitializeAssemblyQueue()
    {
        for (int i = 0; i < partsOrder.Count; i++)
        {
            EnginePart part = partsOrder[i];
            EngineSocket socket = socketsOrder[i];

            part.SetState(AssemblyState.Locked);
            socket.SetSocketActive(false);

            assemblyQueue.Enqueue(part);
        }

        if (assemblyQueue.Count > 0)
        {
            EnginePart firstPart = assemblyQueue.Peek();
            firstPart.SetState(AssemblyState.Unlocked);

            EngineSocket firstSocket = socketsOrder[0];
            firstSocket.SetSocketActive(true);
        }
    }

    public void AssemblePart(EnginePart part)
    {
        if (assemblyQueue.Peek() == part && part.State == AssemblyState.Unlocked)
        {
            part.SetState(AssemblyState.Assembled);
            assemblyQueue.Dequeue();

            if (assemblyQueue.Count > 0)
            {
                EnginePart nextPart = assemblyQueue.Peek();
                nextPart.SetState(AssemblyState.Unlocked);

                int nextIndex = partsOrder.IndexOf(nextPart);
                if (nextIndex >= 0 && nextIndex < socketsOrder.Count)
                {
                    EngineSocket nextSocket = socketsOrder[nextIndex];
                    nextSocket.SetSocketActive(true);
                }
            }
        }
    }
}
