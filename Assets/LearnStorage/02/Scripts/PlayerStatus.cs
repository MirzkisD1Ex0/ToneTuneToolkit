namespace LearnStorage
{
  /// <summary>
  /// 
  /// </summary>
  public class PlayerStatus
  {
    private int health = 100;
    private int armor = 100;

    public int Health
    { // 保险做法 // 避免生命值被直接修改
      get
      {
        return this.health; // 把血量返回出去
      }
      set
      {
        this.health = value; // 将传入值赋给health
      }
    }

    public int Armor
    {
      get
      {
        return this.armor;
      }
    }
  }
}