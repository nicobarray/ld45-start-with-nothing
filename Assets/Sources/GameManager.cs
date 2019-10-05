using UnityEngine;

namespace LDJAM45
{
    public class GameManager : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI foodField;

        public int food = 0;

        void Update()
        {
            foodField.text = food + " fish" + (food > 1 ? "es" : "");
        }
    }
}