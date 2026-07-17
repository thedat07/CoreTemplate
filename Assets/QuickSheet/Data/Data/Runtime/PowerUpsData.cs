using UnityEngine;
using System.Collections;

///
/// !!! Machine generated code !!!
/// !!! DO NOT CHANGE Tabs to Spaces !!!
/// 
[System.Serializable]
public class PowerUpsData
{
  [SerializeField]
  PowerUpCategory powerupcategory;
  public PowerUpCategory POWERUPCATEGORY { get {return powerupcategory; } set { this.powerupcategory = value;} }
  
  [SerializeField]
  PowerUpType poweruptype;
  public PowerUpType POWERUPTYPE { get {return poweruptype; } set { this.poweruptype = value;} }
  
  [SerializeField]
  string displayname;
  public string Displayname { get {return displayname; } set { this.displayname = value;} }
  
  [SerializeField]
  float basevalue;
  public float Basevalue { get {return basevalue; } set { this.basevalue = value;} }
  
  [SerializeField]
  float perlevel;
  public float Perlevel { get {return perlevel; } set { this.perlevel = value;} }
  
  [SerializeField]
  int maxlevel;
  public int Maxlevel { get {return maxlevel; } set { this.maxlevel = value;} }
  
  [SerializeField]
  int basecost;
  public int Basecost { get {return basecost; } set { this.basecost = value;} }
  
  [SerializeField]
  float costgrowth;
  public float Costgrowth { get {return costgrowth; } set { this.costgrowth = value;} }
  
  [SerializeField]
  int unlockstage;
  public int Unlockstage { get {return unlockstage; } set { this.unlockstage = value;} }
  
  [SerializeField]
  string iconid;
  public string Iconid { get {return iconid; } set { this.iconid = value;} }
  
  [SerializeField]
  string description;
  public string Description { get {return description; } set { this.description = value;} }
  
}