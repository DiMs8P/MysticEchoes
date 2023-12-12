namespace MysticEchoes.Core.Configuration;

public class Settings
{
    public Settings()
    {
        Player = new PlayerSettings();
    }
    
    public PlayerSettings Player;
}