using Improbable.Entity.Component;
using Improbable.Ship;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Pirates.Behaviours
{
    // Enable this MonoBehaviour on FSim (server-side) workers only
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class TrackScore : MonoBehaviour
	{
		/*
         * An entity with this MonoBehaviour will only be enabled for the single FSim worker
         * which has write access for its Score component.
         */
		[Require] private Score.Writer ScoreWriter;

		void OnEnable()
		{
			// Register command callback
			ScoreWriter.CommandReceiver.OnAwardPoints += OnAwardPoints;
		}

		private void OnDisable()
		{
			// Deregister command callbacks
			ScoreWriter.CommandReceiver.OnAwardPoints -= OnAwardPoints;
		}

		// Command callback for handling points awarded by other entities when they sink
		private void OnAwardPoints(ResponseHandle<Score.Commands.AwardPoints, AwardPoints, AwardResponse> responseHandle)
		{
			int newScore = ScoreWriter.Data.numberOfPoints + (int)responseHandle.Request.amount;
			ScoreWriter.Send(new Score.Update().SetNumberOfPoints(newScore));
			// Acknowledge command receipt
			responseHandle.Respond(new AwardResponse());
		}
	}
}