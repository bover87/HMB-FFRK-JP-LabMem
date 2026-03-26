using FFRK_LabMem.Services;
using System;
using System.Collections.Generic;

namespace FFRK_LabMem.Data
{
    class CounterComparers
    {

        private static readonly Dictionary<Char, Int32> romanMap = new Dictionary<char, int>
        {
            {'I', 1 },
            {'V', 5},
            {'X', 10},
        };

        private static readonly List<string> materialsStrings = new List<string>
        {
            "Rainbow",
            "Rosetta",
            "Adamantite",
            "Scarletite"
        };

        public class HEComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                var first = ExtractRealmValue(x);
                var second = ExtractRealmValue(y);
                var cmp1 = first.CompareTo(second);
                if (cmp1 == 0)
                {
                    return x.CompareTo(y);
                } else
                {
                    return cmp1;
                }
            }
        }

        public class DropComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                int firstCat = GetDropCategoryValue(x);
                int second = GetDropCategoryValue(y);
                int cmp1 = firstCat.CompareTo(second);
                if (cmp1 == 0)
                {
                    return x.CompareTo(y);
                } else
                {
                    return cmp1;
                }
            }
        }

        private static int ExtractRealmValue(String name)
        {
            if (name.EndsWith(")"))
            {
                int start = name.IndexOf('(') + 1;
                string realm = name.Substring(start, name.Length - start - 1);
                if (realm.EndsWith("-DoC")) return 71;
                if (realm.EndsWith("-CC")) return 72;
                if (realm.Equals("FFT")) return 160;
                if (realm.Equals("Type-0")) return 170;
                if (realm.Equals("Beyond")) return 190;
                return ConvertRomanToInt(realm) * 10;
            }
            return 180;
        }

        private static int ConvertRomanToInt(String romanNumeral) {
            int ret = 0;
            for (Int32 index = romanNumeral.Length - 1, last = 0; index >= 0; index--)
            {
                var key = romanNumeral[index];
                if (!romanMap.ContainsKey(key)) return 0;
                var current = romanMap[key];
                ret += (current < last ? -current : current);
                last = current;
            }
            return ret;
        }

        private static int GetDropCategoryValue(String name)
        {
            string nameTranslated = Translation.TranslateItem(name, true);
            if (nameTranslated.Contains("Mote")) return 1;
            if (nameTranslated.Contains("Crystal") && !name.Contains("Rainbow")) return 2;
            if (nameTranslated.Contains("Orb")) return 3;
            if (materialsStrings.Exists(s => nameTranslated.Contains(s))) return 4;
            if (nameTranslated.Contains("Tail")) return 5;
            if (nameTranslated.Contains("Arcana")) return 6;
            if (nameTranslated.Contains("Egg")) return 7;
            return 8;
        }
    }
}
