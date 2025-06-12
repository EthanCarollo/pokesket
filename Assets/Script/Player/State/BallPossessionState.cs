using System.Collections;
using UnityEngine;

public class BallPossessionState : IPokemonPlayerState
{
    private PokemonPlayer _pokemonPlayer;
    private float speed = 5f;
    private bool _isPassing = false;
    
    // Shooting timing system - NBA Style
    private bool isShootingMode = false;
    private float shootingTimer = 0f;
    private float baseShootingWindow = 2f; // Durée de base pour une précision de 100
    private float currentShootingWindow; // Durée adaptée selon la précision
    private float _currentCursorPosition = 0f; // Position réelle du curseur pour les calculs
    private float _timeAt100Percent = 0f; // Temps passé à 100%
    private const float MAX_TIME_AT_100 = 0.2f; // 0.2 secondes max à 100%
    
    // Seuils linéaires
    private const float OK_THRESHOLD = 0.7f;     // 70% = tir OK
    private const float GOOD_THRESHOLD = 0.8f;   // 80% = bon tir  
    private const float PERFECT_THRESHOLD = 0.95f; // 95% = tir parfait
    
    // Juice effects
    private Vector3 _originalSliderPosition;
    private CanvasGroup _sliderCanvasGroup;

    public BallPossessionState(PokemonPlayer pokemonPlayer)
    {
        _pokemonPlayer = pokemonPlayer;
        _pokemonPlayer.speed = speed;
    }

    public void Update()
    {
        if (!_pokemonPlayer.HasBall)
        {
            _pokemonPlayer.UpdateState(new PlayerAttackState(_pokemonPlayer));
            return;
        }

        if (Input.GetKey(_pokemonPlayer.ControlledByPlayer1 ? KeyCode.Joystick1Button1 : KeyCode.Joystick2Button1))
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
        // Relâcher pour tirer
        else if (isShootingMode && Input.GetKeyUp(_pokemonPlayer.ControlledByPlayer1 ? KeyCode.Joystick1Button1 : KeyCode.Joystick2Button1))
        {
            ExecuteShot();
        }

        // Passe (X)
        if (Input.GetKeyDown(_pokemonPlayer.ControlledByPlayer1 ? KeyCode.Joystick1Button3 : KeyCode.Joystick2Button3))
        {
            if (isShootingMode)
            {
                CancelShootingMode();
            }
            else
            {
                PassBall();
            }
        }
    }

    public void HandleMovement()
    {
        float h = _pokemonPlayer.ControlledByPlayer1 ? Input.GetAxis("HorizontalJoystick1") : Input.GetAxis("HorizontalJoystick2");
        float v = _pokemonPlayer.ControlledByPlayer1 ? Input.GetAxis("VerticalJoystick1") : Input.GetAxis("VerticalJoystick2");

        Vector3 move = new Vector3(h, 0, v);
        _pokemonPlayer.ApplyMovement(move);
    }

    private void StartShootingMode()
    {
        isShootingMode = true;
        shootingTimer = 0f;
        _timeAt100Percent = 0f;

        // Calculer la difficulté selon la précision (0-100)
        float precisionFactor = _pokemonPlayer.shootPrecision / 100f; // Normaliser entre 0 et 1

        // Plus la précision est faible, plus c'est difficile
        // Précision 100 = fenêtre de 1.5s, Précision 0 = fenêtre de 0.6s (plus rapide)
        currentShootingWindow = Mathf.Lerp(0.6f, 1.5f, precisionFactor);

        // Setup juice effects
        SetupJuiceEffects();

        _pokemonPlayer.shootingUi.SetActive(true);
        _pokemonPlayer.shootingSlider.value = 0f;

        // Le joueur ralentit quand il vise
        _pokemonPlayer.speed = speed * 0.3f;

        // Fade in rapide avec un petit bounce
        _pokemonPlayer.StartCoroutine(FadeInSlider());

        Debug.Log($"Shooting started - Precision: {_pokemonPlayer.shootPrecision}, Window: {currentShootingWindow:F2}s");
    }

