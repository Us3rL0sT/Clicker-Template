using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    private float[] arrayTitles = {
    1f, 1f,
    3.5f, 3.5f,
    10.5f, 10.5f,
    36.75f, 36.75f,
    128.6f, 128.6f,
    450.1f, 450.1f,
    1350.3f, 1350.3f,
    4050.9f, 4050.9f,
    12152.7f, 12152.7f,
    36458.1f, 36458.1f,
    109374.3f, 109374.3f,
    328122.9f, 328122.9f,
};

    private string[] arrayDescriptions;

    private float[] arrayAdditionalInfo = {
    100f, 150f,
    300f, 450f,
    900f, 1350f,
    2700f, 4050f,
    8100f, 12150f,
    24300f, 36450f,
    72900f, 109350f,
    218700f, 328050f,
    656100f, 984150f,
    1968300f, 2952450f,
    5904900f, 8857350f,
    17714700f, 26572050f,
};


    public static int language = 0;
    public Sprite[] arraySprites;

    public GameObject button;
    public GameObject content;

    private List<GameObject> list = new List<GameObject>();
    private VerticalLayoutGroup _group;

    public TMP_Text moneyText;
    public TMP_Text incomeText;
    public TMP_Text perClickText;
    public ScrollRect scrollRect;

    private void Start()
    {
        ProgressBar.level = LoadDecimalFromPrefs("level", 0);
        MainMenu.money = LoadDecimalFromPrefs("money", 0);
        MainMenu.perClick = LoadDecimalFromPrefs("perClick", 1);
        MainMenu.income = LoadDecimalFromPrefs("income", 0);
        _group = GetComponent<VerticalLayoutGroup>();

        language = PlayerPrefs.GetInt("language", 0); // Установите значение по умолчанию на 0
        SetLanguage(language);

        setShop();
    }

    private void Update()
    {
        language = PlayerPrefs.GetInt("language", 0); // Установите значение по умолчанию на 0
        SetLanguage(language);
        MainMenu.money = LoadDecimalFromPrefs("money", 0);
        MainMenu.perClick = LoadDecimalFromPrefs("perClick", 1);
        MainMenu.income = LoadDecimalFromPrefs("income", 0);

        UpdateMoneyText();

        for (int i = 0; i < list.Count; i++)
        {
            GameObject buttonObj = list[i];
            Button btn = buttonObj.GetComponent<Button>();

            if (btn != null)
            {
                decimal cost = (decimal)arrayAdditionalInfo[i];
                int id = i; // В данном случае id — это индекс элемента в списке

                // Проверка, если денег недостаточно или уровень меньше id объекта
                if (MainMenu.money < cost || ProgressBar.level < id)
                {
                    // Затемняем и отключаем кнопку
                    ColorBlock colors = btn.colors;
                    colors.normalColor = new Color(0.5f, 0.5f, 0.5f); // Общий цвет затемнения
                    btn.colors = colors;
                    btn.interactable = false;
                }
                else
                {
                    // Восстанавливаем цвет и включаем кнопку, если денег достаточно и уровень подходит
                    ColorBlock colors = btn.colors;
                    colors.normalColor = Color.white;
                    btn.colors = colors;

                    if (!btn.interactable)
                    {
                        int index = i;
                        btn.onClick.AddListener(() => OnButtonClick(index));
                    }

                    btn.interactable = true;
                }
            }
        }
    }


    public void UpdateValues(decimal newMoney, decimal newPerClick, decimal newIncome)
    {
        MainMenu.money = newMoney;
        MainMenu.perClick = newPerClick;
        MainMenu.income = newIncome;
        UpdateMoneyText();
    }

    void UpdateMoneyText()
    {
        moneyText.text = FormatMoney(MainMenu.money);
        incomeText.text = FormatMoney(MainMenu.income);
        perClickText.text = FormatMoney(MainMenu.perClick);
    }

    void setShop()
    {
        Debug.Log("Проверка массивов в методе setShop...");

        if (arrayTitles.Length > 0)
        {
            Debug.Log($"arrayTitles содержит {arrayTitles.Length} элементов");
        }
        else
        {
            Debug.LogError("arrayTitles пуст или имеет длину 0");
        }

        if (arrayDescriptions.Length > 0)
        {
            Debug.Log($"arrayDescriptions содержит {arrayDescriptions.Length} элементов");
        }
        else
        {
            Debug.LogError("arrayDescriptions пуст или имеет длину 0");
        }

        if (arrayAdditionalInfo.Length > 0)
        {
            Debug.Log($"arrayAdditionalInfo содержит {arrayAdditionalInfo.Length} элементов");
        }
        else
        {
            Debug.LogError("arrayAdditionalInfo пуст или имеет длину 0");
        }

        if (arraySprites.Length > 0)
        {
            Debug.Log($"arraySprites содержит {arraySprites.Length} элементов");
        }
        else
        {
            Debug.LogError("arraySprites пуст или имеет длину 0");
        }

        if (arrayTitles.Length > 0 &&
            arrayTitles.Length == arrayDescriptions.Length &&
            arrayTitles.Length == arrayAdditionalInfo.Length &&
            arrayTitles.Length == arraySprites.Length)
        {
            var pr1 = Instantiate(button, transform);
            var h = pr1.GetComponent<RectTransform>().rect.height;
            var tr = GetComponent<RectTransform>();
            tr.sizeDelta = new Vector2(tr.rect.width, h * arrayTitles.Length);
            Destroy(pr1);

            for (var i = 0; i < arrayTitles.Length; i++)
            {
                var pr = Instantiate(button, transform);
                TMP_Text[] texts = pr.GetComponentsInChildren<TMP_Text>();

                if (texts.Length >= 3)
                {
                    string suffix = (i % 2 == 0) ? " за клик" : " в секунду";
                    string suffix1 = "+";

                    // Проверка уровня перед установкой текста
                    if (ProgressBar.level < i)
                    {
                        Debug.Log($"Игрок имеет уровень {ProgressBar.level}, объект с индексом {i} заблокирован.");
                        texts[0].text = "???"; // Устанавливаем "???" вместо названия
                        texts[1].text = "???"; // Устанавливаем "???" вместо описания
                    }
                    else
                    {
                        Debug.Log($"Игрок имеет уровень {ProgressBar.level}, объект с индексом {i} доступен.");
                        texts[0].text = suffix1 + FormatMoney((decimal)arrayTitles[i]) + suffix;
                        texts[1].text = arrayDescriptions[i];
                    }

                    texts[1].enableAutoSizing = true;
                    texts[1].fontSizeMin = 20;
                    texts[1].fontSizeMax = 49;
                    texts[1].overflowMode = TextOverflowModes.Overflow;

                    RectTransform rectTransform = texts[1].GetComponent<RectTransform>();
                    texts[2].text = ProgressBar.level < i ? "???" : FormatMoney((decimal)arrayAdditionalInfo[i]);
                }

                Transform iconTransform = pr.transform.Find("Image");
                if (iconTransform != null)
                {
                    Image imageComponent = iconTransform.GetComponent<Image>();
                    if (imageComponent != null && arraySprites[i] != null)
                    {
                        imageComponent.sprite = arraySprites[i];
                    }
                }

                decimal cost = (decimal)arrayAdditionalInfo[i];
                Button buttonComponent = pr.GetComponent<Button>();
                if (MainMenu.money < cost)
                {
                    ColorBlock colorBlock = buttonComponent.colors;
                    colorBlock.normalColor = new Color(0.5f, 0.5f, 0.5f); // Затемнение
                    buttonComponent.colors = colorBlock;
                    buttonComponent.interactable = false;
                }
                else
                {
                    int index = i;
                    buttonComponent.onClick.AddListener(() => OnButtonClick(index));
                }

                list.Add(pr);
            }
        }
        else
        {
            Debug.LogError("Массивы заголовков, описаний, дополнительной информации или спрайтов имеют разные размеры или пусты.");
        }
    }



    void OnButtonClick(int index)
    {
        decimal cost = (decimal)arrayAdditionalInfo[index];
        Debug.Log("Цена: " + cost);

        if (MainMenu.money >= cost)
        {
            MainMenu.money -= cost;

            if (index % 2 != 0)
            {
                MainMenu.income += (decimal)arrayTitles[index];
            }
            else
            {
                MainMenu.perClick += (decimal)arrayTitles[index];
            }

            UpdateMoneyText();
            SaveDecimalToPrefs("money", MainMenu.money);
            SaveDecimalToPrefs("income", MainMenu.income);
            SaveDecimalToPrefs("perClick", MainMenu.perClick);

            Debug.Log("Статистика: " + MainMenu.money + " / " + MainMenu.income + " / " + MainMenu.perClick);
        }
    }

    public void SaveDecimalToPrefs(string key, decimal value)
    {
        PlayerPrefs.SetString(key, value.ToString());
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

    private string FormatMoney(decimal amount)
    {
        if (amount >= 1000000000)
        {
            return (amount / 1000000000m).ToString("0.#") + "B";
        }
        else if (amount >= 1000000)
        {
            return (amount / 1000000m).ToString("0.#") + "M";
        }
        else if (amount >= 1000)
        {
            return (amount / 1000m).ToString("0.#") + "K";
        }
        else
        {
            return amount.ToString("0.#");
        }
    }



    public void SetLanguage(int newLanguage)
    {

        switch (newLanguage)
        {
            case 0: // Russian
                arrayDescriptions = new string[] {
    "Рваный флаг", "Золотой компас", "Сабля пирата", "Сундук сокровищ",
    "Пиратская карта", "Бочка пороха", "Крюк капитана", "Штормовой румпель",
    "Тайный остров", "Непотопляемый корабль", "Галера удачи", "Гавань изгоев",
    "Картель контрабандистов", "Трофейные пушки", "Форт пиратов", "Армада корсаров",
    "Кокосовый риф", "Гроза морей", "Призрачный капитан", "Пещера пиратов",
    "Якорь удачи", "Каперский патент", "Морской змей", "Чёрная метка",
        };
                break;

            case 1: // English
                arrayDescriptions = new string[] {
    "Torn Flag", "Golden Compass", "Pirate's Saber", "Treasure Chest",
    "Pirate Map", "Barrel of Gunpowder", "Captain's Hook", "Stormy Helm",
    "Secret Island", "Unsinkable Ship", "Galley of Fortune", "Outlaw's Harbor",
    "Smuggler's Cartel", "Trophy Cannons", "Pirate Fort", "Corsair Armada",
    "Coconut Reef", "Sea Tempest", "Ghost Captain", "Pirate Cave",
    "Lucky Anchor", "Privateer's Letter", "Sea Serpent", "Black Spot",
        };
                break;
        }








        // Обновляем тексты на экране
        UpdateShopTexts(newLanguage);
    }
    private void UpdateShopTexts(int newLanguage)
    {
        string suffixClick = "";
        string suffixSecond = "";

        switch (newLanguage)
        {
            case 0: // Русский
                suffixClick = " за клик";
                suffixSecond = " в секунду";
                break;
            case 1: // Английский
                suffixClick = " per click";
                suffixSecond = " per second";
                break;
            default:
                suffixClick = " за клик";
                suffixSecond = " в секунду";
                break;
        }

        for (int i = 0; i < list.Count; i++)
        {
            GameObject buttonObj = list[i];
            TMP_Text[] texts = buttonObj.GetComponentsInChildren<TMP_Text>();

            if (texts.Length >= 3)
            {
                // Проверка уровня перед установкой текста
                if (ProgressBar.level < i)
                {
                    // Если уровень игрока ниже индекса, устанавливаем "???"
                    texts[0].text = "???"; // Заголовок
                    texts[1].text = "???"; // Описание
                    texts[2].text = "???"; // Дополнительная информация
                }
                else
                {
                    // Если уровень игрока достаточен, отображаем обычный текст
                    string suffix = (i % 2 == 0) ? suffixClick : suffixSecond;
                    texts[0].text = "+" + FormatMoney((decimal)arrayTitles[i]) + suffix;
                    texts[1].text = arrayDescriptions[i];
                    texts[2].text = FormatMoney((decimal)arrayAdditionalInfo[i]);
                }
            }
        }
    }


}
