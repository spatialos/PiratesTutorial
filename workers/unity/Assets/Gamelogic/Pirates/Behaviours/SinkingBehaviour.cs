using Improbable.Ship;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Pirates.Behaviours
{
    // Enable this MonoBehaviour on client workers only
    [WorkerType(WorkerPlatform.UnityClient)]
    public class SinkingBehaviour : MonoBehaviour
	{
		// Inject access to the entity's Health component
		[Require] private Health.Reader HealthReader;

		public Animation SinkingAnimation;
		private bool alreadySunk = false;

		private void OnEnable()
		{
			InitializeSinkingAnimation();
			// Register callback for when components change
			HealthReader.ComponentUpdated += OnComponentUpdated;
		}

		private void OnDisable()
		{
			// Deregister callback for when components change
			HealthReader.ComponentUpdated -= OnComponentUpdated;
		}

		private void InitializeSinkingAnimation()
		{
			/*
             * SinkingAnimation is triggered when the ship is first killed. But a worker which checks out
             * the entity after this time (for example, a client connecting to the game later) 
             * must not visualise the ship as still alive.
             * 
             * Therefore, on checkout, any sunk ships jump to the end of the sinking animation.
             */
			if (HealthReader.Data.currentHealth <= 0)
			{
				foreach (AnimationState state in SinkingAnimation)
				{
					state.normalizedTime = 1;
				}
				SinkingAnimation.Play();
				alreadySunk = true;
			}
		}

		// Callback for whenever one or more property of the Health component is updated
		private void OnComponentUpdated(Health.Update update)
		{
			// Update objects only have values for fields that have been updated
			if (update.currentHealth.HasValue)
			{
				if (!alreadySunk && update.currentHealth.Value <= 0)
				{
					SinkingAnimation.Play();
					alreadySunk = true;
				}
			}
		}
	}
}