using System;using System.Collections.Generic;
public enum LockOpenState{Success,NotEnoughSkill,Fail}
public enum BlessedState{Normal,Cursed,Doomed,Blessed}
public enum Rarity{Normal,Superior,Legendary,Mythical,Artifact}
public enum TreasureType{Map,BossNefia,BossQuest,SurvivalRaid,RandomChest}
public class Category{public bool IsChildOf(string s)=>false;}
public class CardRow{public string[] loot=Array.Empty<string>();}
public class SourceRace{public class Row{public string[] loot=Array.Empty<string>();}}
public class Point{public List<Card> ListCards(bool includeMasked=false)=>new List<Card>();public bool TryWitnessCrime(Chara criminal,Chara target=null,int radius=4,Func<Chara,bool> funcWitness=null)=>false;}
public class Card{
 public Category category=new Category();public Point pos=new Point();public string id="";public bool isThing=true;
 public int c_lockLv;public int hp;public int MaxHP=100;public int LV=1;public int uid=1;public long ChildrenAndSelfWeight=>1000;
 public bool IsEquipment=>false;public bool IsPC=>true;public bool IsPCFaction=>true;public bool IsPCFactionOrMinion=>true;public Rarity rarity;
 public virtual CardRow sourceCard=>new CardRow();public Chara Chara=>this as Chara;
 public virtual int Evalue(int id)=>0;public Thing MakeEgg(bool effect=true,int num=1,bool addToZone=true,int fertChance=20,BlessedState? state=null)=>new Thing();
}
public class Thing:Card{public int tier;public int Num=1;public void SetTier(int t){tier=t;}public void ModNum(int n,bool notify=true){Num+=n;}public Thing Duplicate(int n)=>new Thing{Num=n,id=id};}
public class Chara:Card{public int DEX;public int STR;public bool IsPCParty=>true;public bool IsMachine;public bool IsAnimal;public SourceRace.Row race=new SourceRace.Row();public override CardRow sourceCard=>new CardRow();public Card AddCard(Card c)=>c;public bool HasElement(int id)=>false;public Thing MakeGene()=>new Thing{id="gene"};}
public class Trait{public Card owner=new Card();public virtual LockOpenState TryOpenLock(Chara c,bool msgFail=true)=>LockOpenState.Fail;}
public class TraitCrafter:Trait{}
public class AI_Fish{public static Thing Makefish(Chara c)=>null;}
public class AI_Steal{public bool Perform()=>true;}
public class TaskChopWood{}
public class GrowSystem{public Thing TryPopSeed(Chara c)=>null;public void Harvest(Chara c){}}
public class Map{public void TrySmoothPick(Point p,Thing t,Chara c){}public void MineBlock(Point p,bool recoverBlock=false,Chara c=null,bool mineObj=true){}public void MineFloor(Point p,Chara c=null,bool recoverBlock=false,bool removePlatform=true){}}
public class MiniGame{public class Balance{public int changeCoin;}public Balance balance=new Balance();public void Deactivate(){}}
public class AttackProcess{public static AttackProcess Current=new AttackProcess();public Card CC;public Card TC;public bool crit;}
public class Zone{public Card AddCard(Card c,Point p)=>c;}
public class ZoneEventManager{public void OnCharaDie(Chara c){}}
public static class ThingGen{public static Thing Create(string id,int idMat=-1,int lv=-1)=>new Thing{id=id,LV=lv};public static void CreateTreasureContent(Thing t,int lv,TreasureType type,bool clearContent){}}
public static class EClass{public static Chara pc=new Chara();public static Zone _zone=new Zone();public static int rnd(int a)=>0;public static float rndf(float a)=>0f;}
