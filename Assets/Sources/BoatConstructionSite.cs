using UnityEngine;

namespace LDJAM45
{
    public class BoatConstructionSite : MonoBehaviour
    {
        public Transform sprite;

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
            float dx = (GameManager.instance.boatProgress / 100);
            sprite.localPosition = Vector2.Lerp(origin, destintion, dx);
        }
    }
}