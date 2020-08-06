using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Transforms;
using Unity.NetCode;

namespace Asteroids.Server
{
    [UpdateInGroup(typeof(ServerSimulationSystemGroup))]
    [UpdateAfter(typeof(AsteroidSystem))]
    [UpdateAfter(typeof(GhostSimulationSystemGroup))]
    [UpdateAfter(typeof(BulletAgeSystem))]
    // If this was a ghost which was just spawned the ghost system needs to see it before we can destroy it
    [UpdateAfter(typeof(AsteroidsGhostSendSystem))]
    public class CollisionSystem : JobComponentSystem
    {
        private EntityQuery shipGroup;
        private EntityQuery bulletGroup;
        private EntityQuery asteroidGroup;
        private EntityQuery m_LevelGroup;
        private BeginSimulationEntityCommandBufferSystem barrier;
        private NativeQueue<Entity> playerClearQueue;
        private EntityQuery settingsGroup;

        protected override void OnCreate()
        {
            shipGroup = GetEntityQuery(ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<CollisionSphereComponent>(), ComponentType.ReadOnly<ShipTagComponentData>());
            bulletGroup = GetEntityQuery(ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<CollisionSphereComponent>(), ComponentType.ReadOnly<BulletTagComponent>(),
                ComponentType.ReadOnly<BulletAgeComponent>());
            asteroidGroup = GetEntityQuery(ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<CollisionSphereComponent>(), ComponentType.ReadOnly<AsteroidTagComponentData>());
            barrier = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            playerClearQueue = new NativeQueue<Entity>(Allocator.Persistent);

            m_LevelGroup = GetEntityQuery(ComponentType.ReadWrite<LevelComponent>());
            RequireForUpdate(m_LevelGroup);

            settingsGroup = GetEntityQuery(ComponentType.ReadOnly<ServerSettings>());
        }

        protected override void OnDestroy()
        {
            playerClearQueue.Dispose();
        }

        [BurstCompile]
        struct DestroyAsteroidJob : IJobChunk
        {
            public EntityCommandBuffer.ParallelWriter commandBuffer;
            [ReadOnly] public NativeArray<ArchetypeChunk> bulletChunks;
            [ReadOnly] public ComponentTypeHandle<BulletAgeComponent> bulletAgeType;
            [ReadOnly] public ComponentTypeHandle<Translation> positionType;
            [ReadOnly] public ComponentTypeHandle<CollisionSphereComponent> sphereType;
            [ReadOnly] public EntityTypeHandle entityType;

            [ReadOnly] public NativeArray<LevelComponent> level;
            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var asteroidPos = chunk.GetNativeArray(positionType);
                var asteroidSphere = chunk.GetNativeArray(sphereType);
                var asteroidEntity = chunk.GetNativeArray(entityType);
                for (int asteroid = 0; asteroid < asteroidPos.Length; ++asteroid)
                {
                    var firstPos = asteroidPos[asteroid].Value.xy;
                    var firstRadius = asteroidSphere[asteroid].radius;
                    if (firstPos.x - firstRadius < 0 || firstPos.y - firstRadius < 0 ||
                        firstPos.x + firstRadius > level[0].width ||
                        firstPos.y + firstRadius > level[0].height)
                    {
                        commandBuffer.DestroyEntity(chunkIndex, asteroidEntity[asteroid]);
                        continue;
                    }
                    // TODO: can check asteroid / asteroid here if required
                    for (int bc = 0; bc < bulletChunks.Length; ++bc)
                    {
                        var bulletAge = bulletChunks[bc].GetNativeArray(bulletAgeType);
                        var bulletPos = bulletChunks[bc].GetNativeArray(positionType);
                        var bulletSphere = bulletChunks[bc].GetNativeArray(sphereType);
                        for (int bullet = 0; bullet < bulletAge.Length; ++bullet)
                        {
                            if (bulletAge[bullet].age > bulletAge[bullet].maxAge)
                                continue;
                            var secondPos = bulletPos[bullet].Value.xy;
                            var secondRadius = bulletSphere[bullet].radius;
                            if (Intersect(firstRadius, secondRadius, firstPos, secondPos))
                            {
                                commandBuffer.DestroyEntity(chunkIndex, asteroidEntity[asteroid]);
                            }
                        }
                    }
                }
            }
        }
        [BurstCompile]
        struct DestroyShipJob : IJobChunk
        {
            public EntityCommandBuffer.ParallelWriter commandBuffer;
            [ReadOnly] public NativeArray<ArchetypeChunk> asteroidChunks;
            [ReadOnly] public NativeArray<ArchetypeChunk> bulletChunks;
            [ReadOnly] public ComponentTypeHandle<BulletAgeComponent> bulletAgeType;
            [ReadOnly] public ComponentTypeHandle<Translation> positionType;
            [ReadOnly] public ComponentTypeHandle<CollisionSphereComponent> sphereType;
            [ReadOnly] public ComponentTypeHandle<PlayerIdComponentData> playerIdType;
            [ReadOnly] public EntityTypeHandle entityType;

            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<ServerSettings> serverSettings;

