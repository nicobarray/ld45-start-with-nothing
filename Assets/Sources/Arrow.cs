using UnityEngine;

namespace LDJAM45
{
    // ? On spawn we want the arrow to flight towards the target, following a parable trajectory.
    // ? Once the arrow touch its target, destroy it and call something on the target script.
    // ? If the arrow touches the ground, let it be stuck for some seconds (world persistancy and game juice !!)
    public class Arrow : MonoBehaviour
    {
        Vector2 origin;
        Vector2 destination;

        float t = 0;
        float arrowSpeed = 1;

        public void SetTarget(Vector2 wolf, float arrowSpeed, float offset = 0)
        {
            this.arrowSpeed = arrowSpeed;
            // TODO: Fire the arrow slightly in front ?
            destination = wolf;
            origin = transform.position;
        }

        void Update()
        {
            t += Time.deltaTime * 0.5f * arrowSpeed;

            float distance = Mathf.Abs(origin.x - destination.x);
            float xOrientation = destination.x - origin.x > 0 ? 1 : -1;
            float y = -t * t + distance * t;

            Vector2 nextPosition = new Vector2(origin.x + distance * xOrientation * t, origin.y + y);
            Vector2 direction = (nextPosition - transform.position.Vec2()).normalized;
            bool isFalling = direction.y < 0;

            transform.position = nextPosition;
            transform.rotation = Quaternion.Euler(0, 0, (isFalling ? -1 : 1) * Vector2.Angle(Vector2.right, direction));
        }
    }
}