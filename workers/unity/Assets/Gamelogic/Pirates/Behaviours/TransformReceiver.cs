using UnityEngine;
using Improbable.General;
using Improbable.Math;
using Improbable.Unity.Visualizer;

namespace Assets.Gamelogic.Pirates.Behaviours
{
    // This MonoBehaviour will be enabled on both client and server-side workers
    public class TransformReceiver : MonoBehaviour
    {
        // Inject access to the entity's WorldTransform component
        [Require] private WorldTransform.Reader WorldTransformReader;

        void OnEnable()
        {
            // Initialize entity's gameobject transform from WorldTransform component values
            transform.position = WorldTransformReader.Data.position.ToVector3();
            transform.rotation = Quaternion.Euler(0.0f, WorldTransformReader.Data.rotation, 0.0f);

            // Register callback for when component changes
            WorldTransformReader.ComponentUpdated.Add(OnComponentUpdated);
        }

        void OnDisable()
        {
            // Deregister callback for when component changes
            WorldTransformReader.ComponentUpdated.Add(OnComponentUpdated);
        }

        // Callback for whenever one or more property of the WorldTransform component is updated
        void OnComponentUpdated(WorldTransform.Update update)
        {
            /* 
             * Only update the transform if this component is on a worker which isn't authorative over the
             * entity's WorldTransform component.
             * This synchronises the entity's local representation on the worker with that of the entity on
             * whichever worker is authoritative over its WorldTransform and is responsible for its movement.
             */
            if (!WorldTransformReader.HasAuthority)
            {
                if (update.position.HasValue)
                    transform.position = update.position.Value.ToVector3();
                if (update.rotation.HasValue)
                    transform.rotation = Quaternion.Euler(0.0f, update.rotation.Value, 0.0f);
            }
        }
    }

    public static class CoordinatesExtensions
    {
        public static Vector3 ToVector3(this Coordinates coordinates)
        {
            return new Vector3((float) coordinates.X, (float) coordinates.Y, (float) coordinates.Z);
        }
    }
}