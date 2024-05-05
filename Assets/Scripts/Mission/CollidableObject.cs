using UnityEngine;

public enum CollidableObjectType
{
    Line, YellowBall, Object, MagnetPositive, MagnetNegative
}
public class CollidableObject : MonoBehaviour
{
    public CollidableObjectType Type;
}
