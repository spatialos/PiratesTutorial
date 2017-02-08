using Improbable.Unity;
using Improbable.Unity.Configuration;
using Improbable.Unity.Core;
using UnityEngine;
using Improbable.Player;
using Improbable;
using Improbable.Worker;
using Improbable.Collections;
using Improbable.Math;
using Improbable.Unity.Core.EntityQueries;

// Placed on a gameobject in client scene to execute connection logic on client startup
public class Bootstrap : MonoBehaviour
{
    public WorkerConfigurationData Configuration = new WorkerConfigurationData();

    public void Start()
    {
        SpatialOS.ApplyConfiguration(Configuration);

        switch (SpatialOS.Configuration.WorkerPlatform)
        {
            case WorkerPlatform.UnityWorker:
                SpatialOS.OnDisconnected += reason => Application.Quit();

                var targetFramerate = 120;
                var fixedFramerate = 20;

                Application.targetFrameRate = targetFramerate;
                Time.fixedDeltaTime = 1.0f / fixedFramerate;
                break;
            case WorkerPlatform.UnityClient:
                SpatialOS.OnConnected += OnConnected;
                break;
        }

        SpatialOS.Connect(gameObject);
    }

    public void OnConnected()
    {
        Debug.Log("Bootstrap connected to SpatialOS, finding player spawner...");
        GetPlayerSpawnerEntityId((playerSpawnerEntityId, errorMessage) =>
        {
            if (errorMessage != null)
            {
                Debug.LogError("Bootstrap failed to get player spawner entity id: " + errorMessage);
                return;
            }
            Debug.Log("Bootstrap found player spawner with entity id: " + playerSpawnerEntityId.Value);

            SpatialOS.WorkerCommands.SendCommand(Spawner.Commands.SpawnPlayer.Descriptor,
                                                 new SpawnPlayerRequest(SpatialOS.Configuration.EngineId,
                                                 new Coordinates(0, 0, 0)),
                                                 playerSpawnerEntityId.Value,
                                                 result =>
                {
                    if (result.StatusCode != StatusCode.Success)
                    {
                        Debug.LogError("Bootstrap spawn player command failed: " + result.ErrorMessage);
                        return;
                    }
                    Debug.Log("Bootstrap created a player entity with ID: " + result.Response.Value.entityId);
                });
        });
    }

    private delegate void GetPlayerSpawnerEntityIdDelegate(Option<EntityId> playerSpawnerEntityId, string errorMessage);
    private void GetPlayerSpawnerEntityId(GetPlayerSpawnerEntityIdDelegate callback)
    {
        SpatialOS.WorkerCommands.SendQuery(Query.HasComponent<Spawner>().ReturnOnlyEntityIds(), result =>
        {
            if (result.StatusCode != StatusCode.Success || !result.Response.HasValue)
            {
                callback(null, "Bootstrap find player spawner query failed with error: " + result.ErrorMessage);
                return;
            }

            var response = result.Response.Value;
            if (response.EntityCount < 1)
            {
                callback(null, "Bootstrap failed to find player spawner: no entities found with the PlayerSpawner component");
                return;
            }
            var playerSpawnerEntityId = response.Entities.First.Value.Key;
            callback(new Option<EntityId>(playerSpawnerEntityId), null);
        });
    }
}