using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeScreen : MonoBehaviour
{
    public List<UIUpgradeField> _upgradeFields = new List<UIUpgradeField>();
    public Button _saveButton;
    public Button _closeWindow;
    public Button _toggleUpgrades;
    public bool _closed;
    public Transform _layoutGroup;
    private void Awake()
    {
        CloseUpgrades();
        _saveButton.onClick.AddListener(() => SaveUpgrades());
        _closeWindow.onClick.AddListener(() => CloseUpgrades());
        _toggleUpgrades.onClick.AddListener(() => ToggleUpgradeScreen());
    }

    public UIUpgradeField CreateUpgradeField()
    {
        GameObject field = Instantiate((GameObject)Resources.Load("UpgradeField"));
        field.transform.parent = _layoutGroup;
        field.transform.localScale = Vector3.one;
        UIUpgradeField upgrade = field.GetComponent<UIUpgradeField>();
        Ref.UI._upgradeScreen._upgradeFields.Add(upgrade);

        return upgrade;
    }
    public void SaveUpgrades()
    {
        Events.instance.SaveUpgrades();
    }
    public void ToggleUpgradeScreen()
    {
        if (_closed) OpenUpgrades();
        else CloseUpgrades();
    }
    public void OpenUpgrades()
    {
        gameObject.SetActive(true);

        //  Set everything back to temp without saving!
        _closed = false;
    }
    public void CloseUpgrades()
    {
        gameObject.SetActive(false);
        _closed = true;
    }
}
