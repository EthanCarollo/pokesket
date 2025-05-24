using UnityEngine;

public class AttackState : IPokemonPlayerState
{
    private PokemonPlayer _pokemonPlayer;
    
    public AttackState(PokemonPlayer pokemonPlayer)
    {
        _pokemonPlayer = pokemonPlayer;
    }

    public void Update()
    {
        if(!_pokemonPlayer.hasBall)
        {
            _pokemonPlayer.UpdateState(new DefenseState(_pokemonPlayer));
            return;
        }
        
        _pokemonPlayer.HandleMovement();
        if (Input.GetKeyDown(KeyCode.JoystickButton1)) // B on Xbox
        {
            LaunchBall();
        }
    }
    
    public void LaunchBall()
    {
        var rim = _pokemonPlayer.team == BasketTeam.Blue 
            ? GameManager.Instance.basketRimRed.innerRim 
            : GameManager.Instance.basketRimBlue.innerRim;
        GameManager.Instance.InstanciedBasketBall.GoDirectlyIn(rim.transform.position);
        GameManager.Instance.currentBasketBallHolder = null;
    }
}