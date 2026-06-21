using UnityEngine;
using UnityEngine.UI;

public class BoggleLine : MonoBehaviour
{
    [SerializeField] Image lineImage;

    public void SetLine(Vector2 from, Vector2 to, Color color, float thickness = 20f)
    {
        Vector2 dir = to - from;
        float distance = dir.magnitude;

        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(distance, thickness);
        rect.anchoredPosition = (from + to) / 2f;
        rect.rotation = Quaternion.FromToRotation(Vector3.right, new Vector3(dir.x, dir.y, 0));

        lineImage.color = color;
    }
}