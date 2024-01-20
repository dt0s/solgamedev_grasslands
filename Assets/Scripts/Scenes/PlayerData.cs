[System.Serializable]
public class PlayerData
{
    public float[] position;
    public float[] velocity;
    public float[] camPosition;
    public float currentMaximumHealth;
    public float currentHealth;
    public float damageBoost;
    public string currentScene;
    public int currentMoney;
    // PORTAL LIST:
    // [0] - MainPortal
    // [1] - DesertPortal
    public bool[] portalBools;
    // ITEM LIST:
    // [0] - Rock
    // [0] - Epic shizniz
    public bool[] itemBools;
    // TROPHY LIST:
    // [0] - FireWings
    // [1] - Other Stuff To Come
    public bool[] trophies;

    
    public PlayerData(JumpKing player)
    {
        position = player.savePosition;
        velocity = player.saveVelocity;
        camPosition = player.saveCamPosition;
        currentMaximumHealth = player.saveMaxHealth;
        currentHealth = player.saveCurrentHealth;
        damageBoost = player.saveDamageBoost;
        currentScene = player.saveCurrentScene;
        currentMoney = player.saveCurrentMoney;
        portalBools = player.savePortalBools;
        itemBools = player.saveItemBools;
        trophies = player.saveTrophyBools;
    }
}