    private void UpdateShootingMode()
    {
        shootingTimer += Time.deltaTime;
        
        // Vitesse adaptée à la fenêtre de tir (plus rapide)
        float cycleSpeed = 2f / currentShootingWindow; // Multiplié par 2 pour aller plus vite
        
        // Le curseur va de 0 à 1 en continu SANS retour automatique
        float cursorPosition = (shootingTimer * cycleSpeed);
        
        // Si on dépasse 1 (100%), on reste bloqué à 1
        if (cursorPosition >= 1f)
        {
            cursorPosition = 1f;
            _timeAt100Percent += Time.deltaTime;
            
            // Forcer le tir si on reste trop longtemps à 100%
            if (_timeAt100Percent >= MAX_TIME_AT_100)
            {
                Debug.Log("Forced shot - Too long at 100%!");
                ExecuteShot();
                return;
            }
        }
        else
        {
            _timeAt100Percent = 0f; // Reset si on n'est pas encore à 100%
        }
        
        // Le slider suit directement la position du curseur (linéaire et rapide)
        _pokemonPlayer.shootingSlider.value = cursorPosition;
        
        // Couleurs selon les seuils linéaires
        if (cursorPosition >= PERFECT_THRESHOLD)
        {
            _pokemonPlayer.shootingSliderFill.color = Color.green; // 95%+ = Parfait
        }
        else if (cursorPosition >= GOOD_THRESHOLD)
        {
            _pokemonPlayer.shootingSliderFill.color = Color.yellow; // 80%+ = Bien
        }
        else if (cursorPosition >= OK_THRESHOLD)
        {
            _pokemonPlayer.shootingSliderFill.color = new Color(1f, 0.5f, 0f); // 70%+ = OK (orange)
        }
        else
        {
            _pokemonPlayer.shootingSliderFill.color = Color.red; // <70% = Raté
        }
        
        // Stocker la position réelle du curseur pour les calculs
        _currentCursorPosition = cursorPosition;
    }


    private void ExecuteShot()
    {
        // Utiliser la position réelle du curseur pour les calculs
        float shootingQuality = CalculateShootingQuality(_currentCursorPosition);
        
        // Si on a forcé le tir (trop longtemps à 100%), précision = 0%
        if (_timeAt100Percent >= MAX_TIME_AT_100)
        {
            shootingQuality = 0f;
        }
        
        // La précision finale combine la précision du joueur et la qualité du timing
        float finalPrecision = _pokemonPlayer.shootPrecision * shootingQuality;
        
        var rim = _pokemonPlayer.Team.GetTargetRim();
        _pokemonPlayer.LoseBall();

        PlayShootAnimation();
        BasketBallManager.Instance.ShootTo(rim, finalPrecision, shootingQuality);
        
        // Feedback visuel avec juice !
        ShowShotFeedback(shootingQuality, _currentCursorPosition);
        
        // Shake + Fade out selon la qualité
        _pokemonPlayer.StartCoroutine(ShakeAndFadeOut(shootingQuality));
    }

