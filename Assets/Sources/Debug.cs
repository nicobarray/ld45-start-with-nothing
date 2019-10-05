using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDJAM45
{
    public class Debug : MonoBehaviour
    {
        public GameObject unitPrefab;

        void Start()
        {
            int width = 1280;
            int height = 720;
            int ppu = 64;

            for (int i = 0; i < width / ppu; i++)
            {
                for (int j = 0; j < height / ppu; j++)
                {
                    GameObject gameObject = Instantiate(unitPrefab, transform);
                    print(i + ' ' + j);
                    gameObject.transform.position = new Vector2(i, j);
                    gameObject.GetComponent<SpriteRenderer>().color = new Color(256, 256, i + j / 250);
                }
            }
        }
    }
}