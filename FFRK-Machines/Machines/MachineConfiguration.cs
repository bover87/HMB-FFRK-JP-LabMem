﻿using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FFRK_Machines.Machines
{
    /// <summary>
    /// Base machine configuration class
    /// </summary>
    public class MachineConfiguration
    {

        public MachineConfiguration() {}

        public async Task Save(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
            await Task.CompletedTask;
        }

        public static async Task<T> Load<T>(string path) where T:MachineConfiguration
        {
            var config = await Task.FromResult(JsonConvert.DeserializeObject<T>(File.ReadAllText(path), new JsonSerializerSettings() { Error = HandleDeserializationError }));
            config.Migrate();
            return config;
        }

        protected static void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            ColorConsole.WriteLine(ConsoleColor.Red, "Error reading configuration: {0}", e.ErrorContext.Error.Message);
            e.ErrorContext.Handled = true;
        }

        /// <summary>
        /// Called just after config is loaded from disk to perform any backwards-compatible migrations
        /// </summary>
        protected virtual void Migrate() {}

    }
}
