using UnityEngine;

namespace MiniGolf.KSG
{
    public class Laser : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<GolfBall>(out var ball))
            {
                ball.ReturnToLastShotPosition();
            }
        }
    }
}