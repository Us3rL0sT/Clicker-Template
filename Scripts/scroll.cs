using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scroll : MonoBehaviour
{
    public ScrollRect scrollRect;
    private bool isInitialized = false;

    void Start()
    {
        // Устанавливаем начальную позицию один раз
        scrollRect.verticalNormalizedPosition = 1f; // 1f для верхней позиции
        scrollRect.horizontalNormalizedPosition = 0f;
        isInitialized = true;
    }

    void LateUpdate()
    {
        // Проверяем, установлена ли начальная позиция
        if (!isInitialized)
        {
            scrollRect.verticalNormalizedPosition = 1f;
            scrollRect.horizontalNormalizedPosition = 0f;
            isInitialized = true;
        }
    }
}
