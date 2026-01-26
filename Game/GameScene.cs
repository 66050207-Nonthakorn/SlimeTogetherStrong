using System;
using SlimeTogetherStrong.Engine;
using SlimeTogetherStrong.Engine.Managers;
using SlimeTogetherStrong.Engine.UI;

namespace SlimeTogetherStrong.Game;

class GameScene : Scene
{
    public override void Load()
    {
        GameObject button = new()
        {
            Position = new(300, 200)
        };

        button.AddComponent<Button>();
        button.GetComponent<Button>().Text = "Start Game";
        button.GetComponent<Button>().Font = ResourceManager.Instance.GetFont("DefaultFont");
        button.GetComponent<Button>().OnClick = () =>
        {
            Console.WriteLine("Start Game button clicked!");
        };

        AddGameObject(button);
    }
}