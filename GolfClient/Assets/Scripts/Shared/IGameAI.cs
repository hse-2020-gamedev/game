
using UnityEngine;
using Utils;

public interface IGameAI
{
    void MakeTurn();
}

public class DummyAI : IGameAI
{
    private GameLogic _gameLogic;
    private int _playerId;
    
    public DummyAI(GameLogic gameLogic, int playerId)
    {
        _gameLogic = gameLogic;
        _playerId = playerId;
    }
    
    public void MakeTurn()
    {
        var ballPosition = _gameLogic.PlayerBalls[_playerId].transform.position;
        var targetPosition = _gameLogic.TargetHole.transform.position;
        _gameLogic.HitBall(_playerId, ballPosition.xz().LookAt(targetPosition.xz()), 0.5f);
    }
}
