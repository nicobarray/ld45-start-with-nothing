using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDJAM45
{
    public class MusicManager : MonoBehaviour
    {
        public AudioClip music;
        public AudioSource speaker;

        bool isPlaying = true;

        public float fade = 0.25f;
        public float maxVolume = 0.5f;

        public void Play()
        {
            isPlaying = true;
            speaker.volume = 0;
            speaker.clip = music;
            speaker.Play();
        }

        public void Stop()
        {
            isPlaying = false;
            speaker.volume = maxVolume;
            speaker.clip = music;
        }

        void OnEnable()
        {
            Pubsub.instance.On(EventName.PeriodUpdate, OnPeriodUpdate);
        }

        void OnDisable()
        {
            Pubsub.instance.Off(EventName.PeriodUpdate, OnPeriodUpdate);
        }

        void OnPeriodUpdate(object args)
        {
            DayPeriod period = (DayPeriod)args;

            if (period == DayPeriod.DAWN)
            {
                Play();
            }
            else if (period == DayPeriod.DUSK)
            {
                Stop();
            }
        }

        IEnumerator Delay(float seconds, Action callback)
        {
            yield return new WaitForSeconds(seconds);
            callback();
        }
        void Start()
        {
            speaker.volume = 0;
            StartCoroutine(Delay(3, Play));
        }

        void Update()
        {
            if (!isPlaying)
            {
                if (speaker.volume > 0.1f)
                {
                    speaker.volume -= Time.deltaTime * fade;
                }
                else
                {
                    speaker.pitch = 0.5f;
                    speaker.volume = 0.1f;
                }
            }
            else
            {
                if (speaker.volume < maxVolume)
                {
                    speaker.volume += Time.deltaTime * fade;
                }
                else
                {
                    speaker.pitch = 1;
                    speaker.volume = maxVolume;
                }
            }
        }
    }

}
