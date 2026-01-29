using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlimeTogetherStrong.Engine.Components;
using SlimeTogetherStrong.Engine.Managers;

namespace SlimeTogetherStrong.Engine.UI;

public class TimerUI : UIElement
{
    private GameObject _textObject;
    private Text _text;
    private float _elapsedTime;

    public TimerUI(Vector2 position)
    {
        Position = position;
    }

    public override void Initialize()
    {
        base.Initialize();
        
        _textObject = new GameObject();
        _textObject.Position = Position;
        _textObject.Scale = new Vector2(1.2f, 1.2f);
        
        _text = _textObject.AddComponent<Text>();
        _text.Font = ResourceManager.Instance.GetFont("DefaultFont");
        _text.Content = "00:00";
        _text.Color = Color.White;
        
        // Center the text horizontally
        var textSize = _text.MeasureText();
        _text.Origin = new Vector2(textSize.X / 2, 0);
        
        AddChild(_textObject);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        // Update elapsed time
        _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Update text display
        if (_text != null)
        {
            int minutes = (int)(_elapsedTime / 60);
            int seconds = (int)(_elapsedTime % 60);
            _text.Content = $"{minutes:D2}:{seconds:D2}";
            
            // Recenter the text
            var textSize = _text.MeasureText();
            _text.Origin = new Vector2(textSize.X / 2, 0);
        }
    }

    public float GetElapsedTime() => _elapsedTime;
    
    public string GetFormattedTime()
    {
        int minutes = (int)(_elapsedTime / 60);
        int seconds = (int)(_elapsedTime % 60);
        return $"{minutes:D2}:{seconds:D2}";
    }
    
    public void ResetTimer()
    {
        _elapsedTime = 0f;
    }
}
