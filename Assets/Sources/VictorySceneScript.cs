using UnityEngine;

namespace LDJAM45
{
    public class VictorySceneScript : MonoBehaviour
    {
        public Transform boat;
        float t = 0;

        public float boatSpeed = 0.5f;
        public float boatShrink = 0.01f;

        void Update()
        {
            t += Time.deltaTime * boatShrink;
            boat.Translate(Vector2.right * Time.deltaTime * boatSpeed, Space.World);
            boat.localScale = Vector2.Lerp(Vector2.one * 0.207f, Vector2.zero, t);
        }
    }
}