    private void PlayShootAnimation()
    {
        var move = _pokemonPlayer.Direction;
        
        if (move.magnitude == 0)
        {
            this._pokemonPlayer.pokemonPlayerAnimator.PlayOneShotAnimation(this._pokemonPlayer.actualPokemon.shootTopRightAnimation);
            return;
        }

        // Sélectionner l'animation en fonction de la direction
        if (move.x > 0)
        {
            if (move.z > 0)
            {
                this._pokemonPlayer.pokemonPlayerAnimator.PlayOneShotAnimation(this._pokemonPlayer.actualPokemon.shootTopRightAnimation);
                
            }
            else
            {
                this._pokemonPlayer.pokemonPlayerAnimator.PlayOneShotAnimation(this._pokemonPlayer.actualPokemon.shootBottomRightAnimation);
                
            }
        }
        else if (move.x < 0)
        {
            if (move.z > 0)
            {
                this._pokemonPlayer.pokemonPlayerAnimator.PlayOneShotAnimation(this._pokemonPlayer.actualPokemon.shootTopLeftAnimation);
                
            }
            else
            {
                this._pokemonPlayer.pokemonPlayerAnimator.PlayOneShotAnimation(this._pokemonPlayer.actualPokemon.shootBottomLeftAnimation);
                
            }
        }
        else
        {
            if (move.z > 0)
            {
                this._pokemonPlayer.pokemonPlayerAnimator.PlayOneShotAnimation(this._pokemonPlayer.actualPokemon.shootTopLeftAnimation);
            }
            else if (move.z < 0)
            {
                this._pokemonPlayer.pokemonPlayerAnimator.PlayOneShotAnimation(this._pokemonPlayer.actualPokemon.bottomLeftAnimation);
            }
        }

    }

    private float CalculateShootingQuality(float cursorPosition)
    {
        // Système linéaire simple
        if (cursorPosition >= PERFECT_THRESHOLD) // 95%+
        {
            // Tir parfait : 120-130% selon la proximité du pic
            float distanceFrom100 = Mathf.Abs(1f - cursorPosition);
            float quality = Mathf.Lerp(1.3f, 1.2f, distanceFrom100 * 20f); // 20f pour étendre l'effet
            return quality;
        }
        else if (cursorPosition >= GOOD_THRESHOLD) // 80-95%
        {
            // Bon tir : interpolation linéaire entre 100% et 120%
            float normalizedInRange = (cursorPosition - GOOD_THRESHOLD) / (PERFECT_THRESHOLD - GOOD_THRESHOLD);
            return Mathf.Lerp(1.0f, 1.2f, normalizedInRange);
        }
        else if (cursorPosition >= OK_THRESHOLD) // 70-80%
        {
            // Tir OK : interpolation linéaire entre 70% et 100%
            float normalizedInRange = (cursorPosition - OK_THRESHOLD) / (GOOD_THRESHOLD - OK_THRESHOLD);
            return Mathf.Lerp(0.7f, 1.0f, normalizedInRange);
        }
        else // <70%
        {
            // Mauvais tir : dégradation linéaire de 70% à 10%
            float normalizedPosition = cursorPosition / OK_THRESHOLD;
            return Mathf.Lerp(0.1f, 0.7f, normalizedPosition);
        }
    }
    
    private void ShowShotFeedback(float quality, float cursorPosition)
    {
        string feedbackText;
        Color feedbackColor;
        
        // Cas spécial : tir forcé après 0.2s à 100%
        if (_timeAt100Percent >= MAX_TIME_AT_100)
        {
            feedbackText = "TOO LATE!";
            feedbackColor = Color.red;
        }
        else if (quality >= 1.2f)
        {
            feedbackText = "PERFECT SHOT!";
            feedbackColor = Color.green;
        }
        else if (quality >= 1.0f)
        {
            feedbackText = "EXCELLENT!";
            feedbackColor = Color.green;
        }
        else if (quality >= 0.7f)
        {
            feedbackText = "Good Shot";
            feedbackColor = Color.yellow;
        }
        else if (quality >= 0.4f)
        {
            feedbackText = "Off Target";
            feedbackColor = new Color(1f, 0.5f, 0f); // Orange (RGB)
        }
        else
        {
            feedbackText = "Brick...";
            feedbackColor = Color.red;
        }
        
        Debug.Log($"Shot Result: {feedbackText} | Quality: {quality:F2} | Cursor Position: {cursorPosition:P1} | Final Precision: {_pokemonPlayer.shootPrecision * quality:F1}");
    }

    private void CancelShootingMode()
    {
        // Simple fade out si annulé
        if (isShootingMode)
        {
            _pokemonPlayer.StartCoroutine(SimpleFadeOut());
        }
    }
    
