using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    // Start is called before the first frame update
    protected override void Die()
    {
        base.Die();
        Player.instance.BossDefeated();
    }
}
