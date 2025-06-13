using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class VirtualCursor : MonoBehaviour
{
    [Header("Cursor Settings")]
    [SerializeField] private float cursorSpeed = 800f;
    [SerializeField] private bool constrainToScreen = true;

    [Header("Cursor Sprites")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite hoverSprite;
    [SerializeField] private Sprite clickSprite;

    [SerializeField] private Sprite outlineNormalSprite;
    [SerializeField] private Sprite outlineHoverSprite;
    [SerializeField] private Sprite outlineClickSprite;

    [SerializeField] private Image outlineImage;

    [SerializeField] private Canvas canvas;
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    private RectTransform rectTransform;
    private Image image;
    private EventSystem eventSystem;

    private GameObject currentHoveredObject;
    private GameObject lastHoveredObject;
    private List<RaycastResult> raycastResults = new List<RaycastResult>();

    public TextMeshProUGUI playerNumberIndicationText;
    public int actualControllerIndex = 1;

    private bool ControlledByPlayer1
    {
        get { return actualControllerIndex == 1; }
    }

    private bool isPressed = false;

    void Start()
    {
        InitializeComponents();
        InitializeCursor();

        playerNumberIndicationText.text = actualControllerIndex.ToString();
        playerNumberIndicationText.fontMaterial = new Material(playerNumberIndicationText.fontMaterial);

        if (actualControllerIndex == 1)
        {
            outlineImage.color = new Color(0f, 0.5f, 1, 1);
            playerNumberIndicationText.fontMaterial.SetColor("_GlowColor", new Color(0f, 0.5f, 0.7f, 1));
        }
        else
        {
            outlineImage.color = new Color(1, 0.2f, 0.2f, 1);
            playerNumberIndicationText.fontMaterial.SetColor("_GlowColor", new Color(0.7f, 0.2f, 0.2f, 1));
        }
    }

    void InitializeComponents()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        if (image != null)
            image.raycastTarget = false;

        eventSystem = EventSystem.current;
        if (eventSystem == null)
            Debug.LogError("VirtualCursor: No EventSystem found in scene!");
    }

    void InitializeCursor()
    {
        if (image != null && normalSprite != null)
            image.sprite = normalSprite;
    }

    void Update()
    {
        if (rectTransform == null) return;

        ReadInput();
        MoveCursor();
        HandleUIInteraction();
        UpdateVisuals();
    }

    void ReadInput()
    {
        float h = ControlledByPlayer1 ? Input.GetAxis("HorizontalJoystick1") : Input.GetAxis("HorizontalJoystick2");
        float v = ControlledByPlayer1 ? Input.GetAxis("VerticalJoystick1") : Input.GetAxis("VerticalJoystick2");

        Vector2 moveInput = new Vector2(h, v);

        if (moveInput.sqrMagnitude > 0.01f)
        {
            Vector2 movement = moveInput.normalized * cursorSpeed * Time.unscaledDeltaTime;
            Vector3 newPosition = rectTransform.position + (Vector3)movement;

            if (constrainToScreen)
            {
                newPosition.x = Mathf.Clamp(newPosition.x, 0, Screen.width);
                newPosition.y = Mathf.Clamp(newPosition.y, 0, Screen.height);
            }

            rectTransform.position = newPosition;
        }

        // Read button press
        KeyCode button = ControlledByPlayer1 ? KeyCode.Joystick1Button0 : KeyCode.Joystick2Button0;

        if (Input.GetKeyDown(button))
        {
            isPressed = true;
            PerformClick();
        }
        else if (Input.GetKeyUp(button))
        {
            isPressed = false;
        }
    }

    void MoveCursor() { /* Already handled in ReadInput */ }

    void HandleUIInteraction()
    {
        if (graphicRaycaster == null || eventSystem == null) return;

        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = rectTransform.position
        };

        raycastResults.Clear();
        graphicRaycaster.Raycast(pointerData, raycastResults);

        lastHoveredObject = currentHoveredObject;
        currentHoveredObject = null;

        if (raycastResults.Count > 0)
        {
            currentHoveredObject = raycastResults[0].gameObject;
        }

        if (currentHoveredObject != lastHoveredObject)
        {
            if (lastHoveredObject != null)
                ExecuteEvents.Execute(lastHoveredObject, pointerData, ExecuteEvents.pointerExitHandler);

            if (currentHoveredObject != null)
                ExecuteEvents.Execute(currentHoveredObject, pointerData, ExecuteEvents.pointerEnterHandler);
        }
    }

    void UpdateVisuals()
    {
        if (image == null) return;

        if (isPressed && clickSprite != null)
        {
            image.sprite = clickSprite;
            outlineImage.sprite = outlineClickSprite;
        }
        else if (currentHoveredObject != null && hoverSprite != null)
        {
            image.sprite = hoverSprite;
            outlineImage.sprite = outlineHoverSprite;
        }
        else if (normalSprite != null)
        {
            image.sprite = normalSprite;
            outlineImage.sprite = outlineNormalSprite;
        }
    }

    void PerformClick()
    {
        if (eventSystem == null) return;

        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = rectTransform.position,
            button = PointerEventData.InputButton.Left
        };

        if (currentHoveredObject != null)
        {
            if (currentHoveredObject.GetComponent<SelectablePokemonPrefab>() != null)
            {
                currentHoveredObject.GetComponent<SelectablePokemonPrefab>().SelectPokemon(actualControllerIndex - 1);
            }
            else
            {
                ExecuteEvents.Execute(currentHoveredObject, pointerData, ExecuteEvents.pointerDownHandler);
                ExecuteEvents.Execute(currentHoveredObject, pointerData, ExecuteEvents.pointerClickHandler);
                ExecuteEvents.Execute(currentHoveredObject, pointerData, ExecuteEvents.submitHandler);
            }
        }
    }
}
