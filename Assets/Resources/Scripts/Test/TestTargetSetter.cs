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
        var aiStars = setCharacter.aiPath;
        if (aiStars != null)
        {
            setCharacter.aiPath.destination = transform.position;
        }
    }
}
