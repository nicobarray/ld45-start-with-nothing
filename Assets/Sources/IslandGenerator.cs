using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDJAM45
{
    enum IslandSlab
    {
        BEACH,
        PLAIN,
        FOREST,
        CAMP
    }

    public class IslandGenerator : MonoBehaviour
    {
        [Header("Prefabs")]
        public GameObject beachPrefab;
        public GameObject plainPrefab;
        public GameObject forestPrefab;

        public GameObject campPrefab;

        [Header("References")]
        public Transform slabParent;
        public Transform islandObjectsParent;

        private struct Slab
        {
            public GameObject gameObject;
            public IslandSlab type;
        }

        private Slab[] islandSlabs = new Slab[7];

        void Start()
        {
            for (int i = 0; i < islandSlabs.Length; ++i)
            {
                IslandSlab type = IslandSlab.PLAIN;
                GameObject slabPrefab = plainPrefab;
                if (i == islandSlabs.Length / 2)
                {
                    type = IslandSlab.CAMP;
                    slabPrefab = plainPrefab;
                }
                else if (i == 0 || i == islandSlabs.Length - 1)
                {
                    type = IslandSlab.BEACH;
                    slabPrefab = beachPrefab;
                }
                else if (UnityEngine.Random.value < 0.5)
                {
                    type = IslandSlab.FOREST;
                    slabPrefab = forestPrefab;
                }

                GameObject slabGameObject = Instantiate(slabPrefab, slabParent);
                slabGameObject.name = "Slab_" + i + "_" + type.ToString();
                // ? Each slab is 20 unit of width.
                slabGameObject.transform.position = new Vector3((i - islandSlabs.Length / 2) * 20, 0);

                islandSlabs[i] = new Slab
                {
                    gameObject = slabGameObject,
                    type = type
                };

                if (type == IslandSlab.CAMP)
                {
                    GameObject campGameObject = Instantiate(campPrefab, islandObjectsParent.transform);
                    campGameObject.transform.position = slabGameObject.transform.position.Vec2() + Vector2.up * Utils.GROUND_HEIGHT;
                }
            }
        }
    }
}