using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PokemonPlayer))]
public class ShootPlayer : MonoBehaviour
{
    [Header("Reference")]
    public GameObject shootingUi;
    public Slider shootingSlider;
    public Image shootingSliderFill;
    public Slider overLoadSlider;
    public TextMeshProUGUI feedbackShootingText;

    private PokemonPlayer _pokemonPlayer;

    private float speedNerf = 0.3f;
    private bool isShootingMode = false;
    private float shootingTimer = 0f;
    private float baseShootingWindow = 2f;
    private float currentShootingWindow;
    private float _currentCursorPosition = 0f;
    private float _timeAt100Percent = 0f;
    private const float MAX_TIME_AT_100 = 0.2f;

    private const float OK_THRESHOLD = 0.7f;
    private const float GOOD_THRESHOLD = 0.8f;
    private const float PERFECT_THRESHOLD = 0.95f;

    private Vector3 _originalSliderPosition;
    private CanvasGroup _sliderCanvasGroup;

    void Awake()
    {
        _pokemonPlayer = GetComponent<PokemonPlayer>();
        shootingUi.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(_pokemonPlayer.ControlledByPlayer1 ? XboxInput.B1 : XboxInput.B2))
        {
            if (!isShootingMode)
            {
                StartShootingMode();
            }
            else
            {
                UpdateShootingMode();
            }
        }
        else if (isShootingMode && Input.GetKeyUp(_pokemonPlayer.ControlledByPlayer1 ? XboxInput.B1 : XboxInput.B2))
        {
            ExecuteShot();
        }

