using Unity.Entities;
using Unity.NetCode;
using Unity.Mathematics;

[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
public class ThinClientFakeInput : ComponentSystem
{
    private Random _random;

    protected override void OnCreate()
    {
        base.OnCreate();
        _random = new Random((uint) System.DateTime.Now.Ticks);
        RequireSingletonForUpdate<ThinClientComponent>();
        RequireSingletonForUpdate<CommandTargetComponent>();
        RequireSingletonForUpdate<NetworkIdComponent>();
    }

    protected override void OnUpdate()
    {
        if (HasSingleton<ThinClientComponent>())
        {
            var localInput = GetSingleton<CommandTargetComponent>().targetEntity;
            if (localInput == Entity.Null)
            {
                var inputEntity = EntityManager.CreateEntity(typeof(CubeInput));
                SetSingleton(new CommandTargetComponent() {targetEntity = inputEntity});

                EntityManager.AddComponent<MoveParams>(inputEntity);
                EntityManager.SetComponentData(inputEntity, new MoveParams()
                {
                    VerticalSpeed = _random.NextFloat(0.01f, 1f),
                    HorizontalSpeed = _random.NextFloat(0.1f, 1f)
                });
                return;
            }

            var moveParams = EntityManager.GetComponentData<MoveParams>(localInput);
            var input = default(CubeInput);
            input.tick = World.GetExistingSystem<ClientSimulationSystemGroup>().ServerTick;
            input.horizontal = (int) math.round(math.sin(UnityEngine.Time.time * moveParams.HorizontalSpeed * 1f));
            input.vertical = (int) math.round(math.sin(UnityEngine.Time.time * moveParams.VerticalSpeed * 1f));
            var inputBuffer = EntityManager.GetBuffer<CubeInput>(localInput);
            inputBuffer.AddCommandData(input);
        }
    }
}
