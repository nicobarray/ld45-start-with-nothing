﻿using System.Collections;
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
        // ? Camps around the island where old crew members are established.
        public GameObject crewCampPrefab;
        public GameObject crewPrefab;
        public GameObject boatConstructionSitePrefab;

        [Header("Generator knobs")]
        int islandSize = 24;

        [Header("References")]
        public Transform slabParent;
        public Transform islandObjectsParent;

        private struct Slab
        {
            public GameObject prefab;
            public GameObject gameObject;
            public IslandSlab type;
        }

        private Slab[] islandSlabs;

        void Start()
        {
            islandSlabs = new Slab[islandSize];

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

                islandSlabs[i] = new Slab
                {
                    prefab = slabPrefab,
                    type = type
                };
            }

            for (int i = 0; i < islandSlabs.Length; ++i)
            {
                GameObject slabGameObject = Instantiate(islandSlabs[i].prefab, slabParent);
                slabGameObject.name = "Slab_" + i + "_" + islandSlabs[i].type.ToString();
                // ? Each slab is 20 unit of width.
                slabGameObject.transform.position = new Vector3((i - islandSlabs.Length / 2) * 20, 0);

                if (islandSlabs[i].type == IslandSlab.CAMP)
                {
                    GameObject campGameObject = Instantiate(campPrefab, islandObjectsParent.transform);
                    campGameObject.transform.position = slabGameObject.transform.position.Vec2() + Vector2.up * 2;
                    GameManager.instance.camp = campGameObject.transform;

                    GameObject boatCampGameObject = Instantiate(boatConstructionSitePrefab, islandObjectsParent.transform);
                    boatCampGameObject.transform.position = new Vector2(slabGameObject.transform.position.Vec2().x + 10, 0);
                    GameManager.instance.boatCamp = boatCampGameObject.transform;
                }

                if (islandSlabs[i].type != IslandSlab.CAMP)
                {
                    if (
                        i != 0
                        && i != islandSlabs.Length - 1
                        && islandSlabs[i - 1].type != IslandSlab.CAMP
                        && islandSlabs[i + 1].type != IslandSlab.CAMP
                    )
                    {
                        if (UnityEngine.Random.value > 0.5)
                        {
                            GameObject campGameObject = Instantiate(crewCampPrefab, islandObjectsParent.transform);
                            campGameObject.transform.position = slabGameObject.transform.position.Vec2() + Vector2.up * 2.18f;
                            GameManager.instance.crewCamps.Add(campGameObject.transform);

                            int crewMembersCount = UnityEngine.Random.Range(1, 3);
                            for (int c = 0; c < crewMembersCount; c++)
                            {
                                GameObject crewMember = Instantiate(crewPrefab, islandObjectsParent.transform);
                                crewMember.transform.position = slabGameObject.transform.position.Vec2() + Vector2.up * Utils.REAL_GROUND_HEIGHT + Vector2.right * (UnityEngine.Random.value - 0.5f) * 2 * 3;
                                crewMember.GetComponent<Crew>().Idle(campGameObject.transform);
                            }
                        }
                    }
                }
            }
        }
    }
}