    // === JUICE EFFECTS === //
    
    private void SetupJuiceEffects()
    {
        // Essayer de récupérer le CanvasGroup sur l'UI principal
        _sliderCanvasGroup = _pokemonPlayer.shootingUi.GetComponent<CanvasGroup>();
        if (_sliderCanvasGroup == null)
        {
            _sliderCanvasGroup = _pokemonPlayer.shootingUi.AddComponent<CanvasGroup>();
        }
        
        // Sauvegarder la position originale pour les shakes
        _originalSliderPosition = _pokemonPlayer.shootingUi.transform.localPosition;
        
        // Commencer invisible
        _sliderCanvasGroup.alpha = 0f;
        
        Debug.Log("Juice effects setup complete!");
    }
    
    private IEnumerator FadeInSlider()
    {
        Debug.Log("Starting fade in animation");
        
        float duration = 0.3f; // Plus long pour voir l'effet
        float elapsed = 0f;
        
        // Commencer petit
        _pokemonPlayer.shootingUi.transform.localScale = Vector3.one * 0.5f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            
            // Fade in 
            if (_sliderCanvasGroup != null)
            {
                _sliderCanvasGroup.alpha = progress;
            }
            
            // Scale up avec bounce
            float scale = Mathf.Lerp(0.5f, 1.2f, progress);
            if (progress > 0.7f)
            {
                // Petit bounce à la fin
                scale = Mathf.Lerp(1.2f, 1f, (progress - 0.7f) / 0.3f);
            }
            _pokemonPlayer.shootingUi.transform.localScale = Vector3.one * scale;
            
            yield return null;
        }
        
        // Finaliser
        if (_sliderCanvasGroup != null)
        {
            _sliderCanvasGroup.alpha = 1f;
        }
        _pokemonPlayer.shootingUi.transform.localScale = Vector3.one;
        
