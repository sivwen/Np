using System;using System.Collections.Generic;
public enum Rarity{Random=-999,Crude=-1,Normal=0,Superior=1,Legendary=2,Mythical=3,Artifact=4}
public enum LockOpenState{Success,NotEnoughSkill,Fail}
public enum BlessedState{Normal,Cursed,Doomed,Blessed}
public class TagList{readonly HashSet<string>h=new HashSet<string>();public bool Contains(string s)=>h.Contains(s);}
public class Category{public TagList tag=new TagList();public bool IsChildOf(string s)=>false;}
public class SourceCard{public int quality;}
public class CardBlueprint{public bool isCraft;}
public class Element{public int id;public int vBase;public int Value;public SourceElement source=new SourceElement();}
public class SourceElement{public int LV;public string encSlot="";public string category="";public int encFactor;public TagList tag=new TagList();}
public class ElementContainer{public Dictionary<int,Element> dict=new Dictionary<int,Element>();public Element ModBase(int id,int n){if(!dict.TryGetValue(id,out var e)){e=new Element{id=id};dict[id]=e;}e.vBase+=n;return e;}}
public class CardRow{public string id="";public int LV;public int chance=1;}
public class SourceThing{public class Row:CardRow{public int value;}}
public class SourceChara{public class Row:CardRow{public int quality;}}
public class Card{public CardBlueprint bp=new CardBlueprint();public SourceCard sourceCard=new SourceCard();public Category category=new Category();public Rarity rarity;public string id="";public int uid;public int c_lockLv;public virtual int Evalue(int id)=>0;public int ChildrenAndSelfWeight=>0;public virtual void Destroy(){}}
public class Thing:Card{public bool IsEquipmentOrRangedOrAmmo=>true;public bool IsAmmo=>false;public bool IsEquipmentOrRanged=>true;public bool IsCursed=>false;public TagList tags=new TagList();public ElementContainer elements=new ElementContainer();public int tier;public int Num=1;public virtual void OnCreate(int genLv){}public virtual void ApplyMaterial(bool remove=false){}public Element AddEnchant(int lv=-1)=>new Element();public bool HasTag(string s)=>false;public void SetTier(int t){tier=t;}public void ModNum(int a,bool notify=true){Num+=a;}public Thing Duplicate(int n){return new Thing{Num=n,id=id};}}
public class Chara:Card{public bool IsPC=>true;public bool IsPCParty=>true;public int STR;public SourceChara.Row source=new SourceChara.Row();public Card AddCard(Card c)=>c;}
public class Trait{public Card owner=new Card();public virtual LockOpenState TryOpenLock(Chara c,bool msgFail=true)=>LockOpenState.Fail;}
public class TraitCrafter:Trait{}
public class Point{public bool TryWitnessCrime(Chara criminal,Chara target=null,int radius=4,Func<Chara,bool> funcWitness=null)=>false;}
public class Map{public void TrySmoothPick(Point p,Thing t,Chara c){}}
public class AI_Fish{public static Thing Makefish(Chara c)=>null;}
public class RecipeSource{public Element GetReqSkill()=>new Element();}
public class Recipe{public RecipeSource source=new RecipeSource();public virtual Thing Craft(BlessedState blessed,bool sound=false,List<Thing> ings=null,TraitCrafter crafter=null,bool model=false)=>new Thing();}
public class RecipeCard:Recipe{public override Thing Craft(BlessedState blessed,bool sound=false,List<Thing> ings=null,TraitCrafter crafter=null,bool model=false)=>new Thing();}
public class SpawnList{public CardRow Select(int lv=-1,int levelRange=-1)=>new CardRow();}
public class LayerGachaResult{public static Chara Draw(string id="citizen")=>new Chara();}
public class MiniGame{public class Balance{public int lastCoin;public int changeCoin;}public Balance balance=new Balance();public void Deactivate(){}}
public static class EClass{public static Chara pc=new Chara();public static int rnd(int a)=>0;public static float rndf(float a)=>0f;}
public static class Dice{public static long Roll(int n,int s,int b=0,Card c=null)=>0;}
public static class CTAG{public const string noRandomEnc="noRandomEnc";}
public class GrowSystem{public Thing TryPopSeed(Chara c)=>null;}
