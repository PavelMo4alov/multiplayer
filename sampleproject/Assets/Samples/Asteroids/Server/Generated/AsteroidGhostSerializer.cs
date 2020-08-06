using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Collections;
using Unity.NetCode;
using Unity.Transforms;

public struct AsteroidGhostSerializer : IGhostSerializer<AsteroidSnapshotData>
{
    private ComponentType componentTypeAsteroidTagComponentData;
    private ComponentType componentTypeCollisionSphereComponent;
    private ComponentType componentTypeRotation;
    private ComponentType componentTypeTranslation;
    private ComponentType componentTypeVelocity;
    // FIXME: These disable safety since all serializers have an instance of the same type - causing aliasing. Should be fixed in a cleaner way
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentTypeHandle<Rotation> ghostRotationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentTypeHandle<Translation> ghostTranslationType;


    public int CalculateImportance(ArchetypeChunk chunk)
    {
        return 1;
    }

    public int SnapshotSize => UnsafeUtility.SizeOf<AsteroidSnapshotData>();
    public void BeginSerialize(ComponentSystemBase system)
    {
        componentTypeAsteroidTagComponentData = ComponentType.ReadWrite<AsteroidTagComponentData>();
        componentTypeCollisionSphereComponent = ComponentType.ReadWrite<CollisionSphereComponent>();
        componentTypeRotation = ComponentType.ReadWrite<Rotation>();
        componentTypeTranslation = ComponentType.ReadWrite<Translation>();
        componentTypeVelocity = ComponentType.ReadWrite<Velocity>();
        ghostRotationType = system.GetComponentTypeHandle<Rotation>(true);
        ghostTranslationType = system.GetComponentTypeHandle<Translation>(true);
    }

    public void CopyToSnapshot(ArchetypeChunk chunk, int ent, uint tick, ref AsteroidSnapshotData snapshot, GhostSerializerState serializerState)
    {
        snapshot.tick = tick;
        var chunkDataRotation = chunk.GetNativeArray(ghostRotationType);
        var chunkDataTranslation = chunk.GetNativeArray(ghostTranslationType);
        snapshot.SetRotationValue(chunkDataRotation[ent].Value, serializerState);
        snapshot.SetTranslationValue(chunkDataTranslation[ent].Value, serializerState);
    }
}
