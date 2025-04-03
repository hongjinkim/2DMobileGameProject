using System;
using UnityEngine;

public class PlayerControl : CharacterBase
{
    public static event Action OnDie = delegate { };


    protected override void HandleEvent(string eventName)
    {

    }

    protected override void Finish(EActType ActType)
    {

    }
}
