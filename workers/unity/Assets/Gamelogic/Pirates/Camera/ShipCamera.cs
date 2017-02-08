using Assets.Gamelogic.Pirates.Behaviours;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Pirates.Camera
{
    // Enable this MonoBehaviour on client workers only
    [WorkerType(WorkerPlatform.UnityClient)]
    public class ShipCamera : MonoBehaviour
    {
        public ShipController Controller;
        public AnimationCurve Distance;
        public AnimationCurve Angle;

        private Transform ourTransform;

        void Start ()
        {
            ourTransform = transform;
        }

        void Update ()
        {
            if (Controller != null)
            {
                var speed = Controller.currentSpeed;
                var rot = Quaternion.Euler(Angle.Evaluate(speed), 0f, 0f);
                ourTransform.localPosition = rot * Vector3.back * Distance.Evaluate(speed);
                ourTransform.localRotation = rot;
            }
        }
    }
}