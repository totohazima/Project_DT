using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Pathfinding.Util;
using Pathfinding;
public class TestTargetSetter : VersionedMonoBehaviour
{
    public Character setCharacter;

    void Update()
    {
        UpdateTargetPosition();  
    }

    public void UpdateTargetPosition()
    {
        if (setCharacter.targetUnit != transform)
        {
            setCharacter.targetUnit = transform;
        }
    }
}
