using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using YG;

public class MainMenu : MonoBehaviour
{
    public static decimal money = 0;
    public static decimal perClick = 1;
    public static decimal income = 0;
    public static int language = 0;

    [SerializeField] private bool isShop = true;
    [SerializeField] private bool isZombieList = false;
    private bool isSoundOn = true; // Состояние звука

    public TMP_Text moneyText;
    public TMP_Text incomeText;
    public TMP_Text perClickText;
    public TMP_Text levelText;
    public TMP_Text x2Text;

    public Button soundButton;
    public Button ruLanguageButton;
    public Button engLanguageButton;

    public Sprite soundOnIcon;
    public Sprite soundOffIcon;

    public Sprite rusIcon;

    public Sprite engIcon;

    public Sprite turkIcon;


    public Image soundButtonImage;
    public Image languageButtonImage;

    public GameObject shopWindow;

    public AudioSource audioSource;
    public AudioSource audioSourceButton;

    public ProgressBar progressBar;

    public GameObject floatingTextPrefab;
    public Transform floatingTextSpawnPoint;


    public float totalTime = 60f; // Установите общее время в секундах
    private float currentTime;
    public TMP_Text timerText; // Перетащите сюда ссылку на ваш TMP_Text компонент из инспектора
    public Button adsButton; // Перетащите сюда ссылку на вашу кнопку из инспектора
    private bool isTimerRunning = false;
    private bool isCooldownActive = false; // Переменная для отслеживания блокировки кнопки




    private void Start()
    {
        YandexGame.StickyAdActivity(true);
        YandexGame.FullscreenShow();

        money = LoadDecimalFromPrefs("money", 0);
        perClick = LoadDecimalFromPrefs("perClick", 1);
        income = LoadDecimalFromPrefs("income", 0);

        StartCoroutine(IdleFarm());

        if (ruLanguageButton == null || engLanguageButton == null)
        {
            Debug.LogError("One or more buttons are not assigned!");
            return; // Прекращаем выполнение метода, если кнопки не назначены
        }

        ruLanguageButton.onClick.AddListener(() => SwitchLanguage(ruLanguageButton, engLanguageButton, 1));
        engLanguageButton.onClick.AddListener(() => SwitchLanguage(engLanguageButton, ruLanguageButton, 0));

        soundButton.onClick.AddListener(OnButtonSoundClick);
        soundButton.onClick.AddListener(InitializeAudioContext);

        if (floatingTextPrefab == null)
        {
            floatingTextPrefab = Resources.Load<GameObject>("FloatingText");
        }

        adsButton.onClick.AddListener(StartTimer); // Добавляем обработчик нажатия на кнопку
        UpdateTimerDisplay(currentTime);
    }

    private void Update()
    {
        moneyText.text = FormatMoney(money);
        incomeText.text = FormatMoney(income);
        perClickText.text = FormatMoney(perClick);

        switch (language)
        {
            case 0:
                levelText.text = "Уровень " + FormatMoney(ProgressBar.level);
                x2Text.text = "x2 клик";
                break;
            case 1:
                levelText.text = "Level " + FormatMoney(ProgressBar.level);
                x2Text.text = "x2 click";
                break;
            default:
                levelText.text = "Level " + FormatMoney(ProgressBar.level); // Значение по умолчанию
                x2Text.text = "x2 click";
                break;
        }
    }

    private void OnEnable()
    {
        // Подписываемся на событие успешного просмотра рекламы
        YandexGame.RewardVideoEvent += Rewarded;
    }

    private void OnDisable()
    {
        // Отписываемся от события успешного просмотра рекламы
        YandexGame.RewardVideoEvent -= Rewarded;
    }

    // Метод, который вызывается при успешном просмотре рекламы
    void Rewarded(int id)
    {
        if (id == 0) // Если ID равен 0, то выполняем вознаграждение
        {
            perClick *= 2; // Умножаем на 2 только при успешном просмотре рекламы
            SaveDecimalToPrefs("perClick", perClick);
        }
    }

    private void StartTimer()
    {
        if (isCooldownActive)
        {
            Debug.LogWarning("Попытка запуска таймера при активной блокировке. Таймер не запущен.");
            return;
        }

        isTimerRunning = true;
        adsButton.interactable = false;

        // Показываем рекламу, но не увеличиваем perClick сразу
        YandexGame.RewVideoShow(0);

        currentTime = totalTime;
        UpdateTimerDisplay(currentTime);

        StartCoroutine(UnlockButtonAfterCooldown()); // Запускаем таймер для разблокировки кнопки
    }

