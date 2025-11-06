using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Forms;
using WindowsFormsApp1;

namespace UN5ModdingWorkshop
{
    public class ConfigData
    {
        public string GamePath { get; set; } = string.Empty;
        public string Language { get; set; } = "en";
    }

    public static class Config
    {
        private static readonly string ConfigPath = "config.json";
        public static ConfigData Data { get; private set; } = new ConfigData();

        public static void Load(Main form)
        {
            if (File.Exists(ConfigPath))
            {
                string json = File.ReadAllText(ConfigPath);
                Data = JsonConvert.DeserializeObject<ConfigData>(json) ?? new ConfigData();
            }

            form.txtGamePath.Text = Data.GamePath;
            if(Directory.Exists(Data.GamePath))
            {
                CharSel.Create(form, Data.GamePath);
            }
            else
            {
                MessageBox.Show("Game directory not found. Please select it manually. If you haven't extracted it yet, use Game > Extract.");
            }
        }

        public static void Save()
        {
            string json = JsonConvert.SerializeObject(Data, Formatting.Indented);
            File.WriteAllText(ConfigPath, json);
        }
    }
}
