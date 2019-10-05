using System.Collections;
using UnityEngine;

namespace LDJAM45
{
    public class SquashSprite : MonoBehaviour
    {

        private Coroutine coroutine;

        public float squashSteps = 10;
        public float stableSteps = 3;

        IEnumerator Squash()
        {
            transform.localScale = Vector2.one;

            Vector2 stable = Vector2.one;
            Vector2 squash = new Vector2(2f, 0.5f);

            for (float t = 0; t < 1; t += 1 / squashSteps)
            {
                transform.localScale = Vector2.Lerp(stable, squash, t);
                yield return null;
            }

            for (float t = 0; t < 1; t += 1 / stableSteps)
            {
                transform.localScale = Vector2.Lerp(squash, stable, t);
                yield return null;
            }

            transform.localScale = Vector2.one;
        }
    }
}