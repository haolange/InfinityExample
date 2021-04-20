using InfinityTech.Rendering.GPUResource;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePoolTest : MonoBehaviour
{
    BufferDescription bufferADescription;
    BufferDescription bufferBDescription;
    FResourceFactory m_ResourceFactory;

    void OnEnable()
    {
        m_ResourceFactory = new FResourceFactory();

        bufferADescription = new BufferDescription(16, 4);
        bufferADescription.name = "BufferA";

        bufferBDescription = new BufferDescription(16, 4);
        bufferBDescription.name = "BufferB";
    }

    void Update()
    {
        BufferRef bufferA = m_ResourceFactory.AllocateBuffer(bufferADescription);
        BufferRef bufferB = m_ResourceFactory.AllocateBuffer(bufferBDescription);

        m_ResourceFactory.ReleaseBuffer(bufferA);
        m_ResourceFactory.ReleaseBuffer(bufferB);
    }

    void OnDisable()
    {
        m_ResourceFactory.Disposed();
    }
}
