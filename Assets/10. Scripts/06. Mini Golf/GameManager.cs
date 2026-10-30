using Jay;
using UnityEngine;

namespace MiniGolf.KSG
{
    public class GameManager : SingletonBase<GameManager>
    {
        public GolfBall ball;
        public float restartDistance;
        
        public Transform[] startPoints;
        public GameObject EndUI;
        public int maxLevel;
        public int level = 1;

        protected override void OnInitialize()
        {
            ball = GameObject.FindWithTag("Ball").GetComponent<GolfBall>();
        }
        
        private void Update()
        {
            if (ball.transform.position.y < restartDistance)
            {
                ball.ReturnToLastShotPosition();
            }
        }

        public void NextLevel()
        {
            ball.PlaceAt(startPoints[level].position);
        }
    }
}