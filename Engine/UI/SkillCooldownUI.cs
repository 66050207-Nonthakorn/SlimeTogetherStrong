using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlimeTogetherStrong.Engine.Components;
using SlimeTogetherStrong.Engine.Managers;

namespace SlimeTogetherStrong.Engine.UI;

public class SkillCooldownUI : UIElement
{
    private SkillManager _skillManager;
    private Texture2D _activeTexture;
    private Texture2D _inactiveTexture;
    private SpriteFont _font;
    
    private GameObject _iconObject;
    private GameObject _keybindObject;
    private GameObject _cooldownTextObject;
    
    private Vector2 _iconSize;
    private Vector2 _basePosition;

    public SkillCooldownUI(Vector2 position, Vector2 iconSize)
    {
        _basePosition = position;
        _iconSize = iconSize;
        Position = position;
        
        _skillManager = SkillManager.Instance;
        
        // Get textures from ResourceManager
        _activeTexture = ResourceManager.Instance.GetTexture("Skill_Icon_Active");
        _inactiveTexture = ResourceManager.Instance.GetTexture("Skill_Icon_Inactive");
        _font = ResourceManager.Instance.GetFont("DefaultFont");
    }

    public override void Initialize()
    {
        base.Initialize();
        
        // Create icon game object
        _iconObject = new GameObject();
        _iconObject.Position = _basePosition;
        
        var iconRenderer = _iconObject.AddComponent<SpriteRenderer>();
        iconRenderer.Texture = _activeTexture;
        
        // Calculate scale to fit the desired size
        if (_activeTexture != null)
        {
            float scaleX = _iconSize.X / _activeTexture.Width;
            float scaleY = _iconSize.Y / _activeTexture.Height;
            _iconObject.Scale = new Vector2(scaleX, scaleY);
        }
        
        AddChild(_iconObject);
        
        // Create keybind text above the icon
        _keybindObject = new GameObject();
        _keybindObject.Position = new Vector2(
            _basePosition.X + _iconSize.X / 2,
            _basePosition.Y - 5
        );
        _keybindObject.Scale = new Vector2(1.5f, 1.5f); // Increase font size by 1.5x
        
        var keybindText = _keybindObject.AddComponent<Text>();
        keybindText.Font = _font;
        keybindText.Content = "Q";
        keybindText.Color = Color.White;
        keybindText.LayerDepth = 0f;
        
        // Center the text
        if (_font != null)
        {
            var textSize = _font.MeasureString("Q");
            keybindText.Origin = textSize / 2;
        }
        
        AddChild(_keybindObject);
        
        // Create cooldown text (centered on icon)
        _cooldownTextObject = new GameObject();
        _cooldownTextObject.Position = new Vector2(
            _basePosition.X + _iconSize.X / 2,
            _basePosition.Y + _iconSize.Y / 2
        );
        
        var cooldownText = _cooldownTextObject.AddComponent<Text>();
        cooldownText.Font = _font;
        cooldownText.Content = "";
        cooldownText.Color = Color.White;
        cooldownText.LayerDepth = 0f;
        cooldownText.Origin = Vector2.Zero; // Will be set dynamically
        
        AddChild(_cooldownTextObject);
        
        // Subscribe to placement mode changes
        _skillManager.OnPlacementModeChanged += OnPlacementModeChanged;
    }
    
    private void OnPlacementModeChanged(bool isInPlacementMode)
    {
        // Highlight icon when in placement mode
        if (_iconObject != null)
        {
            var renderer = _iconObject.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                // Tint the icon yellow when in placement mode
                renderer.Tint = isInPlacementMode ? Color.Yellow : Color.White;
            }
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        // Early return if not initialized yet
        if (_iconObject == null || _cooldownTextObject == null)
            return;
        
        // Update icon based on cooldown state
        var iconRenderer = _iconObject.GetComponent<SpriteRenderer>();
        if (iconRenderer != null)
        {
            if (_skillManager.IsOnCooldown)
            {
                iconRenderer.Texture = _inactiveTexture;
            }
            else
            {
                iconRenderer.Texture = _activeTexture;
            }
        }
        
        // Update cooldown text
        var cooldownText = _cooldownTextObject.GetComponent<Text>();
        if (cooldownText != null)
        {
            if (_skillManager.IsOnCooldown)
            {
                float remaining = _skillManager.CooldownRemaining;
                cooldownText.Content = $"{remaining:F1}";
                
                // Center the text
                if (_font != null)
                {
                    var textSize = _font.MeasureString(cooldownText.Content);
                    cooldownText.Origin = textSize / 2;
                }
            }
            else
            {
                cooldownText.Content = "";
            }
        }
    }
}
