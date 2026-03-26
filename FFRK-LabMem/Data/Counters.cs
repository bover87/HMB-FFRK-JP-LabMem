using FFRK_LabMem.Config;
using FFRK_LabMem.Machines;
using FFRK_Machines;
using FFRK_LabMem.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FFRK_LabMem.Data
{
    public class Counters
    {

        // Singleton instance
        private static Counters _instance = null;
        // Constants
        private const string CONFIG_PATH = "./Data/counters.json";
        public static readonly ReadOnlyDictionary<string, CounterSet> DefaultCounterSets = new ReadOnlyDictionary<string, CounterSet>(
            new Dictionary<string, CounterSet>
            {
                {"Session", new CounterSet() { Name = "Current Session" } },
                {"CurrentLab", new CounterSet() { Name = "Current Lab" } },
                {"Group", new CounterSet() { Name = "Current Group" } },
                {"Total", new CounterSet() { Name = "All Time" } },
            }
        );

        // Events
        public static event Action OnUpdated;

        [Flags]
        public enum DropCategory
        {
            UNKNOWN = 0,
            EQUIPMENT = 1 << 0,
            LABYRINTH_ITEM = 1 << 1,
            COMMON = 1 << 2,
            SPHERE_MATERIAL = 1 << 3,
            HERO_MOTE = 1 << 4,
            ABILITY_MATERIAL = 1 << 5,
            EQUIPMENT_SP_MATERIAL = 1 << 6,
            HISTORIA_CRYSTAL_ENHANCEMENT_MATERIAL = 1 << 7,
            GROW_EGG = 1 << 8,
            BEAST_FOOD = 1 << 9,
            RECORD_MATERIA = 1 << 10,
        }

        // Public properties
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public Dictionary<string, CounterSet> CounterSets { get; set; }
        public DropCategory DropCategories { get; set; } = DropCategory.EQUIPMENT | 
            DropCategory.LABYRINTH_ITEM | 
            DropCategory.COMMON | 
            DropCategory.SPHERE_MATERIAL |
            DropCategory.HERO_MOTE;
        public bool LogDropsToTotalCounters { get; set; } = false;
        public int MaterialsRarityFilter { get; set; } = 6;
        public string CurrentLabId = null;
        public int BufferSize { get; set; } = 10;

        // Private fields
        private CounterSet CurrentLabBufferSet { get; set; } = new CounterSet();
        private readonly LabController controller;
        private readonly Stopwatch runtimeStopwatch = new Stopwatch();
        private int bufferWrites = 0;

        private Counters(LabController controller)
        {
            this.CounterSets = DefaultCounterSets.Select(dict => dict).ToDictionary(pair => pair.Key, pair => pair.Value);
            this.controller = controller;
            controller.OnEnabled += Controller_OnEnabled;
            controller.OnDisabled += Controller_OnDisabled;
        }
       
        public static Counters Default
        {
            get
            {
                if (_instance == null) throw new InvalidOperationException();
                return _instance;
            }
        }
        public static async Task Initalize(ConfigHelper config, LabController controller)
        {
            if (_instance == null)
            {
                _instance = new Counters(controller);
                await _instance.Load();
                _instance.DropCategories = (Counters.DropCategory)config.GetInt("counters.dropCategories", 31);
                _instance.LogDropsToTotalCounters = config.GetBool("counters.logDropsToTotal", false);
                _instance.MaterialsRarityFilter = config.GetInt("counters.materialsRarityFilter", 6);
            }

        }
        public static void Uninitalize()
        {
            _instance.controller.OnEnabled -= _instance.Controller_OnEnabled;
            _instance.controller.OnDisabled -= _instance.Controller_OnDisabled;
        }
        private async void Controller_OnDisabled(object sender, EventArgs e)
        {
            await Save(CONFIG_PATH, true);
            runtimeStopwatch.Stop();
            ClearCurrentLab();
        }
        private void Controller_OnEnabled(object sender, EventArgs e)
        {
            runtimeStopwatch.Restart();
        }
        public static bool IsMissionCompleted()
        {
            // Last completion time taken from Total couterset in UTC
            var last = _instance.CounterSets["Total"].LastCompleted.ToUniversalTime();
            
            // UTC now
            var now = DateTime.UtcNow;
            
            // Missions reset at 08:00 UTC
            // If the hour is 8 or greater then use today, else use yesterday
            var compare = (now.Hour >= 8) ? now.Date.AddHours(8) : now.Date.AddDays(-1).AddHours(8);

            return  last >= compare;
        }
        public static async Task QuickExplore(string id, string name)
        {
            // Reset the current lab
            ClearCurrentLab();
            Counters.SetCurrentLab(id, name, false);

            // Timestamp
            _instance.IncrementLastCompleted();

            // Increment counter
            await _instance.IncrementCounter("QuickExplores");

        }
        public static async Task LabRunCompleted(bool incrementLastCompleted)
        {
            // Increment counters
            await _instance.IncrementCounter("LabRunsCompleted", 1, false);

            // Timestamp
            if (incrementLastCompleted) _instance.IncrementLastCompleted();

            // Reset the current lab counter set
            _instance.CounterSets["CurrentLab"].Reset(CounterSet.DataType.All);

            // Save to file
            await _instance.Save();

            // Reset the current lab id and buffer since it is now unkown
            ClearCurrentLab();

            
        }
        public static async Task PaintingSelected()
        {
            await _instance.IncrementCounter("PaintingsSelected");
        }
        public static async Task BattleWon(TimeSpan runtime)
        {
            _instance.IncrementRuntime("Battle", runtime);
            await _instance.IncrementCounter("BattlesWon");
        }
        public static async Task TreasureOpened()
        {
            await _instance.IncrementCounter("TreasuresOpened");
        }
        public static async Task FoundRadiantPainting()
        {
            await _instance.IncrementCounter("RadiantPaintings");
        }
        public static async Task FoundMagicPot()
        {
            await _instance.IncrementCounter("MagicPots");
        }
        public static async Task UsedTears(int amt)
        {
            await _instance.IncrementCounter("UsedTears", amt);
        }
        public static async Task UsedKeys(int amt)
        {
            await _instance.IncrementCounter("UsedKeys", amt);
        }
        public static async Task UsedTeleportStone()
        {
            await _instance.IncrementCounter("UsedTeleportStones");
        }
        public static async Task UsedStaminaPot()
        {
            await _instance.IncrementCounter("UsedStaminaPots");
        }
        public static async Task PulledInPortal()
        {
            await _instance.IncrementCounter("PulledInPortal");
        }
        public static async Task FFRKCrashed()
        {
            await _instance.IncrementCounter("FFRKCrashes");
        }
        public static async Task FFRKHang(bool decrementRecovered = false)
        {
            await _instance.IncrementCounter("FFRKHangs");
            if (decrementRecovered) await _instance.IncrementCounter("FFRKRecoveries", -1);
        }
        public static async Task FFRKRecovered()
        {
            await _instance.IncrementCounter("FFRKRecoveries");
        }
        public static async Task FFRKRestarted()
        {
            await _instance.IncrementCounter("FFRKRestarts");
        }
        public static async Task EnemyIsUponYou()
        {
            await _instance.IncrementCounter("EnemyIsUponYou");
        }
        static async Task FoundDrop(DropCategory category, string itemName, int rarity, int qty, bool isQE = false)
        {
            if (category.Equals(DropCategory.EQUIPMENT))
            {
                _instance.IncrementHE(itemName, isQE);
                await _instance.IncrementCounter("HeroEquipmentGot");
            }
            else if (category.Equals(DropCategory.HERO_MOTE))
            {
                _instance.IncrementHM(itemName, qty, isQE);
                await _instance.IncrementCounter("HeroMotesGot", qty);
            }
            else
            {
                // Filter materials drops
                if (rarity == 0 && category != DropCategory.LABYRINTH_ITEM && category != DropCategory.RECORD_MATERIA) rarity = CounterInference.InferRarity(category, itemName);
                // if (!(DropCategory.LABYRINTH_ITEM | DropCategory.COMMON | DropCategory.RECORD_MATERIA).HasFlag(category) && rarity > 0 && rarity < _instance.MaterialsRarityFilter) return;

                _instance.IncrementDrop(itemName, qty, isQE);
                await _instance.Save();
            }
        }
        public static async Task FoundDrop(string dropType, string itemName, int rarity, int qty)
        {
            if (Enum.TryParse(dropType, out DropCategory category))
            {
                if (category == DropCategory.SPHERE_MATERIAL && Translation.TranslateItem(itemName, true).Contains(Translation.HeroMote)) category = DropCategory.HERO_MOTE;
                await FoundDrop(category, itemName, rarity, qty);
            }
            else
            {
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Unknown drop type: {0}", dropType);
            }

        }
        public static async Task FoundQEDrop(string name, int qty, string imagePath)
        {
            DropCategory category = CounterInference.InferCategory(imagePath);
            if (category != DropCategory.UNKNOWN)
            {
                // Passing 0 for rarity will use inference
                await FoundDrop(category, name, 0, qty, true);
            } else
            {
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Could not infer drop category for url: {0}", imagePath);
            }

        }
        private async Task IncrementCounter(string key, int amt = 1, bool save = true)
        {
            if (amt == 0) return;
            foreach (KeyValuePair<string, CounterSet> set in GetTargetCounterSets())
            {
                set.Value.Counters[key] += amt;
                if (set.Value.Counters[key] < 0) set.Value.Counters[key] = 0;
            }
            if (save) await _instance.Save();
        }
        private void IncrementRuntime(string key, TimeSpan amt)
        {
            if (amt.TotalMilliseconds <= 0) return;
            foreach (var set in GetTargetCounterSets())
            {
                set.Value.Runtime[key] += amt;
            }
        }
        private void IncrementHE(string name, bool isQE = false)
        {
            foreach (KeyValuePair<string, CounterSet> set in GetTargetCounterSets())
            {
                if (!set.Key.Equals("Total") || LogDropsToTotalCounters)
                {
                    SortedDictionary<string, int> target = (isQE) ? set.Value.HeroEquipmentQE : set.Value.HeroEquipment;
                    if (target.ContainsKey(name))
                    {
                        target[name] += 1;
                    } else
                    {
                        target.Add(name, 1);
                    }
                }
            }
        }
        private void IncrementHM(string name, int amt = 1, bool isQE = false)
        {
            foreach (KeyValuePair<string, CounterSet> set in GetTargetCounterSets())
            {
                if (!set.Key.Equals("Total") || LogDropsToTotalCounters)
                {
                    SortedDictionary<string, int> target = (isQE) ? set.Value.HeroMotesQE : set.Value.HeroMotes;
                    if (target.ContainsKey(name))
                    {
                        target[name] += amt;
                    }
                    else
                    {
                        target.Add(name, amt);
                    }
                }
            }
        }
        private void IncrementDrop(string name, int amt = 1, bool isQE = false)
        {
            foreach (var set in GetTargetCounterSets())
            {
                if (!set.Key.Equals("Total") || LogDropsToTotalCounters)
                {
                    var target = (isQE) ? set.Value.DropsQE : set.Value.Drops;
                    if (target.ContainsKey(name))
                    {
                        target[name] += amt;
                    }
                    else
                    {
                        target.Add(name, amt);
                    }
                }
            }
        }
        private void IncrementLastCompleted()
        {
            GetTargetCounterSets().ForEach(s => s.Value.LastCompleted = DateTime.Now);
        }
        private List<KeyValuePair<string,CounterSet>> GetTargetCounterSets(){
            List<KeyValuePair<string, CounterSet>> ret = CounterSets.Where(s => DefaultCounterSets.ContainsKey(s.Key) || s.Key.Equals(CurrentLabId)).ToList();
            if (CurrentLabId == null) ret.Add(new KeyValuePair<string, CounterSet>("_Buffer", CurrentLabBufferSet));
            return ret;
        }
        private async void SetLab(string id, string dungeonName, bool showMessage = true)
        {
            if (CurrentLabId == null || !CurrentLabId.Equals(id))
            {
                // Update or create entry here
                if (CounterSets.ContainsKey(id))
                {
                    CounterSets[id].AddCounters(CurrentLabBufferSet);
                }
                else
                {
                    // Create a new entry and add counters in the buffer to it
                    CounterSet newEntry = new CounterSet
                    {
                        Name = Translation.TranslateDungeon(dungeonName)
                    };
                    newEntry.AddCounters(CurrentLabBufferSet);
                    CounterSets.Add(id, newEntry);
                }
                await Save();
                if (showMessage)
                {
                    try 
                    {
                        ColorConsole.WriteLine(ConsoleColor.DarkCyan, "Current lab set to: {0}", Translation.TranslateDungeon(dungeonName)); 
                    }
                    catch (InvalidDataException)
                    {
                        ColorConsole.WriteLine(ConsoleColor.Red, "Invalid dungeon data passed, unable to translate");
                        ColorConsole.WriteLine(ConsoleColor.DarkCyan, "Current lab set to: {0}", dungeonName);
                    }
                } else
                {
                    try
                    {
                        ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Current lab set to: {0}", Translation.TranslateDungeon(dungeonName));
                    }
                    catch (InvalidDataException)
                    {
                        ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Invalid dungeon data passed, unable to translate");
                        ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Current lab set to: {0}", dungeonName);
                    }
                }
            }
            // Reset counters in buffer
            CurrentLabBufferSet.Reset(CounterSet.DataType.All);
            CurrentLabId = id;
        }
        public static void SetCurrentLab(string id, string dungeonName, bool showMessage = true)
        {
            _instance.SetLab(id, dungeonName, showMessage);
        }
        public static void ClearCurrentLab()
        {
            _instance.CurrentLabBufferSet.Reset(CounterSet.DataType.All);
            _instance.CurrentLabId = null;
            ColorConsole.Debug(ColorConsole.DebugCategory.Lab, "Current lab cleared");
        }
        public async Task Load(string path = CONFIG_PATH)
        {
            try
            {
                JsonConvert.PopulateObject(File.ReadAllText(path), CounterSets);
            }
            catch (FileNotFoundException) { }
            catch (DirectoryNotFoundException) { }
            catch (Exception ex)
            {
                ColorConsole.WriteLine(ConsoleColor.Yellow, "Error loading counters file: {0}", ex);
            }
            await Task.CompletedTask;
        }
        public async Task Save(string path = CONFIG_PATH, bool noBuffer = false)
        {
            if (runtimeStopwatch.IsRunning)
            {
                IncrementRuntime("Total", runtimeStopwatch.Elapsed);
                runtimeStopwatch.Restart();
            }
            OnUpdated?.Invoke();

            // Buffer Check
            if (!noBuffer)
            {
                bufferWrites++;
                if (bufferWrites < BufferSize) return;
            }

            try
            {
                // Ensure directory created
                new FileInfo(path).Directory.Create();
                
                // Write to temp file
                File.WriteAllText(path + ".tmp", 
                    JsonConvert.SerializeObject(this.CounterSets, 
                    Formatting.Indented, 
                    new ExcludeSessionDictionaryItemConverter<IDictionary<string, CounterSet>, CounterSet>()));

                // Swap temp to live file
                File.Replace(path + ".tmp", path, null);
                bufferWrites = 0;
                Debug.WriteLine($"{path} wrote to disk");
            }
            catch (Exception)
            {
            }
            await Task.CompletedTask;
        }
        public static async Task Reset(string key, CounterSet.DataType types)
        {
            // Ugh I don't like doing this but it works.  Need to dump the current stopwatch and save it before resetting
            await _instance.Save();
            if (key == null)
            {
                foreach (var item in _instance.CounterSets)
                {
                    item.Value.Reset(types);
                }
            } else
            {
                _instance.CounterSets[key].Reset(types);
            }
            // Now save the reset values
            await _instance.Save();

        }

        public static async Task ResetItem(string key)
        {
            if (key == null) return;
            await _instance.Save();
            foreach (var item in _instance.CounterSets)
            {
                if (item.Value.Counters.ContainsKey(key)) item.Value.Counters[key] = 0;
                if (item.Value.Runtime.ContainsKey(key)) item.Value.Runtime[key] = new TimeSpan();
                if (item.Value.HeroEquipment.ContainsKey(key)) item.Value.HeroEquipment.Remove(key);
                if (item.Value.HeroEquipmentQE.ContainsKey(key)) item.Value.HeroEquipmentQE.Remove(key);
                if (item.Value.Drops.ContainsKey(key)) item.Value.Drops.Remove(key);
                if (item.Value.DropsQE.ContainsKey(key)) item.Value.DropsQE.Remove(key);
            }
            await _instance.Save();
        }

        public static async Task Flush()
        {
            await _instance.Save(CONFIG_PATH, true);
        }
        
    }
}
