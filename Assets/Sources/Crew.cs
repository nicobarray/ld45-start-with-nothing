using System;
using System.Collections;
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

        public AudioClip[] onClickAudio;
        public AudioClip onRecruitFisherman;
        public AudioClip onRecruitBuilder;
        public AudioClip onRecruitHunter;
        public AudioClip[] onBreakfast;
        public AudioClip[] onStarve;

        [Header("References")]
        public Transform jobMenu;
        public SpriteRenderer spriteRenderer;
        public AudioSource speaker;

        void OnEnable()
        {
            GameManager.instance.onClickOutside.AddListener(Unselect);
            jobMenu.gameObject.SetActive(false);
            Pubsub.instance.On(EventName.PeriodUpdate, OnPeriodUpdate);
        }

        void OnDisable()
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.onClickOutside.RemoveListener(Unselect);
            }

            Pubsub.instance.Off(EventName.PeriodUpdate, OnPeriodUpdate);
        }

        IEnumerator DelaySpeak(float seconds, Action callback)
        {
            float realDelay = UnityEngine.Random.value * seconds;
            yield return new WaitForSeconds(realDelay);
            callback();
        }

        void Speak(AudioClip clip, bool delay)
        {
            if (delay)
            {
                StartCoroutine(DelaySpeak(1, () =>
                          {
                              speaker.clip = clip;
                              speaker.Play();
                          }));
            }
            else
            {
                speaker.clip = clip;
                speaker.Play();
            }

        }

        void Speak(AudioClip[] clips, bool delay)
        {
            if (delay)
            {
                StartCoroutine(DelaySpeak(1, () =>
                          {
                              speaker.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
                              speaker.Play();
                          }));
            }
            else
            {
                speaker.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
                speaker.Play();
            }

        }

        void OnPeriodUpdate(object args)
        {
            DayPeriod period = (DayPeriod)args;

            if (job != JobType.WANDERER)
            {
                if (period == DayPeriod.DAWN)
                {
                    if (GameManager.instance.fishCount > 0)
                    {
                        GameManager.instance.fishCount--;
                        Speak(onBreakfast, true);
                    }
                    else
                    {
                        Wander(GameManager.instance.crewCamps[UnityEngine.Random.Range(0, GameManager.instance.crewCamps.Count)]);
                        Speak(onStarve, true);
                        return;
                    }

                    if (job == JobType.WARRIOR)
                    {
                        Idle(GameManager.instance.camp);
                    }
                    else
                    {
                        Work(GameManager.instance.camp);
                    }
                }
                else if (period == DayPeriod.DUSK)
                {
                    if (job == JobType.WARRIOR)
                    {
                        Work(GameManager.instance.camp);
                    }
                    else
                    {
                        Idle(GameManager.instance.camp);
                    }
                }
            }
        }

        public void Wander(Transform around)
        {
            job = JobType.WANDERER;
            UpdateSprite(true, false);
            Idle(around);
        }

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
            stateHandler = new WorkState(transform, job, UpdateSprite, (origin, destination) =>
            {
                GameObject arrowGameObject = Instantiate(arrowPrefab.gameObject, origin, Quaternion.identity);
                arrowGameObject.GetComponent<Arrow>().SetTarget(destination, 25);
                Destroy(arrowGameObject, 10);
            });
            lastTargetAround = around;
        }

        private void UpdateSprite(bool end, bool flipX)
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
        }

        public void RecruitBuilder()
        {
            if (Recruit(JobType.BUILDER))
            {
                Speak(onRecruitBuilder, false);
            }
        }

        public void RecruitWarrior()
        {
            if (Recruit(JobType.WARRIOR))
            {
                Speak(onRecruitHunter, false);
            }
        }

        public void RecruitFisherman()
        {
            if (Recruit(JobType.FISHERMAN))
            {
                Speak(onRecruitFisherman, false);
            }
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

            if (job == JobType.WARRIOR && GameManager.instance.period == DayPeriod.NIGHT
                || job != JobType.WARRIOR && GameManager.instance.period != DayPeriod.NIGHT)
            {
                Work(GameManager.instance.camp);
            }
            else
            {
                Idle(GameManager.instance.camp);
            }

            return true;
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

            jobMenu.gameObject.SetActive(true);

            Speak(onClickAudio, false);
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
                return;
            }

            if (state == CrewState.IDLE || state == CrewState.WORK)
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