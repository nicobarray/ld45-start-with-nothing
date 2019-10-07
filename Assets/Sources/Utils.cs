using System;
using System.Collections;
using UnityEngine;

namespace LDJAM45
{
    public static class Utils
    {
        public const float GROUND_HEIGHT = 2.5225f;
        public const float REAL_GROUND_HEIGHT = 1.72f;

        public static Vector2 Vec2(this Vector3 vector)
        {
            return vector;
        }

        public static IEnumerator Delay(float seconds, Action callback)
        {
            yield return new WaitForSeconds(seconds);
            callback();
        }

    }
}