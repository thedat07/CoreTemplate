using UnityEngine;
using System.Collections;

///
/// !!! Machine generated code !!!
/// !!! DO NOT CHANGE Tabs to Spaces !!!
/// 
[System.Serializable]
public class LevelPreviewData
{
  [SerializeField]
  PowerUpType poweruptype;
  public PowerUpType POWERUPTYPE { get {return poweruptype; } set { this.poweruptype = value;} }
  
  [SerializeField]
  int level;
  public int Level { get {return level; } set { this.level = value;} }
  
  [SerializeField]
  float value;
  public float Value { get {return value; } set { this.value = value;} }
  
  [SerializeField]
  int upgradecost;
  public int Upgradecost { get {return upgradecost; } set { this.upgradecost = value;} }
  
}