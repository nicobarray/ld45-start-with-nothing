using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

namespace LDJAM45
{
    public class Firelight : MonoBehaviour
    {
        public Light2D firelight;

        float t = 0;

        void Update()
        {
            t += Time.deltaTime;
            if (t > 0.25f)
            {
                t -= 0.25f;
                // firelight.shapeLightFalloffSize = UnityEngine.Random.value * 0.25f + 0.48f;
            }
        }
    }
}