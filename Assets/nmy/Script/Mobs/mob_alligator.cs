using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mob_alligator : Mobs
{
    // Start is called before the first frame update

    public override void Start()
    {
        base.Start();
        HP = 1584;
        attack = 55;
        Exp = 14;
        Money = 158;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
