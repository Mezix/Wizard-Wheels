using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabberArm : ASystem
{
    public bool _armLaunched;
    public Transform _grabberArmCrossbowBody;
    public Transform _chainStartingPos;
    public GrabberClaw _clawScript;
    [SerializeField]
    private Animator _grabberArmAnimator;
    private float grabberSpeed;

    private Vector3 scrapPos;
    private LootCrate scrap;
    private bool scrapCollection;
    public override void Awake()
    {
        base.Awake();
        _armLaunched = false;
        scrapCollection = false;
        grabberSpeed = 10f;
    }
    void Update()
    {
        TryHighlightScrap();
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryLaunchToScrap();
        }
        if (_armLaunched)
        {
            MoveClawToScrapSlowly();
        }
        else
        {
            if (!scrapCollection) RotateToMouse();
            else RetractClawSlowly();
        }
    }
    private void LateUpdate()
    {
        if(Time.timeScale > 0)
        {
            if (scrapCollection) CreateChain();
        }
    }

    private void TryHighlightScrap()
    {
        RaycastHit2D hit = HM.RaycastToMouseCursor();
        if (!hit.collider) return;
        if (hit.collider.TryGetComponent(out LootCrate s))
        {
            s.Highlight();
        }
    }
    private void TryLaunchToScrap()
    {
        if (_armLaunched) return;

        RaycastHit2D hit = HM.RaycastToMouseCursor();
        if (!hit.collider) return;
        if (hit.collider.TryGetComponent(out LootCrate s))
        {
            FireCrossbow();
            scrapPos = s.transform.position;
            _armLaunched = true;
            scrapCollection = true;
            scrap = s;
            s._collecting = true;
        }
    }
    private void RotateToMouse()
    {
        float angle = HM.GetEulerAngle2DBetween(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.position);
        HM.RotateTransformToAngle(_grabberArmCrossbowBody.transform, new Vector3(0, 0, angle));
        HM.RotateTransformToAngle(_clawScript.transform, new Vector3(0, 0, angle));
    }
    private void RotateToScrap()
    {
        if (!scrap) return;
        float angle = HM.GetEulerAngle2DBetween(scrap.transform.position, transform.position);
        HM.RotateTransformToAngle(_grabberArmCrossbowBody.transform, new Vector3(0, 0, angle));
        HM.RotateTransformToAngle(_clawScript.transform, new Vector3(0, 0, angle));
    }
    private void MoveClawToScrapSlowly()
    {
        RotateToScrap();
        _clawScript.transform.parent = null;
        _clawScript._clawAnimator.transform.localPosition = Vector3.zero;

        Vector3 nextPos = (scrapPos - _clawScript.transform.position).normalized * grabberSpeed * Time.deltaTime;
        if (Vector3.Distance(_clawScript.transform.position + nextPos, scrapPos) < 0.5f)
        {
            scrap.PickUpScrap(_clawScript._clawAnimator.transform);
            _armLaunched = false;
            _clawScript.transform.position = scrapPos;
            _clawScript.transform.parent = null;
            _clawScript._clawAnimator.transform.localPosition = Vector3.zero;
        }
        else
        {
            _clawScript.transform.position += nextPos;
        }
    }
    private void RetractClawSlowly()
    {
        Vector3 nextPos = (_grabberArmCrossbowBody.position - _clawScript.transform.position).normalized * grabberSpeed * 1.5f * Time.deltaTime;
        if (Vector3.Distance(_clawScript.transform.position + nextPos, _grabberArmCrossbowBody.position) < 0.5f)
        {
            _clawScript.transform.parent = _grabberArmCrossbowBody;
            _clawScript.transform.localPosition = Vector3.zero;
            _clawScript._clawAnimator.transform.localPosition = new Vector3(0.166f, 0, 0);
            _clawScript._clawAnimator.SetBool("CloseClaw", false);
            if (scrap)
            {
                scrap.DestroyScrapObject();
                scrap = null;
                scrapPos = Vector3.zero;
                scrapCollection = false;
            }
        }
        else
        {
            _clawScript.transform.position += nextPos;
        }
    }
    public void FireCrossbow()
    {
        _grabberArmAnimator.SetTrigger("FireCrossbow");
    }

    //  Chain

    private void CreateChain()
    {
        Sprite chain = Resources.Load(GS.WeaponGraphics("chain_link"), typeof(Sprite)) as Sprite;
        Vector3 clawStartingPos = _clawScript._clawAnimator.transform.position;
        clawStartingPos += (_chainStartingPos.position - clawStartingPos).normalized * 0.1f;
        DottedLine.DottedLine.Instance.DrawDottedLine(_chainStartingPos.position, clawStartingPos, Color.white, chain);
    }
}
