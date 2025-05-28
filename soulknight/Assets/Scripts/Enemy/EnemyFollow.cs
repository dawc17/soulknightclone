using UnityEngine;

namespace Tenko
{
    public class EnemyFollow : MonoBehaviour
    {
        Transform player;
        public float moveSpeed = 2f; // Added a default speed
    
        
        void Start()
        {
            player = FindAnyObjectByType<PlayerLocomotionManager>().transform;
        }
        void Update()
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }
}