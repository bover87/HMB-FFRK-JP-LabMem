using FFRK_LabMem.Config;
using FFRK_LabMem.Machines;
using Microsoft.VisualBasic.Devices;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFRK_Machines;
using static FFRK_LabMem.Services.TranslationLookups;

namespace FFRK_LabMem.Services
{
    internal static class Translation
    {
        public const string EnemySuffix = " (Labyrinth)";
        public const string HeroMote = "Hero Mote";

        // Translates dungeon names
        public static String TranslateDungeon(String name)
        {
            if (!ColorConsole.Translate) return name;
            else
            {
                int index = name.IndexOf(" S");
                if (index >= 0)
                {
                    String dungeon = name.Substring(0, index);
                    String season = name.Replace(dungeon, String.Empty);

                    if (Dungeons.TryGetValue(dungeon, out string dungeonName)) return dungeonName + season;
                    else return name;
                }
                else throw new InvalidDataException();
            }
        }
        
        // Translates painting names
        public static String TranslatePainting(String name)
        {
            if (ColorConsole.Translate && PaintingStrings.TryGetValue(name, out string paintingName)) return paintingName;
            else return name;
        }

        // Translates enemy names, with option to ignore setting for use in blocklist checks
        public static String TranslateEnemy(String name, bool ignoreSetting = false)
        {
            if (!ColorConsole.Translate && !ignoreSetting) return name;
            else if (Enemies.TryGetValue(name, out string enemyName))
            {
                if (!ignoreSetting) return NumberEnemies(enemyName);
                else return enemyName;
            }
            else return name;
        }

        private static String NumberEnemies(String name)
        {
            if (EnemyNumber.TryGetValue(name, out string enemyCount)) return name + " " + enemyCount + EnemySuffix;
            else return name + EnemySuffix;
        }

        // Translates item names (and ignores Translate setting if ignoreSetting == true)
        public static String TranslateItem(String name, bool ignoreSetting = false)
        {
            // Checks if translation is turned on and aborts method if not (unless called with ignoreSetting, for use with Counters)
            if (!ColorConsole.Translate && !ignoreSetting) return name;

            // Checks if item name requires a more complex translation procedure
            else if (name.Contains("英雄専用フラグメント")) return TranslateHeroMote(name);
            else if (name.Contains("アニマレンズ")) return TranslateAnima(name);
            else if (name.Contains("のオーブ")) return TranslateOrb(name);
            else if (name.Contains("の結晶")) return TranslateCrystal(name);
            else if (name.Contains("のフラグメント")) return TranslateMote(name);
            else if (name.Contains("ねずみのしっぽ")) return TranslateRatTail(name);

            // Checks for item in translation dictionaries
            else if (Items.TryGetValue(name, out string itemName)) return itemName;
            else if (HeroEquipment.TryGetValue(name, out string equipName)) return equipName;
            else return name;
        }

        //Translates Hero Mote names
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
            else if (s.Contains("("))
            {
                heroName = heroName + "(" + s.Split('(', ')')[1] + ")";
            }
            // Get tier of Hero Mote
            String moteTier = s.Replace(heroName, String.Empty);

            // Generate full Hero Mote name if possible
            if (Characters.TryGetValue(heroName, out string hero)) return HeroMote + " (" + hero + ") " + moteTier;
            else return name;
        }

        // Translates Anima Lens names
        private static String TranslateAnima(String name)
        {
            if (AnimaTypes.TryGetValue(name.Replace("アニマレンズ", string.Empty), out string lensType)) return "Anima Lens " + lensType;
            else return name;
        }

        // Translates Crystal names
        private static String TranslateCrystal(String name)
        {
            if (OrbTypes.TryGetValue(name.Replace("の結晶", string.Empty), out string crystalType)) return crystalType + " Crystal";
            else return name;
        }
        
        // Translate Orb names
        private static String TranslateOrb(String name)
        {
            if (OrbTypes.TryGetValue(name.Replace("のオーブ(極大)", string.Empty), out string orbType)) return "Major " + orbType + " Orb";
            else return name;
        }

        //Translates mote names
        private static String TranslateMote(String name)
        {
            String moteType = name[0].ToString();
            String moteRarity = name.Remove(0, 1);
            moteRarity = moteRarity.Replace("のフラグメント(★", string.Empty);

            if (MoteTypes.TryGetValue(moteType, out string type) && Rarities.TryGetValue(moteRarity, out string rarity)) return type + " Mote (" + rarity;
            else return name;
        }

        // Translates Rat Tail names
        private static String TranslateRatTail(String name)
        {
            if (Sizes.TryGetValue(name.Replace("ねずみのしっぽ", string.Empty), out string size)) return size + "Rat Tail";
            else return name;
        }
    }
}
