[System.Serializable]
public class InGameSettingData : ISaveData
{
    public MouseSensitive MouseSensitive;

    public InGameSettingData()
    {
        MouseSensitive = new(2, 2);
    }

}
[System.Serializable]
public struct MouseSensitive
{
    public float x;
    public float y;

    public MouseSensitive(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
}
