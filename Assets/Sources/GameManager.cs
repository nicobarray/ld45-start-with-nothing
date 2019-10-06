using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace LDJAM45
{
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
        }

        void OnDisable()
        {
            instance = null;
        }

        public UnityEvent onClickOutside;
        public TMPro.TextMeshProUGUI foodField;
        public Fish fishPrefab;
        public Wolf wolfPrefab;

        [Header("Game State - Do not set in editor")]
        public int fishCount = 0;
        public float boatProgress = 0;
        public Transform camp;
        public Transform boatCamp;
        public List<Transform> crewCamps = new List<Transform>();
        public Transform leftShore;
        public Transform rightShore;

        void Start()
        {
            Instantiate(fishPrefab.gameObject, new Vector2(0, 5), Quaternion.identity);
        }

        void Update()
        {
            foodField.text = "x " + fishCount;

            bool clickOutside = true;
            // ? If the user is over a UI element, this is not count as "outside".
            if (EventSystem.current.IsPointerOverGameObject())
            {
                clickOutside = false;
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

                // ? Uncomment to spawn fish for debug.
                Instantiate(wolfPrefab.gameObject, new Vector2(mousePosition.x, Utils.REAL_GROUND_HEIGHT), Quaternion.identity);
            }
        }

        public void SpawnFish(Vector2 position)
        {
            GameObject fish = Instantiate(fishPrefab.gameObject, position, Quaternion.identity);
            Destroy(fish, 30);
        }
    }
}