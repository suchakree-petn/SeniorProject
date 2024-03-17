[System.Serializable]
public class InGameSettingData : ISaveData
{
    public MouseSensitive_ThirdPerson MouseSensitive_ThirdPerson;
    public MouseSensitive_Focus MouseSensitive_Focus;


    public InGameSettingData()
    {
        MouseSensitive_ThirdPerson = new(5, 3);
        MouseSensitive_Focus = new(10, 10);
        
    }
}

[System.Serializable]
public struct MouseSensitive_ThirdPerson
{
    public float x;
    public float y;

    public MouseSensitive_ThirdPerson(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
}

[System.Serializable]
public struct MouseSensitive_Focus
{
    public float x;
    public float y;

    public MouseSensitive_Focus(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
}
