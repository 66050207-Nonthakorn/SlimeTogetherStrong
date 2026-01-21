using Microsoft.Xna.Framework;

namespace SlimeTogetherStrong.Engine.Components.Physics;

public abstract class BoxCollider : Collider
{
    public Rectangle Bounds { get; set; }

    public Vector2 GetCenter()
    {
        return base.GameObject.Position +
                base.Offset +
                new Vector2(this.Bounds.Width / 2f, this.Bounds.Height / 2f);
    }

    public override bool IsIntersect(Collider other)
    {
        return other.IsIntersect(this);
    }

    // Box vs Circle collision (Reuse from CircleCollider)
    public override bool IsIntersect(CircleCollider other)
    {
        return other.IsIntersect(this);
    }

    // Box vs Box collision        
    public override bool IsIntersect(BoxCollider other)
    {
        return this.Bounds.Intersects(other.Bounds);
    }
}
