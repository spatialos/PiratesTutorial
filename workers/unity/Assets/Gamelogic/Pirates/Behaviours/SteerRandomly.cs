using UnityEngine;
using Improbable.Ship;
using Improbable.Unity;
using Improbable.Unity.Visualizer;

namespace Assets.Gamelogic.Pirates.Behaviours
{
    // Enable this MonoBehaviour on FSim (server-side) workers only
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class SteerRandomly : MonoBehaviour
	{
		/*
         * An entity with this MonoBehaviour will only be enabled for the single FSim worker
         * which has write-access for its WorldTransform component.
         */
		[Require] private ShipControls.Writer ShipControlsWriter;

		private void OnEnable()
		{
			// Change steering decisions every five seconds
			InvokeRepeating("RandomizeSteering", 0, 5.0f);
		}

		private void OnDisable()
		{
			CancelInvoke("RandomizeSteering");
		}

		private void RandomizeSteering()
		{
			ShipControlsWriter.Send(new ShipControls.Update()
				.SetTargetSpeed(Random.value*0.75f)
				.SetTargetSteering((Random.value*30.0f)-15.0f));
		}
	}
}