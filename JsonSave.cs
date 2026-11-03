using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;

namespace game_project_3
{
    public class GemSaveData
    {
        public float X { get; set; }
        public float Y { get; set; }
        public bool Collected { get; set; }
    }

    public class JsonSave
    {
        public static string DefaultPath =>
            Path.Combine(AppContext.BaseDirectory, "save.json");

        private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

        public float PlayerX { get; set; }
        public float PlayerY { get; set; }
        public int GemsCollected { get; set; }
        public List<GemSaveData> Gems { get; set; } = new();

        // Required for deserialization
        public JsonSave() { }

        public JsonSave(Vector2 playerPosition, int gemsCollected, IEnumerable<GemSprite> gems, IEnumerable<Vector2> gemPoints)
        {
            PlayerX = playerPosition.X;
            PlayerY = playerPosition.Y;
            GemsCollected = gemsCollected;

            // gems and gemPoints are built in the same order in Game1.LoadContent
            using var pointEnum = gemPoints.GetEnumerator();
            foreach (var gem in gems)
            {
                if (!pointEnum.MoveNext()) break;
                Gems.Add(new GemSaveData
                {
                    X = pointEnum.Current.X,
                    Y = pointEnum.Current.Y,
                    Collected = gem.Collected
                });
            }
        }

        public Vector2 PlayerPosition => new Vector2(PlayerX, PlayerY);

        public void SaveToFile(string path = null)
        {
            File.WriteAllText(path ?? DefaultPath, JsonSerializer.Serialize(this, Options));
        }

        public static JsonSave LoadFromFile(string path = null)
        {
            path ??= DefaultPath;
            if (!File.Exists(path)) return null;

            try
            {
                return JsonSerializer.Deserialize<JsonSave>(File.ReadAllText(path));
            }
            catch (JsonException)
            {
                return null; // corrupt save file
            }
        }

        public static bool DeleteSave(string path = null)
        {
            path ??= DefaultPath;
            if (!File.Exists(path)) return false;

            File.Delete(path);
            return true;
        }

        public static bool SaveExists(string path = null)
        {
            return File.Exists(path ?? DefaultPath);
        }
    }
}