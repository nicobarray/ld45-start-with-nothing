using System;
using UnityEngine;

namespace LDJAM45
{
    public class PlayerAnimation : MonoBehaviour
    {
        bool reverse = false;

        [Serializable]
        public struct StateParamaters
        {
            public Vector2 target;
            public float squashSpeed;
            public float reverseSpeed;
        }

        public StateParamaters idleState;
        public StateParamaters walkState;

        // ? Do not use. Needed for GetParams return value.
        private StateParamaters defaultState = new StateParamaters
        {
            target = Vector2.one,
            squashSpeed = 0,
            reverseSpeed = 0
        };

        Vector3 defaultLocalScale;
        float t = 0;

        Vector3 prevPosition;

        void Start()
        {
            defaultLocalScale = Vector3.one;
        }

        StateParamaters GetParams()
        {
            bool isWalking = prevPosition != transform.position;
            prevPosition = transform.position;

            return isWalking ? walkState : idleState;
        }

        void Update()
        {
            StateParamaters param = GetParams();

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