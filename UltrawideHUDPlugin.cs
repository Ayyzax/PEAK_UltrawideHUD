using BepInEx;
using BepInEx.Logging;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zorro.Settings;
using SettingsExtender;

namespace Ayzax.UltrawideHUD;

[BepInProcess("PEAK.exe")]
[BepInPlugin("Ayzax.UltrawideHUD", "Ultrawide HUD", "1.0.1")]
[BepInDependency("SettingsExtender")]
public class UltrawideHUDPlugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    bool _active = false;

    RectTransform _barGroup;
    RectTransform _inventory;
    RectTransform _prompts;

    UltrawideAnchorSetting _anchorSetting;
    UltrawideOffsetSetting _squishSetting;

    public const string SettingsCategory = "HUD";

    const float HalfScreenWidth = 1920 / 2;

    const float BarsDefaultX = 70;
    const float BarsFixedX = BarsDefaultX - HalfScreenWidth;
    const float BarsSquish = 250;

    const float InventoryDefaultX = 0;
    const float InventoryFixedX = HalfScreenWidth;
    const float InventorySquish = 490;

    const float PromptDefaultX = -52.7703f;
    const float PromptFixedX = HalfScreenWidth + PromptDefaultX;
    const float PromptSquish = 490;

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
        _prompts = GameObject.Find("/GAME/GUIManager/Canvas_HUD/Prompts/ItemPromptLayout").GetComponent<RectTransform>();
        _prompts.SetParent(_prompts.parent.parent);

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
        float rightAnchor = 0.5f + ((1 - anchorPercent) * 0.5f);
        float leftAnchor = anchorPercent * 0.5f;

        float hudSquish = _squishSetting.Value;


        _barGroup.anchorMin = new Vector2(leftAnchor, 0);
        _barGroup.anchorMax = new Vector2(leftAnchor, 0);
        _barGroup.anchoredPosition = new Vector2(Mathf.Lerp(BarsDefaultX, BarsFixedX, anchorPercent) + (hudSquish * BarsSquish), _barGroup.anchoredPosition.y);

        _inventory.anchorMin = new Vector2(rightAnchor, 0);
        _inventory.anchorMax = new Vector2(rightAnchor, 0);
        _inventory.anchoredPosition = new Vector2(Mathf.Lerp(InventoryDefaultX, InventoryFixedX, anchorPercent) - (hudSquish * InventorySquish), _inventory.anchoredPosition.y);

        _prompts.anchorMin = new Vector2(rightAnchor, 0);
        _prompts.anchorMax = new Vector2(rightAnchor, 1);
        _prompts.anchoredPosition = new Vector2(Mathf.Lerp(PromptDefaultX, PromptFixedX, anchorPercent) - (hudSquish * PromptSquish), _prompts.anchoredPosition.y);
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