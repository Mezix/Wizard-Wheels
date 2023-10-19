using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    public GameObject _bossUIObjects;
    public Image _fill;
    public Text _bossName;
    public EnemyTankController tankController;
    public int tankControllerID;

    private void Start()
    {
        ShowBossUI(false);
        Events.instance.EnemyTankDestroyed += CheckBossKilled;
    }
    private void Update()
    {
        if(tankController)
        {
            _fill.fillAmount = tankController.THealth._currentHealth / (float) tankController.THealth._maxHealth;
        }
    }
    public void Init(EnemyTankController enemyTank)
    {
        ShowBossUI(true);
        _bossName.text = enemyTank._tankName;
        tankController = enemyTank;
        tankControllerID = enemyTank.GetInstanceID();
        _fill.fillAmount = 1;
    }
    public void CheckBossKilled(EnemyTankController TankControllerID)
    {
        if(tankControllerID == TankControllerID.GetInstanceID())
        {
            tankController = null;
            ShowBossUI(false);
        }
    }
    public void ShowBossUI(bool show)
    {
        _bossUIObjects.SetActive(show);
    }
}
