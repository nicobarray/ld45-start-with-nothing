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

    public enum JobType
    {
        WANDERER,
        FISHERMAN,
        WARRIOR,
        BUILDER
    }

    public class Crew : MonoBehaviour
    {
        private CrewState prevState = CrewState.CREATED;
        public CrewState state = CrewState.CREATED;

        public JobType job = JobType.WANDERER;
        private State stateHandler;

        private bool isAlly = false;
        private Transform lastTargetAround;

        public Arrow arrowPrefab;

        [Header("Assets")]
        public Sprite wandererSprite;
        public Sprite fishermanSprite;
        public Sprite builderSprite;
        public Sprite warriorSprite;

        public Sprite fishermanWorkingSprite;
        public Sprite builderWorkingSprite;
        public Sprite warriorWorkingSprite;

        [Header("References")]
        public Transform jobMenu;
        public SpriteRenderer spriteRenderer;

        public void Idle(Transform around)
        {
            if (stateHandler != null)
            {
                stateHandler.End();
            }

            // ? We want to force the Idle state here.
            prevState = CrewState.IDLE;
            state = CrewState.IDLE;

            stateHandler = new IdleState(transform, around);
            lastTargetAround = around;
        }

        public void Work(Transform around)
        {
            if (stateHandler != null)
            {
                stateHandler.End();
            }

            // ? We want to force the Idle state here.
            state = CrewState.WORK;
            stateHandler = new WorkState(transform, job, (end, flipX) =>
            {
                if (job == JobType.FISHERMAN)
                {
                    spriteRenderer.sprite = end ? fishermanSprite : fishermanWorkingSprite;
                }
                else if (job == JobType.BUILDER)
                {
                    spriteRenderer.sprite = end ? builderSprite : builderWorkingSprite;
                }
                else if (job == JobType.WARRIOR)
                {
                    spriteRenderer.sprite = end ? warriorSprite : warriorWorkingSprite;
                }
                else
                {
                    spriteRenderer.sprite = wandererSprite;
                }

                spriteRenderer.flipX = flipX;
            }, (origin, destination) =>
            {
                GameObject arrowGameObject = Instantiate(arrowPrefab.gameObject, origin, Quaternion.identity);
                arrowGameObject.GetComponent<Arrow>().SetTarget(destination, 25);
                Destroy(arrowGameObject, 10);
            });
            lastTargetAround = around;
        }

        public void RecruitBuilder()
        {
            Recruit(JobType.BUILDER);
        }

        public void RecruitWarrior()
        {
            Recruit(JobType.WARRIOR);
        }

        public void RecruitFisherman()
        {
            Recruit(JobType.FISHERMAN);
        }

        public bool Recruit(JobType job)
        {
            if (this.job != JobType.WANDERER)
            {
                return false;
            }

            if (GameManager.instance.fishCount == 0)
            {
                return false;
            }

            GameManager.instance.fishCount--;
            this.job = job;
            spriteRenderer.sprite = GetSprite();

            Unselect();

            Idle(GameManager.instance.camp);
            return true;
        }

        void OnEnable()
        {
            GameManager.instance.onClickOutside.AddListener(Unselect);
            jobMenu.gameObject.SetActive(false);
        }

        void OnDisable()
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.onClickOutside.RemoveListener(Unselect);
            }
        }

        public void Select()
        {
            Player p = FindObjectOfType<Player>();
            if (Vector2.Distance(p.transform.position, transform.position) > 10)
            {
                return;
            }

            if (job != JobType.WANDERER)
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
            // ? Only do this if the crew mate is selected.
            if (state != CrewState.SELECTED)
            {
                return;
            }

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
            else if (state == CrewState.IDLE || state == CrewState.WORK)
            {
                CrewState nextState = stateHandler.Update();

                if (job == JobType.WANDERER)
                {
                    // ? A wanderer does not have a job.
                    return;
                }

                if (nextState != state)
                {
                    Debug.Log(state + " -> " + nextState);

                    prevState = state;
                    state = nextState;
                    stateHandler.End();

                    if (nextState == CrewState.IDLE)
                    {
                        Idle(lastTargetAround);
                    }
                    else if (nextState == CrewState.WORK)
                    {
                        Work(lastTargetAround);
                    }

                    stateHandler.Begin();
                }
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

        Sprite GetSprite()
        {
            switch (job)
            {
                case JobType.WANDERER:
                    return wandererSprite;
                case JobType.BUILDER:
                    return builderSprite;
                case JobType.FISHERMAN:
                    return fishermanSprite;
                case JobType.WARRIOR:
                    return warriorSprite;
                default:
                    return wandererSprite;
            }
        }
    }
}