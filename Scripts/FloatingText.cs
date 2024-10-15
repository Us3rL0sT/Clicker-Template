using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Пример префаба для FloatingText
// Убедитесь, что он содержит компонент TMP_Text
public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float fadeDuration = 1f;
    private TMP_Text textMesh;
    private Color originalColor;

    private void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
        originalColor = textMesh.color;
    }

    public void Initialize(string text, Vector3 position)
    {
        transform.position = position;
        textMesh.text = text;
        StartCoroutine(FadeAndMove());
    }

    private IEnumerator FadeAndMove()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}

