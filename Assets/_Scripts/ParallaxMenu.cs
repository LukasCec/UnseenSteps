using UnityEngine;

public class ParallaxMenu : MonoBehaviour
{
    public float scrollSpeed = 1f;  // rýchlosť posunu vrstvy
    private float spriteWidth;
    private Transform[] parts;

    private void Start()
    {
        // načítame všetky časti (Left, Mid, Right)
        int count = transform.childCount;
        parts = new Transform[count];
        for (int i = 0; i < count; i++)
            parts[i] = transform.GetChild(i);

        // predpokladáme rovnakú šírku pre všetky časti
        SpriteRenderer sr = parts[0].GetComponent<SpriteRenderer>();
        spriteWidth = sr.bounds.size.x;
    }

    private void Update()
    {
        float movement = scrollSpeed * Time.deltaTime;
        foreach (Transform part in parts)
        {
            part.position += Vector3.left * movement;
        }

        // ak niektorý sprite prejde mimo obrazovku, posunieme ho na koniec
        Transform first = parts[0];
        Transform last = parts[parts.Length - 1];

        if (first.position.x < -spriteWidth)
        {
            // posun na koniec
            first.position = new Vector3(last.position.x + spriteWidth, first.position.y, first.position.z);

            // zmena poradia v poli (aby loop fungoval ďalej)
            for (int i = 0; i < parts.Length - 1; i++)
                parts[i] = parts[i + 1];
            parts[parts.Length - 1] = first;
        }
    }
}
