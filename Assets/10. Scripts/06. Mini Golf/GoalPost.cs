using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MiniGolf.KSG
{
    public class GoalPost : MonoBehaviour
    {
        [SerializeField] private GolfBall ball;
        [SerializeField] private float detectionRadius = 4f; // 공을 감지할 거리
        [SerializeField] private float fleeDistance = 3f; // 한 번에 도망갈 거리
        [SerializeField] private float extraSpeed = 4f; // 공 속도보다 얼마나 더 빠르게 이동할지
        [SerializeField] private float minFleeSpeed = 10f; // 공이 천천히 올 때의 최소 도망 속도
        [SerializeField] private float goalInRadius = 0.8f; // 뒤에서 접근 시 골인되는 거리
        [SerializeField, Range(-1f, 1f)] private float goalAllowDot;
        [SerializeField] private int maxRunCount = 3;
        [SerializeField] private bool isMove = false;
        [SerializeField] LayerMask wallLayer;

        private Transform ballTransform;
        private Rigidbody ballRb;
        private Vector3 currentPosition;
        private Vector3 targetPosition;
        private bool isFleeing = false;
        private bool isIn = false;
        private float currentFleeSpeed;
        private int runCount = 0;

        private void Start()
        {
            ball = GameManager.Instance.ball;
            ballTransform = ball.gameObject.transform;
            ballRb = ball.GetComponent<Rigidbody>();
            currentPosition = transform.position;
        }

        private void Update()
        {
            float distanceToBall = Vector3.Distance(transform.position, ballTransform.position);
            
            if (isMove == false)
            {
                if (distanceToBall <= goalInRadius)
                {
                    OnGoalIn();
                }
                
                return;
            }
            
            if (ballTransform == null || ballRb == null) return;

            // 1. 이미 도망치는 중이라면 목표 지점까지 이동
            if (isFleeing)
            {
                MoveToTarget();
                return;
            }
            
            if (distanceToBall <= detectionRadius && isIn == false)
            {
                CheckAndFlee();
            }
            
            if(SimpleInput.GetKeyDown(Key.R))
                transform.position = currentPosition;
        }

        private void CheckAndFlee()
        {
            // [내적 연산] 골대 중심에서 공을 향하는 방향 벡터
            Vector3 dirToBall = (ballTransform.position - transform.position).normalized;
            float dot = Vector3.Dot(transform.right, dirToBall);
            
            if (dot <= goalAllowDot)
            {
                if (runCount >= maxRunCount)
                {
                    transform.position = currentPosition;
                    runCount = 0;
                }
                else
                {
                    StartFleeing();
                }
            }
            else
            {
                float distanceToBall = Vector3.Distance(transform.position, ballTransform.position);
                if (distanceToBall <= goalInRadius)
                {
                    OnGoalIn();
                }
            }
        }

        private void StartFleeing()
        {
            isFleeing = true;

            // 공이 날아가고 있는 진행 방향 (공이 바라보는 방향)
            Vector3 moveDirection = ballRb.linearVelocity.normalized;

            if (moveDirection == Vector3.zero)
            {
                return;
            }

            // 도망갈 목표 위치 계산 (공의 진행 방향으로 일정 거리)
            targetPosition = transform.position + (moveDirection.normalized * fleeDistance);

            // 공의 현재 속도보다 무조건 빠르게 속도 설정 (공 속도 + extraSpeed)
            currentFleeSpeed = Mathf.Max(ballRb.linearVelocity.magnitude + extraSpeed, minFleeSpeed);
            
            runCount++;
        }

        private void MoveToTarget()
        {
            // 목표 위치로 빠르게 이동
            transform.position =
                Vector3.MoveTowards(transform.position, targetPosition, 
                    currentFleeSpeed * Time.deltaTime);

            // 목표 지점에 도착하면 도망 상태 해제
            if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
            {
                isFleeing = false;
            }
        }

        private void OnGoalIn()
        {
            isIn = true;

            if (GameManager.Instance.level < GameManager.Instance.maxLevel)
            {
                GameManager.Instance.NextLevel();
                GameManager.Instance.level++;
            }
            else
            {
                ball.gameObject.SetActive(false);
                GameManager.Instance.EndUI.SetActive(true);
            }
        }
    }
}