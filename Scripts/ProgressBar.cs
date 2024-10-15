using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class ProgressBar : MonoBehaviour
{

    public decimal maximum = 150;
    public static decimal level = 0;
    public static decimal current = 0;
    public Slider slider;


    // Start is called before the first frame update
    void Start()
    {
        level = LoadDecimalFromPrefs("level", 0);
        current = LoadDecimalFromPrefs("current", 0);
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentFill();
    }

    void GetCurrentFill()
    {

        float fillAmount = (float)current / (float)maximum;
        if (fillAmount >= 10)
        {
            level++;
            current = 0;
            SaveDecimalToPrefs("level", level);
            SaveDecimalToPrefs("current", 0);
        }
        slider.value = fillAmount;
    }

    public void SetHealth(decimal current)
    {
        slider.value = (float)current;
    }

    public decimal LoadDecimalFromPrefs(string key, decimal defaultValue)
    {
        string stringValue = PlayerPrefs.GetString(key, defaultValue.ToString());
        if (decimal.TryParse(stringValue, out decimal result))
        {
            return result;
        }
        return defaultValue;
    }
    public void SaveDecimalToPrefs(string key, decimal value)
    {
        PlayerPrefs.SetString(key, value.ToString());
    }
}
