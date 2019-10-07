using UnityEngine;

namespace LDJAM45
{
    public class PlayerAnimation : ASquashAndStretch
    {
        public StateParamaters idleState;
        public StateParamaters walkState;

        protected override StateParamaters GetParams(Vector3 prevPosition)
        {
            bool isWalking = prevPosition != transform.position;
            return isWalking ? walkState : idleState;
        }
    }
}