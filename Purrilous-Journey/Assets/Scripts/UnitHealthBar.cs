using UnityEngine;
using UnityEngine.UI;

public class UnitHealthBar : MonoBehaviour
{
    [SerializeField] private Image barFill;  // Assign BarFill in the Inspector
    [SerializeField] private UnitHealth parentHealth; // Assign at runtime or in Inspector

    private void Start()
    {
        if (parentHealth == null)
        {
            // If not assigned in Inspector, try to find it on the parent
            parentHealth = GetComponentInParent<UnitHealth>();
        }
    }

    private void Update()
    {
        if (parentHealth != null && barFill != null)
        {
            float fillAmount = (float) parentHealth.CurrentHealth / parentHealth.MaxHealth;
            barFill.fillAmount = fillAmount;
        }

        // Optional: Billboard the health bar so it always faces the camera
        // transform.LookAt(Camera.main.transform);
        // transform.Rotate(0, 180, 0); // If needed to flip
    }
}
