using System;
using UnityEngine;

namespace LDJAM45
{
    public abstract class ASquashAndStretch : MonoBehaviour
    {
        bool reverse = false;

        [Serializable]
        public struct StateParamaters
        {
            public Vector2 target;
            public float squashSpeed;
            public float reverseSpeed;
        }

        // ? Do not use. Needed for GetParams return value.
        protected StateParamaters defaultState = new StateParamaters
        {
            target = Vector2.one,
            squashSpeed = 0,
            reverseSpeed = 0
        };

        public Vector3 defaultLocalScale = Vector3.one;
        float t = 0;

        Vector3 prevPosition;

        protected abstract StateParamaters GetParams(Vector3 prevPosition);

        void Update()
        {
            StateParamaters param = GetParams(prevPosition);
            prevPosition = transform.position;

            t += (reverse ? -param.reverseSpeed : param.squashSpeed) * Time.deltaTime;

            Vector2 target = param.target;

            transform.localScale = Vector3.Lerp(defaultLocalScale, target, t);

            if (reverse && t < 0)
            {
                reverse = false;
            }
            else if (!reverse && t > 1)
            {
                reverse = true;
            }
        }
    }
}