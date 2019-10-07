using UnityEngine;

namespace LDJAM45
{
    public class BoatConstructionSite : MonoBehaviour
    {
        public Transform sprite;
        public TMPro.TextMeshProUGUI progressField;

        public Vector2 origin;
        public Vector2 destintion;

        public float baseBuildingHeight = 2.18f;

        void Start()
        {
            origin = sprite.localPosition;
            destintion = new Vector2(0, baseBuildingHeight);
        }

        void Update()
        {
            float progress = GameManager.instance.boatProgress;
            progressField.text = progress.ToString().Substring(0, Mathf.Min(progress.ToString().Length, 4)) + "%";

            if (GameManager.instance.boatProgress >= 100)
            {
                sprite.rotation = Quaternion.Euler(0, 0, 90);
                sprite.localPosition = Vector2.up * 2;
                return;
            }

            float dx = (GameManager.instance.boatProgress / 100);
            sprite.localPosition = Vector2.Lerp(origin, destintion, dx);
        }
    }
}