using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDJAM45
{
    public class IslandGenerator : MonoBehaviour
    {
        [Header("Prefabs")]
        public GameObject campSlabPrefab;
        public GameObject beachPrefab;
        public GameObject plainPrefab;
        public GameObject forestPrefab;

        public GameObject campPrefab;
        // ? Camps around the island where old crew members are established.
        public GameObject crewCampPrefab;
        public GameObject crewPrefab;
        public GameObject boatConstructionSitePrefab;

        [Header("References")]
        public Transform slabParent;
        public Transform islandObjectsParent;

        public Slab[] Generate(int islandSize)
        {
            Slab[] islandSlabs = new Slab[islandSize];

            for (int i = 0; i < islandSlabs.Length; ++i)
            {
                IslandSlab type = IslandSlab.PLAIN;
                GameObject slabPrefab = plainPrefab;
                if (i == islandSlabs.Length / 2)
                {
                    type = IslandSlab.CAMP;
                    slabPrefab = campSlabPrefab;
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

                islandSlabs[i].transform = slabGameObject.transform;

                if (i == 0)
                {
                    var slabSpriteRenderers = slabGameObject.GetComponentsInChildren<SpriteRenderer>();

                    foreach (var item in slabSpriteRenderers)
                    {
                        item.flipX = true;
                    }

                    GameManager.instance.leftShore = slabGameObject.transform;
                }

                if (i == islandSlabs.Length - 1)
                {
                    GameManager.instance.rightShore = slabGameObject.transform;
                }

                if (islandSlabs[i].type == IslandSlab.CAMP)
                {
                    GameObject campGameObject = Instantiate(campPrefab, islandObjectsParent.transform);
                    campGameObject.transform.position = slabGameObject.transform.position.Vec2() + Vector2.up * 2;
                    GameManager.instance.camp = campGameObject.transform;

                    GameObject boatCampGameObject = Instantiate(boatConstructionSitePrefab, islandObjectsParent.transform);
                    boatCampGameObject.transform.position = new Vector2(slabGameObject.transform.position.Vec2().x + 10, 0);
                    GameManager.instance.boatCamp = boatCampGameObject.transform;

                    SpawnWanderer(campGameObject.transform);
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

                            islandSlabs[i].camp = campGameObject.transform;

                            int crewMembersCount = UnityEngine.Random.Range(1, 4);
                            for (int c = 0; c < crewMembersCount; c++)
                            {
                                SpawnWanderer(campGameObject.transform);
                            }
                        }
                    }
                }
            }

            return islandSlabs;
        }

        private void SpawnWanderer(Transform around)
        {
            GameObject crewMember = Instantiate(crewPrefab, islandObjectsParent.transform);
            crewMember.transform.position = Vector2.up * Utils.REAL_GROUND_HEIGHT
                                            + Vector2.right * around.position.Vec2().x
                                            + Vector2.right * (UnityEngine.Random.value - 0.5f) * 2 * 3;
            crewMember.GetComponent<Crew>().Idle(around);
        }
    }
}