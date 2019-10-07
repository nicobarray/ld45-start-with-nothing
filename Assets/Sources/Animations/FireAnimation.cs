using UnityEngine;

namespace LDJAM45
{
    public class FireAnimation : ASquashAndStretch
    {
        public StateParamaters fireState;

        protected override StateParamaters GetParams(Vector3 prevPosition)
        {
            return fireState;
        }
    }
}