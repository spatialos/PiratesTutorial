using UnityEngine;
using Improbable.Unity;
using Improbable.Unity.Visualizer;

namespace Assets.Gamelogic.Pirates.Shaders
{
    // Enable this MonoBehaviour on client workers only
    [WorkerType(WorkerPlatform.UnityClient)]
    public class ReapplyShaders : MonoBehaviour
    {
        // This script fixes a Unity bug that prevents shaders from correctly being visualised on Macs.

        private Renderer[] renderers;
        private Material[] materials;
        private string[] shaders;

        private void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>();
        }

        private void Start()
        {
            foreach (var renderer in renderers)
            {
                materials = renderer.sharedMaterials;
                shaders = new string[materials.Length];

                for (int i = 0; i < materials.Length; i++)
                {
                    shaders[i] = materials[i].shader.name;
                }

                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].shader = Shader.Find(shaders[i]);
                }
            }
        }

    }
}
