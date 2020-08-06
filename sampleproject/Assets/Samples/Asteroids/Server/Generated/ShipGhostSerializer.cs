using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Collections;
using Unity.NetCode;
using Unity.Transforms;

public struct ShipGhostSerializer : IGhostSerializer<ShipSnapshotData>
{
    private ComponentType componentTypeCollisionSphereComponent;
    private ComponentType componentTypePlayerIdComponentData;
    private ComponentType componentTypeShipCommandData;
    private ComponentType componentTypeShipStateComponentData;
    private ComponentType componentTypeShipTagComponentData;
    private ComponentType componentTypeRotation;
    private ComponentType componentTypeTranslation;
    private ComponentType componentTypeVelocity;
    // FIXME: These disable safety since all serializers have an instance of the same type - causing aliasing. Should be fixed in a cleaner way
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentTypeHandle<PlayerIdComponentData> ghostPlayerIdComponentDataType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentTypeHandle<ShipStateComponentData> ghostShipStateComponentDataType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentTypeHandle<Rotation> ghostRotationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentTypeHandle<Translation> ghostTranslationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ComponentTypeHandle<Velocity> ghostVelocityType;


    public int CalculateImportance(ArchetypeChunk chunk)
    {
        return 200;
    }

    public int SnapshotSize => UnsafeUtility.SizeOf<ShipSnapshotData>();
    public void BeginSerialize(ComponentSystemBase system)
    {
        componentTypeCollisionSphereComponent = ComponentType.ReadWrite<CollisionSphereComponent>();
        componentTypePlayerIdComponentData = ComponentType.ReadWrite<PlayerIdComponentData>();
        componentTypeShipCommandData = ComponentType.ReadWrite<ShipCommandData>();
        componentTypeShipStateComponentData = ComponentType.ReadWrite<ShipStateComponentData>();
        componentTypeShipTagComponentData = ComponentType.ReadWrite<ShipTagComponentData>();
        componentTypeRotation = ComponentType.ReadWrite<Rotation>();
        componentTypeTranslation = ComponentType.ReadWrite<Translation>();
        componentTypeVelocity = ComponentType.ReadWrite<Velocity>();
        ghostPlayerIdComponentDataType = system.GetComponentTypeHandle<PlayerIdComponentData>(true);
        ghostShipStateComponentDataType = system.GetComponentTypeHandle<ShipStateComponentData>(true);
        ghostRotationType = system.GetComponentTypeHandle<Rotation>(true);
        ghostTranslationType = system.GetComponentTypeHandle<Translation>(true);
        ghostVelocityType = system.GetComponentTypeHandle<Velocity>(true);
    }

    public void CopyToSnapshot(ArchetypeChunk chunk, int ent, uint tick, ref ShipSnapshotData snapshot, GhostSerializerState serializerState)
    {
        snapshot.tick = tick;
        var chunkDataPlayerIdComponentData = chunk.GetNativeArray(ghostPlayerIdComponentDataType);
        var chunkDataShipStateComponentData = chunk.GetNativeArray(ghostShipStateComponentDataType);
        var chunkDataRotation = chunk.GetNativeArray(ghostRotationType);
        var chunkDataTranslation = chunk.GetNativeArray(ghostTranslationType);
        var chunkDataVelocity = chunk.GetNativeArray(ghostVelocityType);
        snapshot.SetPlayerIdComponentDataPlayerId(chunkDataPlayerIdComponentData[ent].PlayerId, serializerState);
        snapshot.SetShipStateComponentDataState(chunkDataShipStateComponentData[ent].State, serializerState);
        snapshot.SetRotationValue(chunkDataRotation[ent].Value, serializerState);
        snapshot.SetTranslationValue(chunkDataTranslation[ent].Value, serializerState);
        snapshot.SetVelocityValue(chunkDataVelocity[ent].Value, serializerState);
    }
}
