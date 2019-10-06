using UnityEngine;

namespace LDJAM45
{
    class IdleState : State
    {
        Transform targetAround;
        Transform myTransform;

        Vector2 origin;
        Vector2 destination;
        float t = 0;

        bool sleep = true;
        float sleepTimer = 0;

        public IdleState(Transform myTransform, Transform targetAround)
        {
            this.myTransform = myTransform;
            this.targetAround = targetAround;
        }

        private void Sleep()
        {
            sleep = true;
            sleepTimer = 0;
        }

        public override void Begin()
        {
            origin = myTransform.position;

            // ? Move between 0 and 4 around the target.
            Vector2 destination = targetAround.position.Vec2();
            destination.x += (UnityEngine.Random.value - 0.5f) * 4;
            destination.y = origin.y;

            this.destination = destination;
        }

        public override void End()
        {

        }

        public override CrewState Update()
        {
            if (sleep)
            {
                sleepTimer += Time.deltaTime;
                if (sleepTimer > 5)
                {
                    sleepTimer -= 5;
                    sleep = false;
                    Begin();
                }

                return CrewState.IDLE;
            }

            if (t >= 1)
            {
                t = 0;
                // ? Loop the IdleState for now.
                Sleep();

                if (UnityEngine.Random.value > 0.2)
                    return CrewState.WORK;
                return CrewState.IDLE;
            }

            float deltaX = Vector2.Distance(origin, destination);
            t += Time.deltaTime / deltaX;
            myTransform.position = Vector2.Lerp(origin, destination, t);
            return CrewState.IDLE;
        }
    }

}