using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKillable
{
    float MaxHealth { get; set; }
    float Health { get; set; }

    void Damage();
    void Kill();
}
