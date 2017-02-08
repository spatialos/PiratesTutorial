using Improbable.Ship;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Pirates.Behaviours
{
    // Enable this MonoBehaviour on client workers only
    [WorkerType(WorkerPlatform.UnityClient)]
    public class PlayerInputController : MonoBehaviour
    {
        /* 
         * Client will only have write-access for their own designated PlayerShip entity's ShipControls component,
         * so this MonoBehaviour will be enabled on the client's designated PlayerShip GameObject only and not on
         * the GameObject of other players' ships.
         */
        [Require] private ShipControls.Writer ShipControlsWriter;

        void Update ()
        {
            ShipControlsWriter.Send(new ShipControls.Update()
                .SetTargetSpeed(Mathf.Clamp01(Input.GetAxis("Vertical")))
                .SetTargetSteering(Input.GetAxis("Horizontal")));
			if (Input.GetKeyDown(KeyCode.Q))
			{
				ShipControlsWriter.Send(new ShipControls.Update().AddFireLeft(new FireLeft()));
			}
			if (Input.GetKeyDown(KeyCode.E))
			{
				ShipControlsWriter.Send(new ShipControls.Update().AddFireRight(new FireRight()));
			}

        }
    }
}