            public NativeQueue<Entity>.ParallelWriter playerClearQueue;

            [ReadOnly] public NativeArray<LevelComponent> level;
            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var shipPos = chunk.GetNativeArray(positionType);
                var shipSphere = chunk.GetNativeArray(sphereType);
                var shipPlayerId = chunk.GetNativeArray(playerIdType);
                var shipEntity = chunk.GetNativeArray(entityType);
                for (int ship = 0; ship < shipPos.Length; ++ship)
                {
                    int alive = 1;
                    var firstPos = shipPos[ship].Value.xy;
                    var firstRadius = shipSphere[ship].radius;
                    if (firstPos.x - firstRadius < 0 || firstPos.y - firstRadius < 0 ||
                        firstPos.x + firstRadius > level[0].width ||
                        firstPos.y + firstRadius > level[0].height)
                    {
                        if (shipPlayerId.IsCreated)
                            playerClearQueue.Enqueue(shipPlayerId[ship].PlayerEntity);
                        commandBuffer.DestroyEntity(chunkIndex, shipEntity[ship]);
                        continue;
                    }

                    if (serverSettings.Length > 0 && serverSettings[0].damageShips == 0)
                        continue;
                    /*for (int bc = 0; bc < bulletChunks.Length && alive != 0; ++bc)
                    {
                        var bulletAge = bulletChunks[bc].GetNativeArray(bulletAgeType);
                        var bulletPos = bulletChunks[bc].GetNativeArray(positionType);
                        var bulletSphere = bulletChunks[bc].GetNativeArray(sphereType);
                        for (int bullet = 0; bullet < bulletAge.Length; ++bullet)
                        {
                            if (bulletAge[bullet].age > bulletAge[bullet].maxAge)
                                continue;
                            var secondPos = bulletPos[bullet].Value.xy;
                            var secondRadius = bulletSphere[bullet].radius;
                            if (Intersect(firstRadius, secondRadius, firstPos, secondPos))
                            {
                                if (shipPlayerId.IsCreated)
                                    playerClearQueue.Enqueue(shipPlayerId[ship].PlayerEntity);
                                commandBuffer.DestroyEntity(chunkIndex, shipEntity[ship]);
                                alive = 0;
                                break;
                            }
                        }
                    }*/
                    for (int ac = 0; ac < asteroidChunks.Length && alive != 0; ++ac)
                    {
                        var asteroidPos = asteroidChunks[ac].GetNativeArray(positionType);
                        var asteroidSphere = asteroidChunks[ac].GetNativeArray(sphereType);
                        for (int asteroid = 0; asteroid < asteroidPos.Length; ++asteroid)
                        {
                            var secondPos = asteroidPos[asteroid].Value.xy;
                            var secondRadius = asteroidSphere[asteroid].radius;
                            if (Intersect(firstRadius, secondRadius, firstPos, secondPos))
                            {
                                if (shipPlayerId.IsCreated)
                                    playerClearQueue.Enqueue(shipPlayerId[ship].PlayerEntity);
                                commandBuffer.DestroyEntity(chunkIndex, shipEntity[ship]);
                                alive = 0;
                                break;
                            }
                        }
                    }
                }
            }
        }

        struct ClearShipPointerJob : IJob
        {
            public NativeQueue<Entity> playerClearQueue;
            public ComponentDataFromEntity<CommandTargetComponent> commandTarget;

            public void Execute()
            {
                Entity ent;
                while (playerClearQueue.TryDequeue(out ent))
                {
                    if (commandTarget.HasComponent(ent))
                    {
                        var state = commandTarget[ent];
                        state.targetEntity = Entity.Null;
                        commandTarget[ent] = state;
                    }
                }
            }
        }

        struct ChunkCleanupJob : IJob
        {
            [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<ArchetypeChunk> asteroidChunks;
            [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<ArchetypeChunk> bulletChunks;
            [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<LevelComponent> level;
            public void Execute()
            {
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            JobHandle bulletHandle;
            JobHandle asteroidHandle;
            JobHandle levelHandle;
            JobHandle settingsHandle;

            var asteroidJob = new DestroyAsteroidJob
            {
                commandBuffer = barrier.CreateCommandBuffer().AsParallelWriter(),
                bulletChunks = bulletGroup.CreateArchetypeChunkArrayAsync(Allocator.TempJob, out bulletHandle),
                bulletAgeType = GetComponentTypeHandle<BulletAgeComponent>(true),
                positionType = GetComponentTypeHandle<Translation>(true),
                sphereType = GetComponentTypeHandle<CollisionSphereComponent>(true),
                entityType = GetEntityTypeHandle(),
                level = m_LevelGroup.ToComponentDataArrayAsync<LevelComponent>(Allocator.TempJob, out levelHandle)
            };
            var shipJob = new DestroyShipJob
            {
                commandBuffer = barrier.CreateCommandBuffer().AsParallelWriter(),
                asteroidChunks = asteroidGroup.CreateArchetypeChunkArrayAsync(Allocator.TempJob, out asteroidHandle),
                bulletChunks = asteroidJob.bulletChunks,
                bulletAgeType = asteroidJob.bulletAgeType,
                positionType = asteroidJob.positionType,
                sphereType = asteroidJob.sphereType,
                playerIdType = GetComponentTypeHandle<PlayerIdComponentData>(),
                entityType = asteroidJob.entityType,
                serverSettings = settingsGroup.ToComponentDataArrayAsync<ServerSettings>(Allocator.TempJob, out settingsHandle),
                playerClearQueue = playerClearQueue.AsParallelWriter(),
                level = asteroidJob.level
            };
            var asteroidDep = JobHandle.CombineDependencies(inputDeps, bulletHandle, levelHandle);
            var shipDep = JobHandle.CombineDependencies(asteroidDep, asteroidHandle, settingsHandle);

            var h1 = asteroidJob.Schedule(asteroidGroup, asteroidDep);
            var h2 = shipJob.Schedule(shipGroup, shipDep);

            var handle = JobHandle.CombineDependencies(h1, h2);
            barrier.AddJobHandleForProducer(handle);

            var cleanupShipJob = new ClearShipPointerJob
            {
                playerClearQueue = playerClearQueue,
                commandTarget = GetComponentDataFromEntity<CommandTargetComponent>()
            };
            var cleanupChunkJob = new ChunkCleanupJob
            {
                bulletChunks = shipJob.bulletChunks,
                asteroidChunks = shipJob.asteroidChunks,
                level = shipJob.level
            };
            return JobHandle.CombineDependencies(cleanupShipJob.Schedule(h2), cleanupChunkJob.Schedule(handle));
        }

        private static bool Intersect(float firstRadius, float secondRadius, float2 firstPos, float2 secondPos)
        {
            float2 diff = firstPos - secondPos;
            float distSq = math.dot(diff, diff);

            return distSq <= (firstRadius + secondRadius) * (firstRadius + secondRadius);
        }
    }
}
