using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// add this to things you need to have health
/// </summary>
public interface IKillable
{
    float MaxHealth { get; set; }
    float Health { get; set; }

    void Damage(float amount);
    void Heal(float amount);
    void Die();
}