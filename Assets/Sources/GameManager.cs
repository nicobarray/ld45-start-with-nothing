using System.Collections.Generic;
using UnityEngine;

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

        public TMPro.TextMeshProUGUI foodField;
        public Fish fishPrefab;

        [Header("Game State - Do not set in editor")]
        public int fishCount = 0;
        public Transform camp;
        public List<Transform> crewCamps = new List<Transform>();

        void Update()
        {
            foodField.text = fishCount + " fish" + (fishCount > 1 ? "es" : "");

            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                GameObject fish = Instantiate(fishPrefab.gameObject, mousePosition, Quaternion.identity);
                Destroy(fish, 5);
            }
        }
    }
}