        if (Input.GetKeyDown(_pokemonPlayer.ControlledByPlayer1 ? XboxInput.X1 : XboxInput.X2))
        {
            if (isShootingMode)
            {
                CancelShootingMode();
            }
        }
    }

    private void StartShootingMode()
    {
        isShootingMode = true;
        shootingTimer = 0f;
        _timeAt100Percent = 0f;

        float precisionFactor = _pokemonPlayer.shootPrecision / 100f;
        currentShootingWindow = Mathf.Lerp(0.6f, 1.5f, precisionFactor);

        SetupJuiceEffects();

        shootingUi.SetActive(true);
        shootingSlider.value = 0f;
        overLoadSlider.value = 0f;

        _pokemonPlayer.speed *= speedNerf;
        StartCoroutine(FadeInSlider());
    }

    private void UpdateShootingMode()
    {
        shootingTimer += Time.deltaTime;
        float cycleSpeed = 2f / currentShootingWindow;
        float cursorPosition = (shootingTimer * cycleSpeed);

        if (cursorPosition >= 1f)
        {
            _timeAt100Percent += Time.deltaTime;
            if (_timeAt100Percent >= MAX_TIME_AT_100)
            {
                ExecuteShot();
                return;
            }
        }
        else
        {
            _timeAt100Percent = 0f;
        }

        shootingSlider.value = Mathf.Min(1f, cursorPosition);
        overLoadSlider.value = Mathf.Max(0f, cursorPosition - 1f);

        if (cursorPosition > 1f)
            shootingSliderFill.color = Color.red;
        else if (cursorPosition >= PERFECT_THRESHOLD)
            shootingSliderFill.color = Color.green;
        else if (cursorPosition >= GOOD_THRESHOLD)
            shootingSliderFill.color = Color.yellow;
        else if (cursorPosition >= OK_THRESHOLD)
            shootingSliderFill.color = new Color(1f, 0.5f, 0f); // orange
        else
            shootingSliderFill.color = Color.red;

        _currentCursorPosition = cursorPosition;
    }

    private void ExecuteShot()
    {
        float shootingQuality = CalculateShootingQuality(_currentCursorPosition);
        if (_timeAt100Percent >= MAX_TIME_AT_100)
            shootingQuality = 0f;

        float finalPrecision = _pokemonPlayer.shootPrecision * shootingQuality;
        var rim = _pokemonPlayer.Team.GetTargetRim();
        _pokemonPlayer.LoseBall();

        PlayShootAnimation();
        BasketBallManager.Instance.ShootTo(rim, finalPrecision, shootingQuality);
        ShowShotFeedback(shootingQuality, _currentCursorPosition);
        StartCoroutine(ShakeAndFadeOut(shootingQuality));
    }

    private void PlayShootAnimation()
    {
        var move = _pokemonPlayer.Direction;
        if (move.magnitude == 0)
        {
            _pokemonPlayer.pokemonPlayerAnimator.PlayOneShotAnimation(_pokemonPlayer.actualPokemon.shootTopRightAnimation);
            return;
        }
        if (move.x > 0)
            _pokemonPlayer.pokemonPlayerAnimator.PlayOneShotAnimation(move.z > 0 ? _pokemonPlayer.actualPokemon.shootTopRightAnimation : _pokemonPlayer.actualPokemon.shootBottomRightAnimation);
        else if (move.x < 0)
            _pokemonPlayer.pokemonPlayerAnimator.PlayOneShotAnimation(move.z > 0 ? _pokemonPlayer.actualPokemon.shootTopLeftAnimation : _pokemonPlayer.actualPokemon.shootBottomLeftAnimation);
        else
            _pokemonPlayer.pokemonPlayerAnimator.PlayOneShotAnimation(move.z > 0 ? _pokemonPlayer.actualPokemon.shootTopLeftAnimation : _pokemonPlayer.actualPokemon.bottomLeftAnimation);
    }

    private float CalculateShootingQuality(float cursorPosition)
    {
        if (cursorPosition >= PERFECT_THRESHOLD)
        {
            float distanceFrom100 = Mathf.Abs(1f - cursorPosition);
            return Mathf.Lerp(1.3f, 1.2f, distanceFrom100 * 20f);
        }
        else if (cursorPosition >= GOOD_THRESHOLD)
        {
            float normalizedInRange = (cursorPosition - GOOD_THRESHOLD) / (PERFECT_THRESHOLD - GOOD_THRESHOLD);
            return Mathf.Lerp(1.0f, 1.2f, normalizedInRange);
        }
        else if (cursorPosition >= OK_THRESHOLD)
        {
            float normalizedInRange = (cursorPosition - OK_THRESHOLD) / (GOOD_THRESHOLD - OK_THRESHOLD);
            return Mathf.Lerp(0.7f, 1.0f, normalizedInRange);
        }
        else
        {
            float normalizedPosition = cursorPosition / OK_THRESHOLD;
            return Mathf.Lerp(0.1f, 0.7f, normalizedPosition);
        }
    }

    private void ShowShotFeedback(float quality, float cursorPosition)
    {
        string feedbackText;
        if (_timeAt100Percent >= MAX_TIME_AT_100)
            feedbackText = "TOO LATE!";
        else if (quality >= 1.2f)
            feedbackText = "PERFECT SHOT!";
        else if (quality >= 1.0f)
            feedbackText = "EXCELLENT!";
        else if (quality >= 0.7f)
            feedbackText = "Good Shot";
        else if (quality >= 0.4f)
            feedbackText = "Off Target";
        else
            feedbackText = "Brick...";

        Debug.Log($"Shot Result: {feedbackText} | Quality: {quality:F2} | Cursor Position: {cursorPosition:P1} | Final Precision: {_pokemonPlayer.shootPrecision * quality:F1}");
    }

    private void CancelShootingMode()
    {
        if (isShootingMode)
            StartCoroutine(SimpleFadeOut());
    }

    private void SetupJuiceEffects()
    {
        _sliderCanvasGroup = shootingUi.GetComponent<CanvasGroup>();
        if (_sliderCanvasGroup == null)
            _sliderCanvasGroup = shootingUi.AddComponent<CanvasGroup>();

        _originalSliderPosition = shootingUi.transform.localPosition;
        _sliderCanvasGroup.alpha = 0f;
    }

    private IEnumerator FadeInSlider()
    {
        float duration = 0.3f, elapsed = 0f;
        shootingUi.transform.localScale = Vector3.one * 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            _sliderCanvasGroup.alpha = progress;

            float scale = Mathf.Lerp(0.5f, 1.2f, progress);
            if (progress > 0.7f) scale = Mathf.Lerp(1.2f, 1f, (progress - 0.7f) / 0.3f);
            shootingUi.transform.localScale = Vector3.one * scale;
            yield return null;
        }

        _sliderCanvasGroup.alpha = 1f;
        shootingUi.transform.localScale = Vector3.one;
    }

    private IEnumerator ShakeAndFadeOut(float shootingQuality)
    {
        float shakeIntensity = 20f, shakeDuration = 0.2f;
        if (shootingQuality >= 1.0f) { shakeIntensity = 30f; shakeDuration = 0.25f; }
        else if (shootingQuality >= 0.7f) { shakeIntensity = 40f; shakeDuration = 0.3f; }
        else if (shootingQuality >= 0.4f) { shakeIntensity = 60f; shakeDuration = 0.4f; }
        else { shakeIntensity = 100f; shakeDuration = 0.5f; }

        float shakeElapsed = 0f;
        while (shakeElapsed < shakeDuration)
        {
            shakeElapsed += Time.deltaTime;
            Vector3 shakeOffset = new Vector3(Random.Range(-shakeIntensity, shakeIntensity), Random.Range(-shakeIntensity, shakeIntensity), 0f);
            shootingUi.transform.localPosition = _originalSliderPosition + shakeOffset;
            yield return null;
        }

        shootingUi.transform.localPosition = _originalSliderPosition;

        float fadeDuration = 0.4f, fadeElapsed = 0f;
        while (fadeElapsed < fadeDuration)
        {
            fadeElapsed += Time.deltaTime;
            float progress = fadeElapsed / fadeDuration;
            _sliderCanvasGroup.alpha = 1f - progress;
            shootingUi.transform.localScale = Vector3.one * Mathf.Lerp(1f, 0.3f, progress);
            yield return null;
        }

        FinishShootingMode();
    }

    private IEnumerator SimpleFadeOut()
    {
        float fadeDuration = 0.3f, elapsed = 0f;
        float startAlpha = _sliderCanvasGroup != null ? _sliderCanvasGroup.alpha : 1f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / fadeDuration;
            _sliderCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, progress);
            shootingUi.transform.localScale = Vector3.one * Mathf.Lerp(1f, 0.5f, progress);
            yield return null;
        }

        FinishShootingMode();
    }

    private void FinishShootingMode()
    {
        isShootingMode = false;
        _timeAt100Percent = 0f;
        shootingUi.SetActive(false);
        shootingUi.transform.localPosition = _originalSliderPosition;
        shootingUi.transform.localScale = Vector3.one;
        if (_sliderCanvasGroup != null) _sliderCanvasGroup.alpha = 1f;
    }
}
