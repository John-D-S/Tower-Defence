using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKillable
{
    float MaxHealth { get; set; }
    float Health { get; set; }

    void Damage(float amount);
    void Heal(float amount);
    void Die();
}