using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDJAM45
{
    public class FishStorage : MonoBehaviour
    {
        [Header("References")]
        public SpriteRenderer sr;
        public Sprite emptySprite;
        public Sprite fishSprite;

        public bool empty = true;

        void Update()
        {
            if (empty && GameManager.instance.fishCount > 0)
            {
                empty = false;
                sr.sprite = fishSprite;
            }
            else if (!empty && GameManager.instance.fishCount == 0)
            {
                empty = true;
                sr.sprite = emptySprite;
            }
        }
    }
}