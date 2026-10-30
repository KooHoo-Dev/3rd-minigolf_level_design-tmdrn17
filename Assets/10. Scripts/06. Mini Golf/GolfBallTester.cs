using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MiniGolf.KSG
{
    public class GolfBallTester : MonoBehaviour
    {
        public GolfBall ball;
        public Vector3 startPosition;
        
        private void Update()
        {
            if (SimpleInput.GetKeyDown(Key.Digit1))
            {
                ball.ReturnToLastShotPosition();
            }
            if (SimpleInput.GetKeyDown(Key.Digit2))
            {
                ball.PlaceAt(startPosition);
            }
        }
    }
}