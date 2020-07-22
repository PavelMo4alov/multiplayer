using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public class CustomBootstrap : ClientServerBootstrap
{
    public override bool Initialize(string defaultWorldName)
    {
        TypeManager.Initialize();

        var systems = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default);
        GenerateSystemLists(systems);

        var world = new World(defaultWorldName);
        World.DefaultGameObjectInjectionWorld = world;

        DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, ExplicitDefaultWorldSystems);
        ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);
#if UNITY_EDITOR || UNITY_CLIENT
        CreateClientWorld(world, "ClientWorld");

        for (int i = 0; i < 0; i++)
        {
            var clientWorld = CreateClientWorld(world,
                $"ThinWorld_{i.ToString()}");
            clientWorld.EntityManager.CreateEntity(typeof(ThinClientComponent));
        }
#endif
#if UNITY_SERVER //|| UNITY_EDITOR
        CreateServerWorld(world, "ServerWorld");
#endif
        return true;
    }
}
