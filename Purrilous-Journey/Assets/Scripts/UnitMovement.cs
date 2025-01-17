using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.0f;
    public Transform targetBase;
    public bool canMove = true;

    private void Update()
    {
        if (!canMove || targetBase == null)
        {
            return; // Don't move if disabled or no target
        }

        Vector2 direction = (targetBase.position - transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }
}
