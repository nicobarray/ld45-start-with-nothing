using UnityEngine;

namespace LDJAM45
{
    public class FoodNotif : MonoBehaviour
    {
        float t = 0;
        void Update()
        {
            t += Time.deltaTime;
            transform.Translate(Vector2.down * Time.deltaTime);
            if (t > 2)
            {
                Destroy(gameObject);
            }
        }
    }
}