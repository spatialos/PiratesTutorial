using Improbable.Ship;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Pirates.Behaviours
{
    // Enable this MonoBehaviour on client workers only
    [WorkerType(WorkerPlatform.UnityClient)]
    public class CameraEnablerVisualizer : MonoBehaviour
    {
        /* 
         * Client will only have write-access for their own designated PlayerShip entity's ShipControls component,
         * so this MonoBehaviour will be enabled on the client's designated PlayerShip GameObject only and not on
         * the GameObject of other players' ships.
         */
        [Require] protected ShipControls.Writer ShipControlsWriter;

        public UnityEngine.Camera OurCamera;

        public void OnEnable()
        {
            OurCamera.enabled = true;
        }

        public void OnDisable()
        {
            OurCamera.enabled = false;
        }
    }
}
