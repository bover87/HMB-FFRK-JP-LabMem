using FFRK_LabMem.Machines;
using Microsoft.VisualBasic.Devices;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFRK_LabMem.Services
{
    internal class Translation
    {
        // Setting to turn on translation
        public static bool Translate { get; set; }

        // List of painting names in Japanese and their translations
        public static readonly Dictionary<String, String> Paintings = new Dictionary<String, String>()
        {
            { "戦いの絵画", "Combatant Painting" },
            { "探索の絵画", "Exploration Painting" },
            { "癒しの絵画", "Restoration Painting" },
            { "躍進の絵画", "Onslaught Painting" },
            { "宝物庫の絵画", "Treasure Painting" },
            { "転送の絵画", "Portal Painting" },
            { "堅牢の絵画", "Master Painting" }
        };

        // List of enemy names in Japanese and their translations
        public static readonly Dictionary<String, String> Enemies = new Dictionary<String, String>()
        {
            { "【迷宮】アダマンケリス", "Adamanchelid" },
            { "【迷宮】憤怒の霊帝アドラメレク", "Adrammelech" },
            { "【迷宮】アレキサンダー", "Alexander" },
            { "【迷宮】アデル", "Adel" },
            { "【迷宮】アンジールペナンス",  "Angeal Penance" },
            { "【迷宮】アトモス", "Atomos" },
            { "【迷宮】バハムート", "Bahamut" },
            { "【迷宮】バンダースナッチ", "Bandersnatch" },
            { "【迷宮】バルバリシア", "Barbariccia" },
            { "【迷宮】ベヒーモス", "Behemoth" },
            { "【迷宮】ビッグホーン", "Big Horn" },
            { "【迷宮】ブラックプリン", "Black Flan" },
            { "【迷宮】ビブロス", "Byblos" },
            { "【迷宮】ブリュンヒルデ", "Brynhildr" },
            { "【迷宮】ダイスダーグ", "Dycedarg" },
            { "【迷宮】エキドナ", "Echidna" },
            { "【迷宮】エクスペリメント", "Experiment" },
            { "【迷宮】マルドゥーク", "Faeryl" },
            { "【迷宮】ギザマルーク", "Gizamaluke" },
            { "【迷宮】ガードスコーピオン", "Guard Scorpion" },
            { "【迷宮】ヘルハウス", "Hell House" },
            { "【迷宮】ラニ＆焔色の髪の男", "Lani & Scarlet Hair" },
            { "【迷宮】リバイアサン", "Leviathan" },
            { "【迷宮】マジックポット", "Magic Pot" },
            { "【迷宮】メルティジェミニ", "Meltigemini" },
            { "【迷宮】ミスリルゴーレム", "Mythril Golem" },
            { "【迷宮】ネオチュー", "Neochu ×2" },
            { "【迷宮】ネオガルラ", "Neo Garula ×2" },
            { "【迷宮】ニンジャ", "Ninja" },
            { "【迷宮】神竜", "Nova Dragon" },
            { "【迷宮】パンデモニウム", "Pandaemonium" },
            { "【迷宮】プラントブレイン", "Plant Brain" },
            { "【迷宮】ラムザ", "Ramza" },
            { "【迷宮】レイヴス", "Ravus" },
            { "【迷宮】ウルフラマイター＆【迷宮】カトブレパス", "Red Giant & Catoblepas" },
            { "【迷宮】完熟大王", "Royal Ripeness" },
            { "【迷宮】セクレト&【迷宮】ミノタウロス", "Sacred & Minotaur" },
            { "【迷宮】銀竜", "Silver Dragon" },
            { "【迷宮】シャーリート", "Slyt" },
            { "【迷宮】ソル カノン＆ランチャー", "Soul Cannon" },
            { "【迷宮】タイラント", "Tyrant" },
            { "【迷宮】ウェンディゴ", "Wendigo" },
            { "【迷宮】ザグナル", "Zaghnol" },
            { "【迷宮】ゼムス", "Zemus" }
};

        public static readonly Dictionary<String, String> Items = new Dictionary<string, string>()
        {
            { "アダマンタイト【極大】", "Major Adamantite" },
            { "ギル", "Gil" },
            { "アルカナ【大】", "Greater Arcana" },
            { "グロウエッグ(極大)", "Major Growth Egg" },
            { "レテのしずく", "Lethe Tear" },
            { "魔法の鍵", "Magic Key" },
            { "記憶のしおり", "Record Marker" },
            { "七色の結晶", "Rainbow Shard" },
            { "叡智のロゼッタ石", "Rosette Stone of Wisdom" },
            { "ヒヒイロカネ【極大】", "Giant Scaletite" },
            { "テレポストーン", "Teleport Stone" },
            { "秘宝の地図", "Treasure Map" },
        };

        public static readonly Dictionary<String, String> Characters = new Dictionary<string, string>()
        {
            { "光の戦士", "Warrior of Light" },
            { "エコー", "Echo" },
            { "セーラ", "Sarah" },
            { "ウォル", "Wol" },
            { "ガーランド", "Garland" },
            { "スーパーモンク", "Master" },
            { "マトーヤ", "Matoya" },
            { "メイア", "Meia" },
            { "シーフ(I)", "Thief (I)" },
            { "ヨーゼフ", "Josef" },
            { "ゴードン", "Gordon" },
            { "フリオニール", "Firion" },
            { "リチャード", "Ricard" },
            { "レオンハルト", "Leon" },
            { "マリア", "Maria" },
            { "レイラ", "Leila" },
            { "ミンウ", "Minwu" },
            { "ガイ", "Guy" },
            { "皇帝", "Emperor" },
            { "スコット", "Scott" },
            { "ヒルダ", "Hilda" },
            { "ルーネス", "Luneth" },
            { "アルクゥ", "Arc" },
            { "レフィア", "Refia" },
            { "イングズ", "Ingus" },
            { "デッシュ", "Desch" },
            { "オニオンナイト", "Onion Knight" },
            { "暗闇の雲", "Cloud of Darkness" },
            { "エリア", "Aria" },
            { "カイン", "Kain" },
            { "セシル(暗黒騎士)", "Cecil (Dark Knight)" },
            { "リディア", "Rydia" },
            { "セシル(パラディン)", "Cecil (Paladin)" },
            { "テラ", "Tellah" },
            { "ゴルベーザ", "Golbez" },
            { "ギルバート", "Edward" },
            { "フースーヤ", "Fusoya" },
            { "ローザ", "Rosa" },
            { "エッジ", "Edge" },
            { "パロム", "Palom" },
            { "ポロム", "Porom" },
            { "ヤン", "Yang" },
            { "シド(IV)", "Cid (IV)" },
            { "セオドア", "Ceodore" },
            { "ルビカンテ", "Rubicante" },
            { "アーシュラ", "Ursula" },
            { "バルバリシア", "Barbariccia" },
            { "レナ", "Lenna" },
            { "ガラフ", "Galuf" },
            { "ギルガメッシュ", "Gilgamesh" },
            { "バッツ", "Bartz" },
            { "ファリス", "Faris" },
            { "クルル", "Krile" },
            { "エクスデス", "Exdeath" },
            { "ドルガン", "Dorgann" },
            { "ものまねしゴゴ", "Gogo (V)" },
            { "ケルガー", "Kelger" },
            { "ゼザ", "Xezat" },
            { "ティナ", "Terra" },
            { "ストラゴス", "Strago" },
            { "セリス", "Celes" },
            { "セッツァー", "Setzer" },
            { "ロック", "Locke" },
            { "モグ", "Mog" },
            { "エドガー", "Edgar" },
            { "マッシュ", "Sabin" },
            { "シャドウ", "Shadow" },
            { "カイエン", "Cyan" },
            { "ガウ", "Gau" },
            { "ケフカ", "Kefka" },
            { "リルム", "Relm" },
            { "レオ将軍", "Leo" },
            { "ゴゴ", "Gogo (VI)" },
            { "ウーマロ", "Umaro" },
            { "オルトロス", "Ultros" },
            { "クラウド", "Cloud" },
            { "エアリス", "Aerith" },
            { "ティファ", "Tifa" },
            { "セフィロス", "Sephiroth" },
            { "レッドXIII", "Red XIII" },
            { "ザックス", "Zack" },
            { "バレット", "Barret" },
            { "シド(VII)", "Cid (VII)" },
            { "レノ", "Reno" },
            { "ユフィ", "Yuffie" },
            { "ヴィンセント", "Vincent" },
            { "ケット・シー", "Cait Sith" },
            { "アンジール", "Angeal" },
            { "ルーファウス", "Rufus" },
            { "シェルク", "Shelke" },
            { "ルード", "Rude" },
            { "イリーナ", "Elena" },
            { "ジェネシス", "Genesis" },
            { "リノア", "Rinoa" },
            { "アーヴァイン", "Irvine" },
            { "スコール", "Squall" },
            { "セルフィ", "Selphie" },
            { "キスティス", "Quistis" },
            { "ゼル", "Zell" },
            { "サイファー", "Seifer" },
            { "ラグナ", "Laguna" },
            { "イデア", "Edea" },
            { "雷神", "Raijin" },
            { "風神", "Fujin" },
            { "キロス", "Kiros" },
            { "ウォード", "Ward" },
            { "アルティミシア", "Ultimecia" },
            { "ビビ", "Vivi" },
            { "ガーネット", "Garnet" },
            { "スタイナー", "Steiner" },
            { "エーコ", "Eiko" },
            { "サラマンダー", "Amarant" },
            { "ジタン", "Zidane" },
            { "クイナ", "Quina" },
            { "ベアトリクス", "Beatrix" },
            { "フライヤ", "Freya" },
            { "クジャ", "Kuja" },
            { "マーカス", "Marcus" },
            { "ティーダ", "Tidus" },
            { "ワッカ", "Wakka" },
            { "キマリ", "Kimahri" },
            { "ユウナ", "Yuna" },
            { "アーロン", "Auron" },
            { "ルールー", "Lulu" },
            { "リュック", "Rikku" },
            { "ジェクト", "Jecht" },
            { "ブラスカ", "Braska" },
            { "パイン", "Paine" },
            { "シーモア", "Seymour" },
            { "シャントット", "Shantotto" },
            { "アヤメ", "Ayame" },
            { "クリルラ", "Curilla" },
            { "プリッシュ", "Prishe" },
            { "ライオン", "Lion" },
            { "アフマウ", "Aphmau" },
            { "ザイド", "Zeid" },
            { "リリゼット", "Lilisette" },
            { "ナジャ", "Naja" },
            { "アシェラ", "Arciela" },
            { "フラン", "Fran" },
            { "バルフレア", "Balthier" },
            { "ヴァン", "Vaan" },
            { "アーシェ", "Ashe" },
            { "パンネロ", "Penelo" },
            { "バッシュ", "Basch" },
            { "ガブラス", "Gabranth" },
            { "ラーサー", "Larsa" },
            { "ヴェイン", "Vayne" },
            { "レックス", "Reks" },
            { "スノウ", "Snow" },
            { "ヴァニラ", "Vanille" },
            { "ライトニング", "Lightning" },
            { "サッズ", "Sazh" },
            { "ホープ", "Hope" },
            { "ファング", "Fang" },
            { "セラ", "Serah" },
            { "レインズ", "Cid Raines" },
            { "ノエル", "Noel" },
            { "ナバート", "Nabaat" },
            { "ヤ・シュトラ", "Y'shtola" },
            { "サンクレッド", "Thancred" },
            { "イダ", "Yda" },
            { "パパリモ", "Papaylmo" },
            { "アルフィノ", "Alphinaud" },
            { "ミンフィリア", "Minfilia" },
            { "シド(XIV)", "Cid (XIV)" },
            { "イゼル", "Ysayle" },
            { "オルシュファン", "Haurchefant" },
            { "エスティニアン", "Estinien" },
            { "アリゼー", "Alisaie" },
            { "ノクティス", "Noctis" },
            { "グラディオラス", "Gladiolus" },
            { "イリス", "Iris" },
            { "プロンプト", "Prompto" },
            { "アラネア", "Aranea" },
            { "イグニス", "Ignis" },
            { "コル", "Cor" },
            { "ルナフレーナ", "Lunafreya" },
            { "アーデン", "Ardyn" },
            { "レイヴス", "Ravus" },
            { "クライヴ", "Clive" },
            { "ジル", "Jill" },
            { "ジョシュア", "Joshua" },
            { "シドルファス", "Cidolfus" },
            { "トルガル", "Torgal" },
            { "ラムザ", "Ramza" },
            { "アグリアス", "Agrias" },
            { "オヴェリア", "Ovelia" },
            { "ムスタディオ", "Mustadio" },
            { "ディリータ", "Delita" },
            { "オルランドゥ", "Orlandeau" },
            { "ガフガリオン", "Gaffgarion" },
            { "ラファ", "Rapha" },
            { "マラーク", "Marach" },
            { "メリアドール", "Meliadoul" },
            { "マーシュ", "Marche" },
            { "モンブラン", "Montblanc" },
            { "アルマ", "Alma" },
            { "オーラン", "Orran" },
            { "エース", "Ace" },
            { "デュース", "Deuce" },
            { "ナイン", "Nine" },
            { "マキナ", "Machina" },
            { "レム", "Rem" },
            { "クイーン", "Queen" },
            { "キング", "King" },
            { "シンク", "Cinque" },
            { "セブン", "Seven" },
            { "サイス", "Sice" },
            { "ジャック", "Jack" },
            { "エイト", "Eight" },
            { "ケイト", "Cater" },
            { "トレイ", "Trey" },
            { "クラサメ", "Kurasame" },
            { "レェン", "Reynn" },
            { "ラァン", "Lann" },
            { "トゥモロ", "Morrow" },
            { "エモ", "Aemo" },
            { "リーグ", "Wrieg" },
            { "タマ", "Tama" },
            { "エナ・クロ", "Enna Kros" },
            { "セラフィ", "Serafie" },
            { "レイン", "Rain" },
            { "フィーナ", "Fina" },
            { "デシ", "Tyro" },
            { "ウララ", "Elarra" },
            { "ビッグス", "Biggs" },
            { "ウェッジ", "Wedge" },
            { "Dr.モグ", "Dr. Mog" },
            { "シャドウスミス", "Shadowsmith" }
        };

        public static readonly Dictionary<String, String> MoteTypes = new Dictionary<String, String>()
        {
            { "技", "Dexterity" },
            { "心", "Spirit" },
            { "体", "Vitality" },
            { "知", "Wisdom" },
            { "勇", "Bravery" }
        };

        public static readonly Dictionary<String, String> AnimaTypes = new Dictionary<String, String>()
        {
            { "Lv1", "Anima Lens Lv1" },
            { "Lv2", "Anima Lens Lv2" },
            { "Lv3", "Anima Lens Lv3" },
            { "EX", "Anima Lens EX" }
        };

        public static readonly Dictionary<String, String> Rarities = new Dictionary<String, String>()
        {
            { "3)", "3★)" },
            { "4)", "4★)" },
            { "5)", "5★)" },
            { "6)", "6★)" },
            { "7)", "7★)" }
        };

        public static readonly Dictionary<String, String> Sizes = new Dictionary<String, String>()
        {
            { "【大】", "Greater" },
            { "【極大】", "Major" }
        };

        // List of orb/crystal types in Japanese and their translations
        public static readonly Dictionary<String, String> OrbTypes = new Dictionary<String, String>()
        {
            { "炎", "Fire" },
            { "氷", "Ice" },
            { "雷", "Lightning" },
            { "水", "Water" },
            { "風", "Wind" },
            { "地", "Earth" },
            { "聖", "Holy" },
            { "闇", "Dark" },
            { "毒", "Poison" },
            { "無", "Non-Elemental" },
            { "力", "Power" },
            { "白", "White" },
            { "黒", "Black" },
            { "召喚", "Summoning" }
        };

        // Translates painting names
        public static String TranslatePainting(String name)
        {
            if (Translate == true && Paintings.TryGetValue(name, out string translated)) return translated;
            else return name;
        }

        public static String TranslateEnemy(String name)
        {
            if (Translate == true && Enemies.TryGetValue(name, out string translated)) return translated + " (Labyrinth)";
            else return name;
        }

        public static String TranslateItem(String name)
        {
            if (Translate == false) return name;
            else if (name.Contains("英雄専用フラグメント")) return TranslateHeroMote(name);
            else if (name.Contains("アニマレンズ")) return TranslateAnima(name);
            else if (name.Contains("のオーブ")) return TranslateOrb(name);
            else if (name.Contains("の結晶")) return TranslateCrystal(name);
            else if (name.Contains("のフラグメント")) return TranslateMote(name);
            else if (name.Contains("ねずみのしっぽ")) return TranslateRatTail(name);

            else if (Items.TryGetValue(name, out string translated)) return translated;
            else return name;
        }

        private static String TranslateHeroMote(String name)
        {
            // Strip brackets and Japanese "Hero Mote" text from mote name
            String s = name.Replace("英雄専用フラグメント", string.Empty);
            s = s.Replace("【", string.Empty);
            s = s.Replace("】", string.Empty);

            // Remove the tier of the mote from the hero name as well as text marking the realm if present
            String heroName = s.Replace("I", string.Empty);
            heroName = heroName.Replace("V", string.Empty);
            heroName = heroName.Replace("X", string.Empty);
            heroName = heroName.Replace("(", string.Empty);
            heroName = heroName.Replace(")", string.Empty);

            // Check if user's name has a realm in their name (e.g. the Cids) and re-add it
            if (name.Contains("レッドXIII")) heroName = "レッドXIII";
            else if (s.Contains("(")) heroName = "(" + s.Split('(', ')')[1] + ")";

            // Get tier of Hero Mote
            String moteTier = s.Replace(heroName, String.Empty);

            // Generate full Hero Mote name if possible
            if (Characters.TryGetValue(heroName, out string hero)) return "Hero Mote (" + hero + ") " + moteTier;
            else return name;
        }

        private static String TranslateAnima(String name)
        {
            if (AnimaTypes.TryGetValue(name.Replace("アニマレンズ", string.Empty), out string lensType)) return "Anima Lens " + lensType;
            else return name;
        }

        private static String TranslateCrystal(String name)
        {
            if (OrbTypes.TryGetValue(name.Replace("の結晶", string.Empty), out string crystalType)) return crystalType + " Crystal";
            else return name;
        }
        
        private static String TranslateOrb(String name)
        {
            if (OrbTypes.TryGetValue(name.Replace("のオーブ(極大)", string.Empty), out string orbType)) return "Major " + orbType + " Orb";
            else return name;
        }

        private static String TranslateMote(String name)
        {
            String moteType = name[0].ToString();
            String moteRarity = name.Remove(0, 1);
            moteRarity = moteRarity.Replace("のフラグメント(★", string.Empty);

            if (MoteTypes.TryGetValue(moteType, out string type) && Rarities.TryGetValue(moteRarity, out string rarity)) return type + " Mote (" + rarity;
            else return name;
        }

        private static String TranslateRatTail(String name)
        {
            if (Sizes.TryGetValue(name.Replace("ねずみのしっぽ", string.Empty), out string rarity)) return rarity + " Rat Tail";
            else return name;
        }
    }
}
