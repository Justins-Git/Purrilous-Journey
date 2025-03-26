using UnityEngine;
using TMPro;

public class FloatingGold : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float lifetime = 1.2f;
    public Vector3 floatDirection = Vector3.up;
    public TMP_Text goldText;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += floatDirection * floatSpeed * Time.deltaTime;
    }

    public void SetText(string text)
    {
        if (goldText != null)
        {
            goldText.text = text;
        }
    }
}
