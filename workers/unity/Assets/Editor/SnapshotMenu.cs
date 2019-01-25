using Assets.Gamelogic.Core;
using Assets.Gamelogic.EntityTemplates;
using Improbable;
using Improbable.Worker;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using Random = UnityEngine.Random;
using UnityEngine;

namespace Assets.Editor
{
    public class SnapshotMenu : MonoBehaviour
    {
        [MenuItem("Improbable/Snapshots/Generate Default Snapshot")]
        private static void GenerateDefaultSnapshot()
        {
            var snapshotEntities = new Dictionary<EntityId, Entity>();
            var currentEntityId = 1;

            snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreatePlayerCreatorTemplate());
            PopulateSnapshotWithIslandTerrainEntities(ref snapshotEntities, ref currentEntityId);
            PopulateSnapshotWithSmallFishGroups(ref snapshotEntities, ref currentEntityId);
            PopulateSnapshotWithLargeFish(ref snapshotEntities, ref currentEntityId);

            SaveSnapshot(snapshotEntities);
        }

        // Create an island entity for each island prefab at its given world coordinates
        public static void PopulateSnapshotWithIslandTerrainEntities(ref Dictionary<EntityId, Entity> snapshotEntities, ref int nextAvailableId)
        {
            foreach(var item in SimulationSettings.IslandsEntityPlacements)
            {
                snapshotEntities.Add(new EntityId(nextAvailableId++),
                    EntityTemplateFactory.GenerateIslandEntityTemplate(item.Value, item.Key));
            }
        }

        // Create shoal of small fish entities around given coordinates
        public static void PopulateSnapshotWithSmallFishGroups(ref Dictionary<EntityId, Entity> snapshotEntities, ref int nextAvailableId)
        {
            foreach (var location in SimulationSettings.FishShoalStartingLocations)
            {
                for (var i = 0; i < SimulationSettings.TotalFishInShoal; i++)
                {
                    var nextFishCoodinates = new Vector3(location.x + Random.Range(-SimulationSettings.ShoalRadius, SimulationSettings.ShoalRadius),
                                                         location.y + Random.Range(SimulationSettings.ShoalMinDepth, SimulationSettings.ShoalMaxDepth),
                                                         location.z + Random.Range(-SimulationSettings.ShoalRadius, SimulationSettings.ShoalRadius));
                    snapshotEntities.Add(new EntityId(nextAvailableId++), EntityTemplateFactory.GenerateSmallFishTemplate(nextFishCoodinates));
                }
            }
        }

        // Create large fish entities
        public static void PopulateSnapshotWithLargeFish(ref Dictionary<EntityId, Entity> snapshotEntities, ref int nextAvailableId)
        {
            foreach (var location in SimulationSettings.LargeFishStartingLocations)
            {
                snapshotEntities.Add(new EntityId(nextAvailableId++), EntityTemplateFactory.GenerateLargeFishTemplate(location));
            }
        }

        private static void SaveSnapshot(IDictionary<EntityId, Entity> snapshotEntities)
        {
            var snapshotPath = Application.dataPath + SimulationSettings.DefaultRelativeSnapshotPath;
            File.Delete(snapshotPath);
            using (SnapshotOutputStream stream = new SnapshotOutputStream(snapshotPath))
            {
                foreach (var kvp in snapshotEntities)
                {
                    var error = stream.WriteEntity(kvp.Key, kvp.Value);
                    if (error.HasValue)
                        {
                            Debug.LogErrorFormat("Failed to generate initial world snapshot: {0}", error.Value);
                            return;
                        }
                }
            }
                Debug.LogFormat("Successfully generated initial world snapshot at {0}", snapshotPath);
        }
    }
}
