using System;
using UnityEngine;

namespace LDJAM45
{
    public class CrewAnimation : MonoBehaviour
    {
        public Crew crew;
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
        public StateParamaters workState;

        // ? Do not use. Needed for GetParams return value.
        private StateParamaters defaultState;

        Vector3 defaultLocalScale;
        float t = 0;

        Vector3 prevPosition;

        void Start()
        {
            defaultLocalScale = Vector3.one;
        }

        StateParamaters GetParams(CrewState state)
        {
            bool isWalking = prevPosition != transform.position;
            prevPosition = transform.position;

            return state == CrewState.IDLE
                            ? (isWalking
                                ? walkState
                                : idleState)
                            : (state == CrewState.WORK
                                ? workState
                                : defaultState);
        }

        void Update()
        {
            StateParamaters param = GetParams(crew.state);

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