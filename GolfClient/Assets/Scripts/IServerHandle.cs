using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

public interface IServer
{
    int CurrentPlayerId { get; }

    void HitBall(int playerId, float direction, float force);

    void LeaveGame();

    void NextMove();

    [CanBeNull]
    Event NextEvent();
}

