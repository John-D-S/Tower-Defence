using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace terrain
{
    public class Terrain : MonoBehaviour
    {
        [Header("-- Terrain Mesh Settings --")]
        [SerializeField, Tooltip("How many quads will the terrain have overall")]
        private int terrainResolution;
        [SerializeField, Tooltip("How many meters long will the sides of the terrain square be?")]
        private int terrainSize;

        [Header("-- Terrain Noise Settings --")]
        private
    }
}