    private System.Collections.IEnumerator UnlockButtonAfterCooldown()
    {
        isCooldownActive = true;
        float remainingTime = 60f; // 60 секунд ожидания

        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            UpdateTimerDisplay(remainingTime); // Можно обновить отображение таймера
            yield return null;
        }

        isCooldownActive = false;
        adsButton.interactable = true; // Разблокировать кнопку после ожидания
        UpdateTimerDisplay(0); // Обновление дисплея таймера на "00:00" или на сообщение, которое вы хотите
    }

    private void UpdateTimerDisplay(float timeToDisplay)
    {
        timeToDisplay = Mathf.Max(0, timeToDisplay);

        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);

        if (timeToDisplay > 0)
        {
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            timerText.text = "Умножение на 2 активировано"; // Сообщение при завершении ожидания
        }
    }



    void SwitchLanguage(Button buttonToHide, Button buttonToShow, int lang)
    {
        PlayButtonSound();
        language = lang; // Обновляем глобальную переменную
        PlayerPrefs.SetInt("language", language);
        PlayerPrefs.Save(); // Сохраняем изменения
        buttonToHide.gameObject.SetActive(false); // Скрыть текущую кнопку
        buttonToShow.gameObject.SetActive(true);  // Показать следующую кнопку
    }

    void OnButtonShopClick()
    {
        PlayButtonSound();
        isShop = true;
        isZombieList = false;
        UpdateWindows();
    }

    void OnButtonZombieListClick()
    {
        PlayButtonSound();
        isShop = false;
        isZombieList = true;
        UpdateWindows();
    }

    void UpdateWindows()
    {
        shopWindow.SetActive(isShop);
    }
    void OnButtonSoundClick()
    {
        isSoundOn = !isSoundOn;
        soundButtonImage.sprite = isSoundOn ? soundOnIcon : soundOffIcon;
        audioSource.mute = !isSoundOn; // Устанавливаем mute в зависимости от состояния звука
        audioSourceButton.mute = !isSoundOn; // Устанавливаем mute для кнопок
    }


    public void ButtonClick()
    {
        money += perClick;
        YandexGame.NewLeaderboardScores("score", (long)perClick);

        ProgressBar.current += 10;
        SaveDecimalToPrefs("money", money);
        SaveDecimalToPrefs("current", ProgressBar.current);
        progressBar.SetHealth(ProgressBar.current);

        if (isSoundOn)
        {
            audioSource.Play();
        }

        ShowFloatingText("+" + perClick);
    }

    private void ShowFloatingText(string text)
    {
        if (floatingTextPrefab != null)
        {
            float randomX = Random.Range(-1f, 3f);
            float randomY = Random.Range(-3f, 1f);
            Vector3 spawnPosition = floatingTextSpawnPoint.position + new Vector3(randomX, randomY, 0);

            GameObject floatingText = Instantiate(floatingTextPrefab, floatingTextSpawnPoint);
            floatingText.GetComponent<RectTransform>().anchoredPosition = new Vector2(spawnPosition.x, spawnPosition.y);
            floatingText.GetComponent<FloatingText>().Initialize(text, spawnPosition);
        }
    }

    IEnumerator IdleFarm()
    {
        while (true)
        {
            yield return new WaitForSeconds(4);
            money += income;
            SaveDecimalToPrefs("money", money);
        }
    }



    public string FormatMoney(decimal amount)
    {
        if (amount >= 1000000000)
            return (amount / 1000000000m).ToString("0.#") + "B";
        else if (amount >= 1000000)
            return (amount / 1000000m).ToString("0.#") + "M";
        else if (amount >= 1000)
            return (amount / 1000m).ToString("0.#") + "K";
        else
            return amount.ToString("0.#");
    }

    public void SaveDecimalToPrefs(string key, decimal value)
    {
        PlayerPrefs.SetString(key, value.ToString());
    }

    public decimal LoadDecimalFromPrefs(string key, decimal defaultValue)
    {
        string stringValue = PlayerPrefs.GetString(key, defaultValue.ToString());
        if (decimal.TryParse(stringValue, out decimal result))
            return result;
        return defaultValue;
    }

    private void PlayButtonSound()
    {
        audioSourceButton.Play();
    }

    private bool audioInitialized = false;

    private void InitializeAudioContext()
    {
        if (!audioInitialized)
        {
            // Инициализация аудио здесь, если требуется
            audioInitialized = true;
        }

        // Теперь можно воспроизводить звук
        PlayButtonSound();
    }
}
