using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPosition : MonoBehaviour
{
    public Room ParentRoom;
    public int roomPosIndex; //index of roomPos in room

    //  Spawned Objects so we can delete at runtime
    public GameObject _spawnedTopWall = null;
    public GameObject _spawnedRightWall = null;
    public GameObject _spawnedBottomWall = null;
    public GameObject _spawnedLeftWall = null;
    public GameObject _spawnedTire = null;
    public GameObject _spawnedSystem = null;

    //Pathfinding
    public int _gCost; // gCost represents the Distance from our Starting Point to our current Tile.
    public int _hCost; // hCost represents the Distance from our Target Point to our Current Tile.
    public int FCost
    {
        get
        {
            return _gCost + _hCost;
        }
    }
    public RoomPosition _pathfindParent; //important for the pathfinding process
    public int _xPos;
    public int _yPos;
    //relative coordinates of the rooms positions compared to the start of the room, because Im too stupid to figure out how to do it in a matrix
    public int _xRel;
    public int _yRel;


    public enum DamageDirection
    {
        Left, Right, Down, Up, None
    }

    public void DamageWall(DamageDirection damageDir)
    {
        if (damageDir == DamageDirection.None)
        {
            return;
        }
        else
        {
            if (damageDir == DamageDirection.Left)
            {
                if (!_spawnedLeftWall) return;
                ParticleSystem splintersVFX = Instantiate(Resources.Load(GS.Effects("WoodSplintersVFX"), typeof(ParticleSystem)) as ParticleSystem, _spawnedLeftWall.transform, false);
                HM.RotateLocalTransformToAngle(splintersVFX.transform, new Vector3(0, 0, 90));
                splintersVFX.transform.localPosition = new Vector3(-0.25f, 0, 0);
                splintersVFX.Play();

                if (!_spawnedSystem) return;
                ParticleSystem scrapVFX = Instantiate(Resources.Load(GS.Effects("MetalScrapVFX"), typeof(ParticleSystem)) as ParticleSystem, _spawnedLeftWall.transform, false);
                HM.RotateLocalTransformToAngle(scrapVFX.transform, new Vector3(0, 0, 90));
                scrapVFX.transform.localPosition = new Vector3(-0.25f, 0, 0);
                scrapVFX.Play();
            }
            else if (damageDir == DamageDirection.Right)
            {
                if (!_spawnedRightWall) return;
                ParticleSystem splintersVFX = Instantiate(Resources.Load(GS.Effects("WoodSplintersVFX"), typeof(ParticleSystem)) as ParticleSystem, _spawnedRightWall.transform, false);
                HM.RotateLocalTransformToAngle(splintersVFX.transform, new Vector3(0, 0, -90));
                splintersVFX.transform.localPosition = new Vector3(0.25f, 0, 0);
                splintersVFX.Play();

                if (!_spawnedSystem) return;
                ParticleSystem scrapVFX = Instantiate(Resources.Load(GS.Effects("MetalScrapVFX"), typeof(ParticleSystem)) as ParticleSystem, _spawnedRightWall.transform, false);
                HM.RotateLocalTransformToAngle(scrapVFX.transform, new Vector3(0, 0, -90));
                scrapVFX.transform.localPosition = new Vector3(0.25f, 0, 0);
                scrapVFX.Play();
            }
            else if (damageDir == DamageDirection.Up)
            {
                if (!_spawnedTopWall) return;
                ParticleSystem splintersVFX = Instantiate(Resources.Load(GS.Effects("WoodSplintersVFX"), typeof(ParticleSystem)) as ParticleSystem, _spawnedTopWall.transform, false);
                HM.RotateLocalTransformToAngle(splintersVFX.transform, new Vector3(0, 0, 0));
                splintersVFX.transform.localPosition = new Vector3(0, 0.25f, 0);
                splintersVFX.Play();

                if (!_spawnedSystem) return;
                ParticleSystem scrapVFX = Instantiate(Resources.Load(GS.Effects("MetalScrapVFX"), typeof(ParticleSystem)) as ParticleSystem, _spawnedTopWall.transform, false);
                HM.RotateLocalTransformToAngle(scrapVFX.transform, new Vector3(0, 0, 0));
                scrapVFX.transform.localPosition = new Vector3(0, 0.25f, 0);
                scrapVFX.Play();
            }
            else
            {
                if (!_spawnedBottomWall) return;
                ParticleSystem splintersVFX = Instantiate(Resources.Load(GS.Effects("WoodSplintersVFX"), typeof(ParticleSystem)) as ParticleSystem, _spawnedBottomWall.transform, false);
                HM.RotateLocalTransformToAngle(splintersVFX.transform, new Vector3(0, 0, 180));
                splintersVFX.transform.localPosition = new Vector3(0, -0.25f, 0);
                splintersVFX.Play();

                if (!_spawnedSystem) return;
                ParticleSystem scrapVFX = Instantiate(Resources.Load(GS.Effects("MetalScrapVFX"), typeof(ParticleSystem)) as ParticleSystem, _spawnedBottomWall.transform, false);
                HM.RotateLocalTransformToAngle(scrapVFX.transform, new Vector3(0, 0, 180));
                scrapVFX.transform.localPosition = new Vector3(0, -0.25f, 0);
                scrapVFX.Play();
            }

        }
    }
}
