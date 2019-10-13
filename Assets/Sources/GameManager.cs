using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace LDJAM45
{
    public enum IslandSlab
    {
        BEACH,
        PLAIN,
        FOREST,
        CAMP
    }

    public struct Slab
    {
        public GameObject prefab;
        public IslandSlab type;
        public Transform transform;
        public Transform camp;
        public int wanderersCount;
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        void OnEnable()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }

            instance = this;

            Pubsub.instance.On(EventName.PeriodUpdate, OnTimePeriodUpdate);
        }

        void OnDisable()
        {
            instance = null;
            Pubsub.instance.Off(EventName.PeriodUpdate, OnTimePeriodUpdate);
        }

        void OnTimePeriodUpdate(object args)
        {
            DayPeriod period = (DayPeriod)args;
            this.period = period;

            if (period == DayPeriod.DAWN)
            {
                dayCount++;

                if (dayCount > 1)
                {
                    SpawnManyWolfs(fishCount + dayCount);
                }

                foreach (var slab in map)
                {
                    if (slab.type == IslandSlab.BEACH)
                    {
                        float left = slab.transform.position.x < camp.position.x ? 3 : -3;
                        SpawnFish(slab.transform.position.Vec2() + Vector2.up * left);
                    }
                }
            }
            else if (period == DayPeriod.DUSK)
            {
                int count = FindObjectsOfType<WolfSpawn>().Length;

                if (count > 0)
                {
                    speaker.clip = wolfComing;
                    speaker.Play();
                }
            }
        }

        public bool FindSlab(Transform where, out Slab slab)
        {
            for (int i = 0; i < map.Length; i++)
            {
                var item = map[i];
                if (item.transform == where)
                {
                    slab = item;
                    return true;
                }
            }

            slab = map[0];
            return false;
        }

        public void RemoveFish()
        {
            var go = Instantiate(notifPrefab, notifParent);
            fishCount--;
        }

        private int dayCount = 1;
        public Slab[] map;

        [Header("References")]
        public UnityEvent onClickOutside;
        public TMPro.TextMeshProUGUI foodField;
        public IslandGenerator generator;
        public AudioSource speaker;
        public Transform notifParent;

        [Header("Prefabs")]
        public Fish fishPrefab;
        public Wolf wolfPrefab;
        public Arrow arrowPrefab;
        public WolfSpawn wolfSpawn;
        public GameObject notifPrefab;

        public AudioClip wolfComing;

        [Header("Gameplay")]
        public int islandSize = 12;

        [Header("Game State - Do not set in editor")]
        public int fishCount = 0;
        public float boatProgress = 0;
        public DayPeriod period = DayPeriod.DAWN;
        public Transform camp;
        public Transform boatCamp;
        public List<Transform> crewCamps = new List<Transform>();
        public Transform leftShore;
        public Transform rightShore;

        void Start()
        {
            map = generator.Generate(islandSize);

            // TODO: implement the cat.
            Instantiate(fishPrefab.gameObject, new Vector2(0, 5), Quaternion.identity);
        }

        string konamiCode = "baka";
        bool victory = false;

        void Update()
        {
            if (!victory && boatProgress >= 100)
            {
                victory = true;
                SceneManager.UnloadSceneAsync("GameScene");
                SceneManager.LoadScene("VictoryScene");
            }

            foodField.text = "x " + fishCount;

            bool clickOutside = true;
            // ? If the user is over a UI element, this is not count as "outside".
            if (EventSystem.current.IsPointerOverGameObject())
            {
                clickOutside = false;
            }

            if (konamiCode != null)
            {
                CodeKey(KeyCode.B, "b");
                CodeKey(KeyCode.A, "a");
                CodeKey(KeyCode.K, "k");

                if (konamiCode.Length <= 0)
                {
                    konamiCode = null;
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit hit;
                Ray ray = new Ray(new Vector3(mousePosition.x, mousePosition.y, -10), Vector3.forward);

                Debug.DrawRay(new Vector3(mousePosition.x, mousePosition.y, -10), Vector3.forward * 11, Color.red, 5f);
                if (Physics.Raycast(ray, out hit, 20))
                {
                    var handler = hit.collider.GetComponent<ClickHandler>();
                    if (handler != null)
                    {
                        clickOutside = false;
                        handler.onClick.Invoke();
                    }
                }

                // ? Last thing we check is if the user clicked outside any interactible element. If so, unselect everything.
                if (clickOutside)
                {
                    onClickOutside.Invoke();
                }

                if (konamiCode == null)
                {
                    if (Input.GetKey(KeyCode.W))
                    {
                        Instantiate(wolfPrefab, new Vector2(mousePosition.x, Utils.REAL_GROUND_HEIGHT), Quaternion.identity);
                    }
                    else if (Input.GetKey(KeyCode.F))
                    {
                        SpawnFish(mousePosition);
                    }
                    else if (Input.GetKey(KeyCode.R))
                    {
                        SpawnArrow(mousePosition);
                    }
                }
            }
        }

        private void SpawnManyWolfs(int population)
        {
            int maxTurn = 10;
            while (population > 0 && maxTurn > 0)
            {
                foreach (var slab in map)
                {
                    if (slab.type == IslandSlab.FOREST && slab.camp == null && Vector2.Distance(slab.transform.position, camp.position) > 15)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if (population <= 0)
                            {
                                return;
                            }

                            population--;
                            Instantiate(wolfSpawn, new Vector2(slab.transform.position.x + UnityEngine.Random.insideUnitCircle.x * 5, Utils.REAL_GROUND_HEIGHT), Quaternion.identity);
                        }
                    }
                }

                maxTurn--;
            }
        }

        private void SpawnArrow(Vector2 position)
        {
            GameObject arrowGameObject = Instantiate(arrowPrefab.gameObject, position, Quaternion.identity);
            arrowGameObject.GetComponent<Arrow>().SetTarget(position + Vector2.right * (UnityEngine.Random.value > 0.5 ? -1 : 1) * UnityEngine.Random.Range(1, 5), 5);
        }

        public void SpawnFish(Vector2 position)
        {
            GameObject fish = Instantiate(fishPrefab.gameObject, position, Quaternion.identity);
        }

        public Wolf[] FindWolfs()
        {
            return FindObjectsOfType<Wolf>();
        }

        private void CodeKey(KeyCode code, string letter)
        {
            if (Input.GetKeyDown(code) && konamiCode.StartsWith(letter))
            {
                konamiCode = konamiCode.Substring(1);
            }
        }
    }
}