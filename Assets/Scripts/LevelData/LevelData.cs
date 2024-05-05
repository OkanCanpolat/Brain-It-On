
using UnityEngine;
[CreateAssetMenu (fileName = "Level Data")]
public class LevelData : ScriptableObject
{
    public int LevelIndex;
    public float goalTimeSeconds;
    public int goalDrawCount;
    public float SolvedTime;
    public int SolvedDrawCount;
}
