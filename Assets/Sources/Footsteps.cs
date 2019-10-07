using UnityEngine;

namespace LDJAM45
{
    public class Footsteps : MonoBehaviour
    {
        public AudioSource speaker;
        public AudioClip footstepsClip;

        float footstepsTimer = 0;
        Vector2 prevPosition;
        bool isMoving = false;

        void Start()
        {
            speaker.clip = footstepsClip;
        }

        void Update()
        {
            isMoving = prevPosition.x != transform.position.x;
            prevPosition = transform.position;

            if (isMoving)
            {
                speaker.volume = 0.5f;
                footstepsTimer += Time.deltaTime;
                if (footstepsTimer > 0.25f)
                {
                    footstepsTimer = 0;
                    speaker.Play();
                }
            }
            else
            {
                footstepsTimer = 0;
            }
        }
    }
}