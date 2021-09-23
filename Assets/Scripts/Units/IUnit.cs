using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnit
{
    string UnitName { get; set; }
    string UnitClass { get; set; }
    string UnitHealth { get; set; }
    float UnitMoveSpeed { get; set; }
    SpriteRenderer UnitSprite { get; set; }
    bool UnitSelected { get; set; }
    bool UnitIsMoving { get; set; }

    void InitUnit();
}
