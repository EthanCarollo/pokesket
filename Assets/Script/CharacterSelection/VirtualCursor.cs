using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class VirtualCursor : MonoBehaviour
{
    [Header("Cursor Settings")]
    [SerializeField] private float cursorSpeed = 800f;
    [SerializeField] private bool constrainToScreen = true;
    
    [Header("Cursor Sprites")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite hoverSprite;
    [SerializeField] private Sprite clickSprite;
    
    // Components
    private RectTransform rectTransform;
    private Image image;
    private PlayerInput playerInput;
    private Canvas canvas;
    private GraphicRaycaster graphicRaycaster;
    private EventSystem eventSystem;
    
    // Input
    private Vector2 moveInput;
    private bool isPressed = false;
    
    // UI Interaction
    private GameObject currentHoveredObject;
    private GameObject lastHoveredObject;
    private List<RaycastResult> raycastResults = new List<RaycastResult>();
    
    void Start()
    {
        InitializeComponents();
        SetupInputCallbacks();
        InitializeCursor();
        if (playerInput != null && playerInput.notificationBehavior == PlayerNotifications.InvokeUnityEvents)
        {
            Debug.LogError("VirtualCursor: PlayerInput is set to use Unity Events. " +
                           "Please change to 'Send Messages' or 'Invoke C# Events' in the PlayerInput component.");
        }
    }
    
    void InitializeComponents()
    {
        // Get this object's components
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        
        // Make sure this object doesn't block raycasts
        if (image != null)
        {
            image.raycastTarget = false;
        }
        
        // Find PlayerInput (can be on this object or parent)
        playerInput = GetComponentInParent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("VirtualCursor: PlayerInput component not found on this object or parent!");
            return;
        }
        
        // Find Canvas (parent of this UI element)
        canvas = FindAnyObjectByType<Canvas>();
        if (canvas != null)
        {
            graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
            this.transform.SetParent(canvas.transform, false);
        }
        
        // Find EventSystem
        eventSystem = GetComponent<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("VirtualCursor: No EventSystem found in scene!");
        }
    }
    
    void SetupInputCallbacks()
    {
        // Only setup callbacks if PlayerInput is NOT using Unity Events
        if (playerInput != null && playerInput.notificationBehavior != PlayerNotifications.InvokeUnityEvents)
        {
            if (playerInput.actions != null)
            {
                Debug.LogWarning("Set different actionss");
                // Movement
                var moveAction = playerInput.actions.FindAction("Move");
                if (moveAction != null)
                {
                    moveAction.performed += OnMove;
                    moveAction.canceled += OnMove;
                }
                else
                {
                    Debug.LogWarning("Move action doesn't exist!");
                }
                
                // Click/Submit
                var submitAction = playerInput.actions.FindAction("Interact");
                if (submitAction != null)
                {
                    submitAction.performed += OnSubmit;
                    submitAction.canceled += OnSubmitRelease;
                }
                else
                {
                    Debug.LogWarning("Submit action doesn't exist!");
                }
            }
        }
    }
    
    void InitializeCursor()
    {
        if (rectTransform != null)
        {
            // Move this GameObject to be direct child of Canvas
            if (canvas != null)
            {
                rectTransform.SetParent(canvas.transform, false);
            }
            
            // Position cursor at center of screen initially
            rectTransform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
            
            // Make sure cursor is on top
            rectTransform.SetAsLastSibling();
        }
        
        // Set initial sprite
        if (image != null && normalSprite != null)
        {
            image.sprite = normalSprite;
        }
    }
    
    void Update()
    {
        if (rectTransform == null) return;
        
        MoveCursor();
        HandleUIInteraction();
        UpdateVisuals();
    }
    
    void MoveCursor()
    {
        if (moveInput == Vector2.zero) return;
        
        // Calculate movement
        Vector2 movement = moveInput * cursorSpeed * Time.unscaledDeltaTime;
        Vector3 newPosition = rectTransform.position + (Vector3)movement;
        
        // Constrain to screen bounds if enabled
        if (constrainToScreen)
        {
            newPosition.x = Mathf.Clamp(newPosition.x, 0, Screen.width);
            newPosition.y = Mathf.Clamp(newPosition.y, 0, Screen.height);
        }
        
        rectTransform.position = newPosition;
    }
    
    void HandleUIInteraction()
    {
        if (graphicRaycaster == null || eventSystem == null) return;
        
        // Create pointer event data
        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = rectTransform.position
        };
        
        // Perform raycast
        raycastResults.Clear();
        graphicRaycaster.Raycast(pointerData, raycastResults);
        
        // Update hovered object
        lastHoveredObject = currentHoveredObject;
        currentHoveredObject = null;
        
        if (raycastResults.Count > 0)
        {
            currentHoveredObject = raycastResults[0].gameObject;
        }
        
        // Handle hover enter/exit
        if (currentHoveredObject != lastHoveredObject)
        {
            // Exit previous object
            if (lastHoveredObject != null)
            {
                ExecuteEvents.Execute(lastHoveredObject, pointerData, ExecuteEvents.pointerExitHandler);
            }
            
            // Enter new object
            if (currentHoveredObject != null)
            {
                ExecuteEvents.Execute(currentHoveredObject, pointerData, ExecuteEvents.pointerEnterHandler);
            }
        }
    }
    
    void UpdateVisuals()
    {
        if (image == null) return;
        
        // Update cursor sprite based on state
        if (isPressed && clickSprite != null)
        {
            image.sprite = clickSprite;
        }
        else if (currentHoveredObject != null && hoverSprite != null)
        {
            image.sprite = hoverSprite;
        }
        else if (normalSprite != null)
        {
            image.sprite = normalSprite;
        }
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log($"OnMove called: {context.ReadValue<Vector2>()}");
        moveInput = context.ReadValue<Vector2>();
    }
    
    public void OnSubmit(InputAction.CallbackContext context)
    {
        Debug.Log("OnSubmit called");
        isPressed = true;
        PerformClick();
    }
    
    public void OnSubmitRelease(InputAction.CallbackContext context)
    {
        Debug.Log("OnSubmitRelease called");
        isPressed = false;
    }
    
    void PerformClick()
    {
        if (eventSystem == null) return;
        
        // Create pointer event data
        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = rectTransform.position,
            button = PointerEventData.InputButton.Left
        };
        
        if (currentHoveredObject != null)
        {
            // Execute click events
            Debug.Log("Found an hovered object");
            if (currentHoveredObject.GetComponent<SelectablePokemonPrefab>() != null)
            {
                currentHoveredObject.GetComponent<SelectablePokemonPrefab>().SelectPokemon(playerInput.playerIndex);
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