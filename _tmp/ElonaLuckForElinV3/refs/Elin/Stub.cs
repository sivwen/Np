using System;using System.Collections.Generic;
public enum LockOpenState{Success,NotEnoughSkill,Fail}
public enum BlessedState{Normal,Cursed,Doomed,Blessed}
public enum Rarity{Normal,Superior,Legendary,Mythical,Artifact}
public enum TreasureType{Map,BossNefia,BossQuest,SurvivalRaid,RandomChest}
public class Category{public bool IsChildOf(string s)=>false;}
public class Card{
 public Category category=new Category();
 public int c_lockLv;
 public int hp;
 public int MaxHP=100;
 public int uid=1;
 public long ChildrenAndSelfWeight=>1000;
 public bool IsEquipment=>false;
 public Rarity rarity;
 public virtual int Evalue(int id)=>0;
 public bool IsPC=>true;
 public void SpawnLoot(Card origin){}
 public Thing MakeEgg(bool effect=true,int num=1,bool addToZone=true,int fertChance=20,BlessedState? state=null)=>new Thing();
}
public class ThingContainer:List<Thing>{}
public class Thing:Card{
 public ThingContainer things=new ThingContainer();
 public int tier;public int Num=1;public string id="";
 public void SetTier(int t){tier=t;}
 public void ModNum(int n,bool notify=true){Num+=n;}
 public Thing Duplicate(int n)=>new Thing{Num=n,id=id};
}
public class Chara:Card{
 public int DEX;public int STR;
 public bool IsPCParty=>true;
 public Card AddCard(Card c)=>c;
}
public class Trait{
 public Card owner=new Card();
 public virtual LockOpenState TryOpenLock(Chara c,bool msgFail=true)=>LockOpenState.Fail;
}
public class TraitCrafter:Trait{
 public virtual Thing Craft(AI_UseCrafter ai)=>null;
 public SourceRecipe.Row GetSource(AI_UseCrafter ai)=>new SourceRecipe.Row();
}
public class Point{
 public bool TryWitnessCrime(Chara criminal,Chara target=null,int radius=4,Func<Chara,bool> funcWitness=null)=>false;
}
public class Map{public void TrySmoothPick(Point p,Thing t,Chara c){}}
public class AI_Fish{public static Thing Makefish(Chara c)=>null;}
public class AI_Steal{}
public class Cell{}
public class GrowSystem{
 public static Cell cell=new Cell();
 public Thing TryPopSeed(Chara c)=>null;
 public void TryPick(Cell cell,Thing t,Chara c,bool applySeed=false){}
}
public static class TraitSeed{public static Thing MakeSeed(Cell c)=>new Thing();}
public class AI_UseCrafter{}
public class SourceRecipe{public class Row{public object type="";}}
public static class ThingGen{
 public static Thing Create(string id,int mat=-1,int lv=0)=>new Thing{id=id};
 public static Thing CreateFromCategory(string id,int lv=0)=>new Thing{id=id};
 public static void CreateTreasureContent(Thing t,int lv,TreasureType type,bool clearContent){}
}
public class RecipeSource{public Element GetReqSkill()=>new Element();}
public class Element{public int id;}
public class Recipe{
 public RecipeSource source=new RecipeSource();
 public virtual Thing Craft(BlessedState blessed,bool sound=false,List<Thing> ings=null,TraitCrafter crafter=null,bool model=false)=>new Thing();
}
public class RecipeCard:Recipe{
 public override Thing Craft(BlessedState blessed,bool sound=false,List<Thing> ings=null,TraitCrafter crafter=null,bool model=false)=>new Thing();
}
public class MiniGame{
 public class Balance{public int changeCoin;}
 public Balance balance=new Balance();
 public void Deactivate(){}
}
public class AttackProcess{
 public static AttackProcess Current=new AttackProcess();
 public Card CC;
 public Card TC;
 public bool crit;
}
public class GameDate{public int GetRaw()=>0;}
public class World{public GameDate date=new GameDate();}
public static class EClass{
 public static Chara pc=new Chara();
 public static World world=new World();
 public static int rnd(int a)=>0;
 public static float rndf(float a)=>0f;
}
