using System;
using Improbable;
using Improbable.Player;
using Improbable.Ship;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using Improbable.Worker;
using UnityEngine;

namespace Assets.Gamelogic.Player
{
    // Enable this MonoBehaviour on client workers only
    [WorkerType(WorkerPlatform.UnityClient)]
    public class PlayerHeartbeatSender : MonoBehaviour
    {
        /* 
         * Client will only have write-access for their own designated PlayerShip entity's ShipControls component,
         * so this MonoBehaviour will be enabled on the client's designated PlayerShip GameObject only and not on
         * the GameObject of other players' ships.
         */
        [Require] private ShipControls.Writer ShipControlsWriter;
        [Require] private PlayerLifecycle.Reader PlayerLifecycleReader;

        // Local tracking of missed heartbeats for log message purposes
        private uint missed_heartbeats;

        private void OnEnable()
        {
            missed_heartbeats = PlayerLifecycleReader.Data.currentMissedHeartbeats;
            InvokeRepeating("SendHeartbeat", 0f, PlayerLifecycleReader.Data.playerHeartbeatInterval);

            // Register callback for when components change
            PlayerLifecycleReader.ComponentUpdated += OnComponentUpdated;
        }

        private void OnDisable()
        {
            // Deregister callback for when components change
            PlayerLifecycleReader.ComponentUpdated -= OnComponentUpdated;
        }

        // Callback for whenever one or more property of the PlayerLifecycle component is updated
        private void OnComponentUpdated(PlayerLifecycle.Update update)
        {
            // Update object will have values only for fields which have been updated
            if (update.currentMissedHeartbeats.HasValue)
            {
                // Synchronize local missed heartbeat counter with server-side counter
                missed_heartbeats = update.currentMissedHeartbeats.Value;
            }
        }

        private void SendHeartbeat()
        {
            // Issue heartbeat to indicate client is still connected, so player's entity is not deleted
            EntityId thisEntityId = gameObject.EntityId();
            // Use Commands API to self: implementation of command will occur on server-side
            // Any writer can be used when sending commmands
            SpatialOS.Commands.SendCommand(ShipControlsWriter, PlayerLifecycle.Commands.Heartbeat.Descriptor,
                                           new HeartbeatRequest(thisEntityId), thisEntityId, result =>
            {
                if (result.StatusCode != StatusCode.Success)
                {
                    missed_heartbeats++;
                    String msg = "Heartbeats recently failed: " + missed_heartbeats + ", max allowed: " + PlayerLifecycleReader.Data.maxMissedHeartbeats;
                    Debug.LogError(msg);
                }
            });
            if (missed_heartbeats >= PlayerLifecycleReader.Data.maxMissedHeartbeats)
            {
                Debug.LogWarning("Player entity is at risk of being deleted due to missed heartbeats");
            }
        }
    }
}