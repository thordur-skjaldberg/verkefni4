using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance { get; private set; }

    public float displayTime = 4.0f;
    private VisualElement healthBar;

    private void Awake()
    {
        instance = this; // Skapar einstakt tilvik af UIHandler
    }

    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        healthBar = uiDocument.rootVisualElement.Q<VisualElement>("HealthBar"); // Finnur heilsubarann í UI
    }

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        float percentage = (float)currentHealth / maxHealth; // Reiknar prósentu heilsu
        healthBar.style.width = Length.Percent(percentage * 100); // Uppfærir breidd heilsubars
    }
}
