using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PoolableObject;

[System.Serializable]
public class ObjectPoolItem //this is an internal class, which we fill with prefabs of our projectiles
{
    public int _amountToPool; //how many projectiles should be created each time we expand the pool
    public PoolableObject poolableObject; //the actual prefab of the projectile
}
public class ObjectPool : MonoBehaviour
{
    private List<PoolableObject> _objectPool; //where all our different bullets are stored

    public List<ObjectPoolItem> _objectsToPool; //the list of all our bullets we need to instantiate and keep track of
    public static ObjectPool Instance { get; private set; } //the reference to the pool itself

    private void Awake()
    {
        Instance = this; //set the reference to ourselves
    }
    void Start()
    {
        _objectPool = new List<PoolableObject>(); //init the list
        foreach(ObjectPoolItem item in _objectsToPool) //at the start of the game make sure we create a few of each projectile
        {
            GrowPoolable(item.poolableObject);
        }
    }
    private void GrowPoolable(PoolableObject poolablePrefab)
    {
        for (int i = 0; i < 10; i++)
        {
            var poolable = Instantiate(poolablePrefab); //create a bullet of this kind
            poolable.transform.SetParent(transform); //add all the created bullets to the pool so they aren't loose in the scene
            AddToPool(poolable);
        }
    }
    public void AddToPool(PoolableObject projectile)
    {
        projectile.gameObject.SetActive(false); //disable the gameobjects so we don't need to see them
        _objectPool.Add(projectile); //add the projectile to the entire pool
    }

    public GameObject GetProjectileFromPool(PoolableType poolableType) //take the tag of the prefab we need, and return an instance of it
    {
        for (int i = 0; i < _objectPool.Count; i++) //go through the entire pool of all bullets to find one we need
        {
            if (!_objectPool[i].gameObject.activeInHierarchy && _objectPool[i]._poolableType.Equals(poolableType)) //only pull objects which are currently inactive, and have the same tags
            {
                _objectPool[i].gameObject.SetActive(true); //if we found the correct bullet, activate it
                return _objectPool[i].gameObject;          //...and now return it!
            }
        }

        //this is only called if we havent found a correct bullet

        foreach (ObjectPoolItem item in _objectsToPool) //check through all possible types of bullets
        {
            if (item.poolableObject._poolableType.Equals(poolableType)) //check the tag of the prefab to the one we got
            {
                GrowPoolable(item.poolableObject); //create more of the prefab
                return GetProjectileFromPool(poolableType); // now get the new projectile, since we created some
            }
        }
        return null;
    }
}