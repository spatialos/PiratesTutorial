using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Pirates.Camera
{
    // Enable this MonoBehaviour on client workers only
    [WorkerType(WorkerPlatform.UnityClient)]
    public class CameraVisualizer : MonoBehaviour
    {
        public Transform CameraRoot;
        public float RotateSpeed = 60.0f;

        void Update()
        {
            if (Input.GetKey(KeyCode.Q))
            {
                CameraRoot.transform.Rotate(-Vector3.up * Time.deltaTime * RotateSpeed, Space.World);
            } 
            else if (Input.GetKey(KeyCode.E))
            {
                CameraRoot.transform.Rotate(Vector3.up * Time.deltaTime * RotateSpeed, Space.World);
            }
        }
    }
}
