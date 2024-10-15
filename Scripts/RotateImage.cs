using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RotateImage : MonoBehaviour
{
    public float swingAngle = 15f; // Максимальный угол покачивания
    public float swingSpeed = 1f;   // Скорость покачивания

    private float startAngle;

    void Start()
    {
        // Сохраняем начальный угол
        startAngle = transform.rotation.eulerAngles.z;
    }

    void Update()
    {
        // Вычисляем новый угол на основе синуса
        float newAngle = startAngle + swingAngle * Mathf.Sin(Time.time * swingSpeed);
        transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
    }
}
