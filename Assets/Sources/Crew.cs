using UnityEngine;

namespace LDJAM45
{
    public enum CrewState
    {
        CREATED,
        IDLE,
        WORK
    }

    public class Crew : MonoBehaviour
    {
        private abstract class State
        {
            public abstract void Begin();
            public abstract bool Update();
            public abstract void End();
        }
        private class IdleState : State
        {
            Transform targetAround;
            Transform myTransform;
            float speed = 1;

            Vector2 origin;
            Vector2 destination;
            float t = 0;

            bool sleep = true;
            float sleepTimer = 0;

            public IdleState(Transform myTransform, float speed, Transform targetAround)
            {
                this.myTransform = myTransform;
                this.targetAround = targetAround;
                this.speed = speed;
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
                this.destination = destination;
            }

            public override void End()
            {
                print("Idle state ends");
            }

            public override bool Update()
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

                    return false;
                }

                if (t >= 1)
                {
                    t = 0;
                    // ? Loop the IdleState for now.
                    Sleep();
                    // TODO: return true to work !
                    return false;
                }

                t += Time.deltaTime * speed;
                myTransform.position = Vector2.Lerp(origin, destination, t);

                return false;
            }
        }

        public CrewState state = CrewState.CREATED;
        private State currentState;

        public void Idle(Transform around)
        {
            if (currentState != null)
            {
                currentState.End();
            }

            currentState = new IdleState(transform, 1, around);
            state = CrewState.IDLE;
        }

        void Update()
        {
            if (state == CrewState.CREATED)
            {
                return;
            }

            if (state == CrewState.IDLE)
            {
                UpdateIdle();
            }
            else if (state == CrewState.WORK)
            {
                UpdateWork();
            }
        }

        void UpdateIdle()
        {
            if (currentState.Update())
            {
                currentState.End();
            }
        }

        void UpdateWork()
        {

        }
    }
}