﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

public interface IServer
{
    void HitBall(int playerId, Vector2 direction, float force);

    void LeaveGame();

    [CanBeNull]
    Event NextEvent();
}

