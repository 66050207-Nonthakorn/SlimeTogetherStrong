using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SlimeTogetherStrong.Engine.Managers;

public class SkillManager
{
    public static SkillManager Instance { get; private set; } = new SkillManager();

    private float _skillCooldownDuration = 5.0f; // 5 seconds cooldown
    private float _currentCooldown = 0f;
    private int _skillManaCost = 30; // Mana cost for skill
    
    public bool IsOnCooldown => _currentCooldown > 0f;
    public float CooldownRemaining => _currentCooldown;
    public float CooldownDuration => _skillCooldownDuration;
    public float CooldownProgress => IsOnCooldown ? _currentCooldown / _skillCooldownDuration : 0f;
    public int SkillManaCost => _skillManaCost;

    private SkillManager() { }

    public void Update(GameTime gameTime)
    {
        // Update cooldown timer
        if (_currentCooldown > 0f)
        {
            _currentCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_currentCooldown < 0f)
            {
                _currentCooldown = 0f;
            }
        }

        // Check for Q key press to use skill
        if (InputManager.Instance.IsKeyPressed(Keys.Q))
        {
            UseSkill();
        }
    }

    private void UseSkill()
    {
        // Check if skill is off cooldown
        if (!IsOnCooldown)
        {
            // Try to find player and use mana
            var player = SlimeTogetherStrong.Game.GameScene.GetPlayer();
            if (player != null && player.ManaComponent != null)
            {
                if (player.ManaComponent.UseMana(_skillManaCost))
                {
                    // Activate skill (dummy - does nothing for now)
                    System.Console.WriteLine($"Skill activated! Used {_skillManaCost} mana.");
                    
                    // Start cooldown
                    _currentCooldown = _skillCooldownDuration;
                }
                else
                {
                    System.Console.WriteLine("Not enough mana!");
                }
            }
            else
            {
                // Fallback if player not found
                System.Console.WriteLine("Skill activated! (No mana system found)");
                _currentCooldown = _skillCooldownDuration;
            }
        }
        else
        {
            System.Console.WriteLine($"Skill on cooldown: {_currentCooldown:F1}s remaining");
        }
    }

    public void ResetCooldown()
    {
        _currentCooldown = 0f;
    }
}
