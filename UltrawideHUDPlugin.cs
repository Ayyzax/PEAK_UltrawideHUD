using BepInEx;
using BepInEx.Logging;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zorro.Settings;
using SettingsExtender;

namespace Ayzax.UltrawideHUD;

[BepInProcess("PEAK.exe")]
[BepInPlugin("Ayzax.UltrawideHUD", "Ultrawide HUD", "1.0.0")]
[BepInDependency("JSPAPP-Settings_Extender-0.0.1")]
public class UltrawideHUDPlugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    bool _active = false;
    RectTransform _barGroup;
    RectTransform _inventory;

    UltrawideAnchorSetting _anchorSetting;
    UltrawideOffsetSetting _squishSetting;

    public const string SettingsCategory = "HUD";
    const float BarsDefaultX = 70;
    const float BarsFixedX = -890;
    const float BarsSquish = 250;
    const float InventoryDefaultX = 0;
    const float InventoryFixedX = 960;
    const float InventorySquish = 490;

    void Start()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Ultrawide HUD is loaded!");

        SettingsRegistry.Register(SettingsCategory);
        
        _anchorSetting = new UltrawideAnchorSetting();
        _anchorSetting.RegisterListener(HandleSettingChange);
        SettingsHandler.Instance.AddSetting(_anchorSetting);

        _squishSetting = new UltrawideOffsetSetting();
        _squishSetting.RegisterListener(HandleSettingChange);
        SettingsHandler.Instance.AddSetting(_squishSetting);
        
        SceneManager.sceneLoaded += HandleLevelLoad;
    }

    void HandleLevelLoad(Scene scene, LoadSceneMode loadMode)
    {
        _active = scene.name != "Title" && scene.name != "Pretitle";

        if (!_active)
        {
            return;
        }

        _barGroup = GameObject.Find("/GAME/GUIManager/Canvas_HUD/BarGroup").GetComponent<RectTransform>();
        _inventory = GameObject.Find("/GAME/GUIManager/Canvas_HUD/Inventory").GetComponent<RectTransform>();

        AdjustUltrawideHUD();
    }

    void HandleSettingChange(Setting setting)
    {
        AdjustUltrawideHUD();
    }

    public void AdjustUltrawideHUD()
    { 
        if (!_active)
        {
            return;
        }

        float anchorPercent = _anchorSetting.Value;
        float hudSquish = _squishSetting.Value;

        float barAnchor = anchorPercent * 0.5f;
        _barGroup.anchorMin = new Vector2(barAnchor, 0);
        _barGroup.anchorMax = new Vector2(barAnchor, 0);
        _barGroup.anchoredPosition = new Vector2(Mathf.Lerp(BarsDefaultX, BarsFixedX, anchorPercent) + (hudSquish * BarsSquish), _barGroup.anchoredPosition.y);

        float inventoryAnchor = 0.5f + ((1 - anchorPercent) * 0.5f);
        _inventory.anchorMin = new Vector2(inventoryAnchor, 0);
        _inventory.anchorMax = new Vector2(inventoryAnchor, 0);
        _inventory.anchoredPosition = new Vector2(Mathf.Lerp(InventoryDefaultX, InventoryFixedX, anchorPercent) - (hudSquish * InventorySquish), _inventory.anchoredPosition.y);
    }
}

public class UltrawideAnchorSetting : FloatSetting, IExposedSetting
{
    public string GetDisplayName() => "HUD Anchoring";
    public string GetCategory() => SettingsRegistry.GetPageId(UltrawideHUDPlugin.SettingsCategory);
    protected override float GetDefaultValue() => 1f;
    protected override float2 GetMinMaxValue() => new float2(0, 1);
    public override void ApplyValue() { }
}

public class UltrawideOffsetSetting : FloatSetting, IExposedSetting
{
    public string GetDisplayName() => "HUD Squish";
    public string GetCategory() => SettingsRegistry.GetPageId(UltrawideHUDPlugin.SettingsCategory);
    protected override float GetDefaultValue() => 0f;
    protected override float2 GetMinMaxValue() => new float2(0, 1);
    public override void ApplyValue() { }
}