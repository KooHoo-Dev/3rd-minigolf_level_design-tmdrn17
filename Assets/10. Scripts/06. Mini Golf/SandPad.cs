using UnityEngine;

namespace MiniGolf.KSG
{
    public class SandPad : MonoBehaviour
    {
        [SerializeField] private float sandDamping;
        [SerializeField] private float originBallDamping;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ball"))
            {
                originBallDamping = other.GetComponent<GolfBall>().rBody.linearDamping;
                Debug.Log(other.GetComponent<GolfBall>().rBody.linearDamping);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Ball"))
            {
                other.GetComponent<GolfBall>().rBody.linearDamping = sandDamping;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            other.GetComponent<GolfBall>().rBody.linearDamping = originBallDamping;
        }
    }   
}
