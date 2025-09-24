using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MobileInputManager : MonoBehaviour
{
    [Header("Mobile UI Elements")]
    [SerializeField] private GameObject mobileUI;
    [SerializeField] private Joystick movementJoystick;
    [SerializeField] private Button dashButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button shootButton;

    private Player player;
    private bool isMobilePlatform;

    private void Start()
    {
        // Verifica se siamo su mobile
        isMobilePlatform = Application.platform == RuntimePlatform.Android || 
                          Application.platform == RuntimePlatform.IPhonePlayer;
        
        SetupMobileControls();
        
        if (Player.Instance != null)
            player = Player.Instance;
    }

    private void SetupMobileControls()
    {
        if (mobileUI != null)
        {
            mobileUI.SetActive(isMobilePlatform);
        }

        if (isMobilePlatform)
        {
            SetupMobileButtons();
        }
    }

    private void SetupMobileButtons()
    {
        // Setup dash button
        if (dashButton != null)
        {
            dashButton.onClick.AddListener(OnDashButtonPressed);
        }

        // Setup pause button
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(OnPauseButtonPressed);
        }

        // Setup shoot button (per continuous shooting)
        if (shootButton != null)
        {
            // Usa EventTrigger per press/release
            EventTrigger trigger = shootButton.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = shootButton.gameObject.AddComponent<EventTrigger>();

            // On Pointer Down
            EventTrigger.Entry pointerDown = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerDown;
            pointerDown.callback.AddListener((data) => OnShootButtonPressed(true));
            trigger.triggers.Add(pointerDown);

            // On Pointer Up
            EventTrigger.Entry pointerUp = new EventTrigger.Entry();
            pointerUp.eventID = EventTriggerType.PointerUp;
            pointerUp.callback.AddListener((data) => OnShootButtonPressed(false));
            trigger.triggers.Add(pointerUp);
        }
    }

    private void Update()
    {
        if (isMobilePlatform && player != null)
        {
            HandleMobileMovement();
        }
    }

    private void HandleMobileMovement()
    {
        if (movementJoystick != null)
        {
            Vector2 inputVector = movementJoystick.Direction;
            // Applica il movimento al player
            // Dovrai modificare Player.cs per accettare input esterno
            player.SetMovementInput(inputVector);
        }
    }

    private void OnDashButtonPressed()
    {
        if (player != null)
        {
            player.TriggerDash();
        }
    }

    private void OnPauseButtonPressed()
    {
        // Comunica con UIManager
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.OnSettingsPanelClosed(); // O metodo appropriato
        }
    }

    private void OnShootButtonPressed(bool isPressed)
    {
        // Comunica con il sistema di armi
        if (player != null)
        {
            Weapon[] weapons = player.GetComponentsInChildren<Weapon>();
            foreach (Weapon weapon in weapons)
            {
                if (weapon.gameObject.activeInHierarchy)
                {
                    weapon.SetShooting(isPressed);
                }
            }
        }
    }
}

// Classe per Virtual Joystick
public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private float handleRange = 1f;
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;

    private Vector2 inputVector = Vector2.zero;
    private Canvas canvas;

    public Vector2 Direction => inputVector;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            canvas = FindObjectOfType<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, background.position);
        Vector2 radius = background.sizeDelta / 2;
        
        inputVector = (eventData.position - position) / (radius * canvas.scaleFactor);
        inputVector = inputVector.magnitude > 1f ? inputVector.normalized : inputVector;

        // Move handle
        handle.anchoredPosition = inputVector * radius * handleRange;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }
}