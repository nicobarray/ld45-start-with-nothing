using System;
using UnityEngine;

namespace LDJAM45
{
    public class WorkState : State
    {
        Transform myTransform;
        JobType job;

        Vector2 origin;
        Vector2 destination;
        Vector2 workPlace;
        float t = 0;

        bool taskDuration = false;
        float taskDurationTimer = 0;
        Action<bool, bool> updateSprite;
        Action<Vector2, Vector2> spawnArrow;

        float randomOffsetX = 2;

        public WorkState(Transform myTransform, JobType job, Action<bool, bool> updateSprite, Action<Vector2, Vector2> spawnArrow)
        {
            this.myTransform = myTransform;
            this.job = job;
            this.updateSprite = updateSprite;
            this.spawnArrow = spawnArrow;

            Begin();
        }

        public override void Begin()
        {
            if (job == JobType.FISHERMAN)
            {
                float fishingSpotX = 0;
                float offset = (UnityEngine.Random.value - 0.5f) * 2 * 4;

                fishingSpotX = 1;
                if (UnityEngine.Random.value > 0.5)
                {
                    fishingSpotX = -1;
                }

                fishingSpotX *= 5;
                fishingSpotX += offset;
                workPlace = new Vector2(fishingSpotX, Utils.REAL_GROUND_HEIGHT);
            }
            else if (job == JobType.BUILDER)
            {
                float offset = (UnityEngine.Random.value - 0.5f) * 2 * 2;
                workPlace = new Vector2(GameManager.instance.boatCamp.position.x + offset, myTransform.position.y);
            }
            else if (job == JobType.WARRIOR)
            {
                // ? Either stand at the left or the right hand-side of the camp.
                float offset = Mathf.Sign(UnityEngine.Random.value - 0.5f) * 2 * 8 + UnityEngine.Random.insideUnitCircle.x * 2;
                workPlace = new Vector2(GameManager.instance.camp.position.x + offset, myTransform.position.y);
            }

            destination = workPlace;
            origin = myTransform.position;
        }

        public override void End()
        {
            updateSprite(true, false);
        }

        private void StartTask()
        {
            taskDuration = true;
            taskDurationTimer = 0;
            updateSprite(false, workPlace.x - GameManager.instance.camp.transform.position.x > 0 ? false : true);
        }

        private CrewState Work()
        {
            // ? Do the work.
            if (job == JobType.FISHERMAN)
            {
                if (UnityEngine.Random.value > 0.5)
                {
                    GameManager.instance.SpawnFish(myTransform.position);
                }
            }
            else if (job == JobType.BUILDER)
            {
                GameManager.instance.boatProgress += 0.1f;
            }
            else if (job == JobType.WARRIOR)
            {
                // ? Search for in-range wolfs.
                var wolfs = GameManager.instance.FindWolfs();

                Wolf closestWolf = null;
                foreach (var wolf in wolfs)
                {
                    if (closestWolf == null
                        || Vector2.Distance(myTransform.position, wolf.transform.position) < Vector2.Distance(myTransform.position, closestWolf.transform.position))
                    {
                        closestWolf = wolf;
                    }
                }

                // ? The warrior only fire an arrow if a wolf is in sight.
                if (closestWolf != null && Vector2.Distance(closestWolf.transform.position, myTransform.position) < 10)
                {
                    updateSprite(false, workPlace.x - closestWolf.transform.position.x > 0 ? false : true);
                    spawnArrow.Invoke(
                        myTransform.position.Vec2()
                            + Vector2.up
                            + Vector2.right * (closestWolf.transform.position.x > myTransform.position.x ? 1 : -1),
                        closestWolf.transform.position.Vec2() + Vector2.right * (UnityEngine.Random.value - 0.5f) * 2 * randomOffsetX
                    );
                }
                else
                {
                    return CrewState.WORK;
                }
            }

            StartTask();

            return CrewState.WORK;
        }

        private float GetJobDuration()
        {
            if (job == JobType.FISHERMAN)
            {
                return 12;
            }

            if (job == JobType.BUILDER)
            {
                return 3;
            }

            return 3;
        }

        public override CrewState Update()
        {
            // ? The crew mate does its job.
            if (taskDuration)
            {
                taskDurationTimer += Time.deltaTime;
                if (taskDurationTimer > GetJobDuration())
                {
                    return Work();
                }

                return CrewState.WORK;
            }

            // ? Go to his job.
            float deltaX = Vector2.Distance(origin, destination);
            t += (Time.deltaTime / deltaX) * 2;
            myTransform.position = Vector2.Lerp(origin, destination, t);

            // ? Once the dude arrives at his job, work.
            if (t >= 1)
            {
                t = 0;
                StartTask();
            }

            return CrewState.WORK;
        }
    }

}