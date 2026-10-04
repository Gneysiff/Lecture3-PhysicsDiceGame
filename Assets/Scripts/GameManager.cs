using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro; // Для вывода очков на экран

public class GameManager : MonoBehaviour
{
    [Header("Ссылки на объекты")]
    [SerializeField] private List<Dice> dices = new List<Dice>(); // Список всех кубиков НААА СТОЛЕЕЕ
    [SerializeField] private TextMeshProUGUI scoreText; // Ссылка для вывода очков

    [Header("Настройки ввода (Input System)")]
    [SerializeField] private InputActionReference throwActionRef; // Ссылка на бросок 

    private bool isCalculatingScore; // Флаг, идёт ли сейчас процесс подсчёта очков

    // Публичные свойства с проверкой
    public List<Dice> Dices => dices;
    public bool IsCalculatingScore => isCalculatingScore;

    public TextMeshProUGUI ScoreText
    {
        get => scoreText;
        set
        {
            if (value == null)
            {
                Debug.LogWarning("Очков не может быть null! Даже если 0, но не null!");
                return;
            }
            scoreText = value;
        }
    }

    private void OnEnable()
    {
        // Отслеживаем нажатие кнопки
        if (throwActionRef != null && throwActionRef.action != null)
        {
            throwActionRef.action.Enable();
            throwActionRef.action.performed += OnThrowPressed;
        }
    }

    private void OnDisable()
    {
        // Отключение отслеживания кнопки
        if (throwActionRef != null && throwActionRef.action != null)
        {
            throwActionRef.action.performed -= OnThrowPressed;
            throwActionRef.action.Disable();
        }
    }

    private void OnThrowPressed(InputAction.CallbackContext context)
    {
        Debug.Log("Клавиша Space нажата! Начинаем вращать!"); // для проверки

        if (isCalculatingScore) return;

        if (scoreText != null)
        {
            scoreText.text = "Вращайте барабан(кубики)";
        }

        // Запускаем бросок для каждого кубика на сцене
        foreach (var dice in dices)
        {
            if (dice != null)
            {
                dice.Roll();
            }
        }

        // Запускаем корутину(страшное слово) ожидания остановки кубиков
        StartCoroutine(WaitAndCalculateScoreRoutine());
    }

    private IEnumerator WaitAndCalculateScoreRoutine()
    {
        isCalculatingScore = true;

        // Небольшая задержка, чтобы физика успела придать кубикам скорость
        yield return new WaitForSeconds(0.2f);

        // Ждём в цикле, пока ВСЕ кубики перестанут флексить
        var allStopped = false;
        while (!allStopped)
        {
            allStopped = true;
            foreach (var dice in dices)
            {
                if (dice != null && dice.IsRolling)
                {
                    allStopped = false; // Хотя бы один кубик ещё движется
                    break;
                }
            }
            yield return null; // Ждём следующий кадр
        }

        // Сумма выпавших очков
        var totalScore = 0;
        foreach (var dice in dices)
        {
            if (dice != null)
            {
                totalScore += dice.GetUpwardValue();
            }
        }

        // Выводим итоговый результат
        if (scoreText != null)
        {
            scoreText.text = $"Сумма очков: {totalScore}";
        }

        Debug.Log($"Выпало очков: {totalScore}");
        isCalculatingScore = false;
    }
}