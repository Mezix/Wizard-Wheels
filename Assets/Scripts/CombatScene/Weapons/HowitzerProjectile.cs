using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowitzerProjectile : AProjectile
{
    public List<Room> _allRoomsInExplosion = new List<Room>();

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }
}
