using Improbable.Entity.Component;
using UnityEngine;
using Improbable.Unity.Visualizer;
using Improbable.Player;
using Improbable.Worker;
using Improbable.Unity;
using Improbable.Unity.Core;
using Assets.EntityTemplates;

namespace Assets.Gamelogic.Pirates.Behaviours
{
    // Enable this MonoBehaviour on FSim (server-side) workers only
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class PlayerSpawnManager : MonoBehaviour
    {
        // Enable this MonoBehaviour only on the worker which has write-access for the entity's Spawner component
        [Require] private Spawner.Writer SpawnerWriter;

        void OnEnable()
        {
            // Register command callback
            SpawnerWriter.CommandReceiver.OnSpawnPlayer += OnSpawnPlayer;
        }

        void OnDisable()
        {
            // Deregister command callback
            SpawnerWriter.CommandReceiver.OnSpawnPlayer -= OnSpawnPlayer;
        }

        // Command callback for requests by new clients for a new player ship to be spawned
        private void OnSpawnPlayer(
            ResponseHandle<Spawner.Commands.SpawnPlayer, SpawnPlayerRequest, SpawnPlayerResponse> responseHandle)
        {
            var request = responseHandle.Request;
            var playerEntityTemplate = PlayerShipEntityTemplate.GeneratePlayerShipEntityTemplate(responseHandle.CallerInfo.CallerWorkerId,
                 request.initialPosition);
            SpatialOS.Commands.CreateEntity(SpawnerWriter, "PlayerShip", playerEntityTemplate, result =>
            {
                if (result.StatusCode != StatusCode.Success)
                {
                    Debug.LogError("PlayerSpawnManager failed to create entity: " + result.ErrorMessage);
                    return;
                }
                var createdEntityId = result.Response.Value;
                Debug.Log("PlayerSpawnManager created player entity with entity ID: " + createdEntityId);

                // Acknowledge command receipt and provide client with ID for newly created entity
                responseHandle.Respond(new SpawnPlayerResponse(createdEntityId));
            });
        }
    }
}