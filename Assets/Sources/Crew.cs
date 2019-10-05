using UnityEngine;

namespace LDJAM45
{
    public enum CrewState
    {
        CREATED,
        SELECTED,
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
                destination.y = origin.y;
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

        private CrewState prevState = CrewState.CREATED;
        public CrewState state = CrewState.CREATED;
        private State currentState;
        private bool isAlly = false;

        [Header("References")]
        public Transform jobMenu;

        public void Idle(Transform around)
        {
            if (currentState != null)
            {
                currentState.End();
            }

            // ? We want to force the Idle state here.
            prevState = CrewState.IDLE;
            state = CrewState.IDLE;

            currentState = new IdleState(transform, 1, around);
        }

        void OnEnable()
        {
            GameManager.instance.onClickOutside.AddListener(Unselect);
            jobMenu.gameObject.SetActive(false);
        }

        void OnDisable()
        {
            GameManager.instance.onClickOutside.RemoveListener(Unselect);
        }

        public void Select()
        {
            Player p = FindObjectOfType<Player>();
            if (Vector2.Distance(p.transform.position, transform.position) > 10)
            {
                return;
            }

            // ? We selecting a crew mate, unselect everything else.
            GameManager.instance.onClickOutside.Invoke();

            prevState = state;
            state = CrewState.SELECTED;

            if (!isAlly)
            {
                jobMenu.gameObject.SetActive(true);
            }
        }

        public void Unselect()
        {
            jobMenu.gameObject.SetActive(false);
            state = prevState;
        }

        void Update()
        {
            if (state == CrewState.CREATED)
            {
                return;
            }

            if (state == CrewState.SELECTED)
            {
                UpdateSelected();
            }
            else if (state == CrewState.IDLE)
            {
                UpdateIdle();
            }
            else if (state == CrewState.WORK)
            {
                UpdateWork();
            }
        }

        void UpdateSelected()
        {
            Player p = FindObjectOfType<Player>();
            if (Vector2.Distance(p.transform.position, transform.position) > 10)
            {
                Unselect();
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