namespace Code.Core.GameStats
{
public class GameStat
{
    private string StatId { get; }
    private int StatValue { get; set; }
    
    public GameStat(string statId, int statValue)
    {
        StatId = statId;
        StatValue = statValue;
    }
    
    public string GetStatId()
    {
        return StatId;
    }
    
    public int GetStatValue()
    {
        return StatValue;
    }

    public void IncreaseStatValue(int amount = 1)
    {
        StatValue += amount;
    }
}
}
