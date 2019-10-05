using System;
using UnityEngine;

namespace LDJAM45
{
    public class WorkState : State
    {
        Transform myTransform;
        JobType job;

        float fishingSpotX = 0;

        Vector2 origin;
        Vector2 destination;
        Vector2 workPlace;
        float t = 0;

        bool taskDuration = false;
        float taskDurationTimer = 0;
        Action<bool> updateSprite;

        public WorkState(Transform myTransform, JobType job, Action<bool> updateSprite)
        {
            this.myTransform = myTransform;
            this.job = job;
            this.updateSprite = updateSprite;
        }

        public override void Begin()
        {
            float offset = (UnityEngine.Random.value - 0.5f) * 2 * 4;

            fishingSpotX = 1;
            if (UnityEngine.Random.value > 0.5)
            {
                fishingSpotX = -1;
            }

            fishingSpotX *= 5;
            fishingSpotX += offset;

            workPlace = new Vector2(fishingSpotX, myTransform.position.y);
            destination = workPlace;
            origin = myTransform.position;
        }

        public override void End()
        {
            updateSprite(true);
        }

        private void StartTask()
        {
            taskDuration = true;
            taskDurationTimer = 0;
            updateSprite(false);
        }

        private void Work()
        {
            // ? Do the work.
            if (job == JobType.FISHERMAN)
            {
                GameManager.instance.SpawnFish(myTransform.position);
            }

            StartTask();
        }

        public override CrewState Update()
        {
            // ? The fisherman does its job.
            if (taskDuration)
            {
                taskDurationTimer += Time.deltaTime;
                if (taskDurationTimer > 3)
                {
                    Work();
                }

                return CrewState.WORK;
            }


            // ? Go to his job.
            t += Time.deltaTime;
            float deltaX = Vector2.Distance(origin, destination);
            myTransform.position = Vector2.Lerp(origin, destination, t / deltaX);

            // ? Once the dude arrives at his job, work.
            if (t >= 1)
            {
                t = 0;
                StartTask();
                return CrewState.WORK;
            }

            return CrewState.WORK;
        }
    }

}