        Debug.Log("Fade in complete!");
    }
    
    private IEnumerator ShakeAndFadeOut(float shootingQuality)
    {
        Debug.Log($"Starting shake and fade out - Quality: {shootingQuality}");
        
        // Intensité du shake selon la qualité (plus visible)
        float shakeIntensity;
        float shakeDuration;
        
        if (shootingQuality >= 1.2f) // Perfect
        {
            shakeIntensity = 20f; // Plus visible
            shakeDuration = 0.2f;
        }
        else if (shootingQuality >= 1.0f) // Excellent
        {
            shakeIntensity = 30f;
            shakeDuration = 0.25f;
        }
        else if (shootingQuality >= 0.7f) // Good
        {
            shakeIntensity = 40f;
            shakeDuration = 0.3f;
        }
        else if (shootingQuality >= 0.4f) // Off target
        {
            shakeIntensity = 60f; // Très visible
            shakeDuration = 0.4f;
        }
        else // Brick
        {
            shakeIntensity = 100f; // ÉNORME shake
            shakeDuration = 0.5f;
        }
        
        Debug.Log($"Shake intensity: {shakeIntensity}, Duration: {shakeDuration}");
        
        // Phase 1: Shake intense
        float shakeElapsed = 0f;
        while (shakeElapsed < shakeDuration)
        {
            shakeElapsed += Time.deltaTime;
            
            // Shake constant et visible
            Vector3 shakeOffset = new Vector3(
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity),
                0f
            );
            
            _pokemonPlayer.shootingUi.transform.localPosition = _originalSliderPosition + shakeOffset;
            
            yield return null;
        }
        
        // Remettre à la position originale
        _pokemonPlayer.shootingUi.transform.localPosition = _originalSliderPosition;
        
        Debug.Log("Shake complete, starting fade out");
        
        // Phase 2: Fade out visible
        float fadeDuration = 0.4f; // Plus long
        float fadeElapsed = 0f;
        
        while (fadeElapsed < fadeDuration)
        {
            fadeElapsed += Time.deltaTime;
            float progress = fadeElapsed / fadeDuration;
            
            // Fade out avec scale down
            if (_sliderCanvasGroup != null)
            {
                _sliderCanvasGroup.alpha = 1f - progress;
            }
            
            float scale = Mathf.Lerp(1f, 0.3f, progress);
            _pokemonPlayer.shootingUi.transform.localScale = Vector3.one * scale;
            
            yield return null;
        }
        
        Debug.Log("Fade out complete!");
        
        // Finaliser
        FinishShootingMode();
    }
    
    private IEnumerator SimpleFadeOut()
    {
        Debug.Log("Starting simple fade out");
        
        float fadeDuration = 0.3f;
        float elapsed = 0f;
        float startAlpha = _sliderCanvasGroup != null ? _sliderCanvasGroup.alpha : 1f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / fadeDuration;
            
            if (_sliderCanvasGroup != null)
            {
                _sliderCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, progress);
            }
            
            // Scale down aussi
            float scale = Mathf.Lerp(1f, 0.5f, progress);
            _pokemonPlayer.shootingUi.transform.localScale = Vector3.one * scale;
            
            yield return null;
        }
        
        FinishShootingMode();
    }
    
    private void FinishShootingMode()
    {
        Debug.Log("Finishing shooting mode");
        
        isShootingMode = false;
        _timeAt100Percent = 0f;
        _pokemonPlayer.shootingUi.SetActive(false);
        _pokemonPlayer.speed = speed;
        
        // Reset des transformations
        _pokemonPlayer.shootingUi.transform.localPosition = _originalSliderPosition;
        _pokemonPlayer.shootingUi.transform.localScale = Vector3.one;
        if (_sliderCanvasGroup != null)
        {
            _sliderCanvasGroup.alpha = 1f;
        }
    }

    public void PassBall()
    {
        PokemonPlayer target = GetTargetAllie();

        if (target == null)
        {
            Debug.LogWarning("Aucun coéquipier autre que soi");
            return;
        }

        _pokemonPlayer.Team.SetControlledPlayer(target);

        _pokemonPlayer.LoseBall();
        BasketBallManager.Instance.PassTo(target.transform);
    }

    private PokemonPlayer GetTargetAllie()
    {
        float h = _pokemonPlayer.ControlledByPlayer1 ? Input.GetAxis("HorizontalJoystick1") : Input.GetAxis("HorizontalJoystick2");
        float v = _pokemonPlayer.ControlledByPlayer1 ? Input.GetAxis("VerticalJoystick1") : Input.GetAxis("VerticalJoystick2");
        Vector2 inputDir = new Vector2(h, v);
        Vector3 myPosition = _pokemonPlayer.transform.position;

        Vector3? direction = null;
        if (inputDir.sqrMagnitude >= 0.1f)
        {
            direction = new Vector3(inputDir.x, 0, inputDir.y).normalized;
        }

        PokemonPlayer bestTargetInDirection = null;
        float bestScoreInDirection = float.MaxValue;

        PokemonPlayer closestAlly = null;
        float closestDistance = float.MaxValue;

        float maxAngle = 60f;
        int teammateCount = 0;

        foreach (var ally in _pokemonPlayer.Team.pokeTeam)
        {
            if (ally == _pokemonPlayer) continue;
            teammateCount++;

            Vector3 toAlly = ally.transform.position - myPosition;
            float distance = toAlly.sqrMagnitude;

            // Meilleur au global
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestAlly = ally;
            }

            // Meilleur dans la direction donnée
            if (direction.HasValue)
            {
                float angle = Vector3.Angle(direction.Value, toAlly);
                if (angle > maxAngle) continue;

                if (distance < bestScoreInDirection)
                {
                    bestScoreInDirection = distance;
                    bestTargetInDirection = ally;
                }
            }
        }

        if (teammateCount == 0)
            return null;

        return bestTargetInDirection ?? closestAlly;
    }
}