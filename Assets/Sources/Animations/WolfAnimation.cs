using UnityEngine;

namespace LDJAM45
{
    public class WolfAnimation : ASquashAndStretch
    {
        public StateParamaters idleState;

        protected override StateParamaters GetParams(Vector3 prevPosition)
        {
            return idleState;
        }
    }
}