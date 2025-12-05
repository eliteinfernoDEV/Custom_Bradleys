using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Oxide.Core;
using Oxide.Core.Plugins;

namespace Oxide.Plugins
{
    [Info("Custom Bradley", "YourName", "1.0.0")]
    [Description("Spawns a customizable Bradley APC with configurable size, health, damage, and loot")]
    class CustomBradley : RustPlugin
    {
        #region Fields
        private List<BradleyAPC> customBradleys = new List<BradleyAPC>();
        private const string permissionUse = "custombradley.use";
        #endregion

        #region Configuration
        private Configuration config;

        public class Configuration
        {
            public float BradleyScale { get; set; } = 1.0f;
            public float BradleyHealth { get; set; } = 1000f;
            public float BradleyDamage { get; set; } = 10f;
            public bool DisableNPCs { get; set; } = true;
            public bool DisableSmoke { get; set; } = true;
            public List<SpawnLocation> SpawnLocations { get; set; } = new List<SpawnLocation>
            {
                new SpawnLocation
                {
                    Position = new SerializableVector3(0f, 0f, 0f),
                    Rotation = new SerializableVector3(0f, 0f, 0f)
                }
            };
            public List<DropItem> DropItems { get; set; } = new List<DropItem>
            {
                new DropItem { ShortName = "metal.refined", Amount = 100 },
                new DropItem { ShortName = "scrap", Amount = 500 }
            };
            public bool SpawnOnServerStart { get; set; } = true;
        }

        public class SpawnLocation
        {
            public SerializableVector3 Position { get; set; }
            public SerializableVector3 Rotation { get; set; }
        }

        public class SerializableVector3
        {
            public float x { get; set; }
            public float y { get; set; }
            public float z { get; set; }

            public SerializableVector3() { }
            public SerializableVector3(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public Vector3 ToVector3()
            {
                return new Vector3(x, y, z);
            }
        }

        public class DropItem
        {
            public string ShortName { get; set; }
            public int Amount { get; set; }
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            try
            {
                config = Config.ReadObject<Configuration>();
                if (config == null)
                {
                    throw new System.Exception();
                }
            }
            catch
            {
                Config.WriteObject(config, false, $"{Interface.Oxide.ConfigDirectory}/{Name}.jsonError");
                PrintError("The configuration file contains an error and has been replaced with a default config.\n" +
                           "The error configuration file was saved in the .jsonError extension");
                LoadDefaultConfig();
            }

            SaveConfig();
        }

        protected override void LoadDefaultConfig()
        {
            config = new Configuration();
        }

        protected override void SaveConfig()
        {
            Config.WriteObject(config);
        }
        #endregion

        #region Oxide Hooks
        private void Init()
        {
            permission.RegisterPermission(permissionUse, this);
            LoadConfig();
        }

        private void OnServerInitialized()
        {
            if (config.SpawnOnServerStart)
            {
                SpawnAllBradleys();
            }
        }

        private void Unload()
        {
            foreach (var bradley in customBradleys.ToList())
            {
                if (bradley != null && !bradley.IsDestroyed)
                {
                    bradley.Kill();
                }
            }
            customBradleys.Clear();
        }

        private void OnEntitySpawned(BradleyAPC bradley)
        {
            if (bradley == null || !customBradleys.Contains(bradley))
                return;

            NextTick(() =>
            {
                if (bradley == null || bradley.IsDestroyed)
                    return;

                SetupBradley(bradley);
            });
        }

        private void OnEntityDeath(BradleyAPC bradley, HitInfo info)
        {
            if (bradley == null || !customBradleys.Contains(bradley))
                return;

            // Remove default crate spawning
            NextTick(() =>
            {
                var crates = UnityEngine.Object.FindObjectsOfType<LootContainer>()
                    .Where(c => Vector3.Distance(c.transform.position, bradley.transform.position) < 10f)
                    .ToList();

                foreach (var crate in crates)
                {
                    if (crate != null && !crate.IsDestroyed)
                    {
                        crate.Kill();
                    }
                }

                // Spawn custom loot
                SpawnCustomLoot(bradley.transform.position);
            });

            customBradleys.Remove(bradley);

            // Respawn after delay
            timer.Once(300f, () =>
            {
                SpawnAllBradleys();
            });
        }
        #endregion

        #region Core Methods
        private void SpawnAllBradleys()
        {
            // Clear existing bradleys
            foreach (var bradley in customBradleys.ToList())
            {
                if (bradley != null && !bradley.IsDestroyed)
                {
                    bradley.Kill();
                }
            }
            customBradleys.Clear();

            // Spawn new bradleys at configured locations
            foreach (var location in config.SpawnLocations)
            {
                SpawnBradley(location.Position.ToVector3(), location.Rotation.ToVector3());
            }
        }

        private void SpawnBradley(Vector3 position, Vector3 rotation)
        {
            var bradley = GameManager.server.CreateEntity("assets/prefabs/npc/m2bradley/bradleyapc.prefab", position, Quaternion.Euler(rotation)) as BradleyAPC;
            
            if (bradley == null)
            {
                PrintError("Failed to spawn Bradley APC!");
                return;
            }

            bradley.Spawn();
            customBradleys.Add(bradley);

            PrintWarning($"Bradley spawned at {position}");
        }

        private void SetupBradley(BradleyAPC bradley)
        {
            // Set scale
            bradley.transform.localScale = new Vector3(config.BradleyScale, config.BradleyScale, config.BradleyScale);

            // Set health
            bradley.InitializeHealth(config.BradleyHealth, config.BradleyHealth);
            bradley.health = config.BradleyHealth;

            // Modify damage
            var mainCannon = bradley.GetComponent<NPCAutoTurret>();
            if (mainCannon != null)
            {
                mainCannon.bulletDamage = config.BradleyDamage;
            }

            // Disable NPCs if configured
            if (config.DisableNPCs)
            {
                bradley.CancelInvoke(bradley.SpawnScientists);
            }

            // Disable smoke if configured
            if (config.DisableSmoke)
            {
                var smokeEmitters = bradley.GetComponentsInChildren<ParticleSystem>();
                foreach (var emitter in smokeEmitters)
                {
                    if (emitter.name.Contains("smoke", System.StringComparison.OrdinalIgnoreCase))
                    {
                        emitter.Stop();
                        emitter.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void SpawnCustomLoot(Vector3 position)
        {
            foreach (var dropItem in config.DropItems)
            {
                var item = ItemManager.CreateByName(dropItem.ShortName, dropItem.Amount);
                if (item != null)
                {
                    item.Drop(position + new Vector3(0, 1, 0), Vector3.up * 2f);
                }
                else
                {
                    PrintError($"Failed to create item: {dropItem.ShortName}");
                }
            }
        }
        #endregion

        #region Commands
        [ChatCommand("custombradley.spawn")]
        private void SpawnBradleyCommand(BasePlayer player, string command, string[] args)
        {
            if (!permission.UserHasPermission(player.UserIDString, permissionUse))
            {
                SendReply(player, "You don't have permission to use this command.");
                return;
            }

            SpawnAllBradleys();
            SendReply(player, $"Spawned {config.SpawnLocations.Count} custom Bradley(s)!");
        }

        [ChatCommand("custombradley.remove")]
        private void RemoveBradleyCommand(BasePlayer player, string command, string[] args)
        {
            if (!permission.UserHasPermission(player.UserIDString, permissionUse))
            {
                SendReply(player, "You don't have permission to use this command.");
                return;
            }

            int count = customBradleys.Count;
            foreach (var bradley in customBradleys.ToList())
            {
                if (bradley != null && !bradley.IsDestroyed)
                {
                    bradley.Kill();
                }
            }
            customBradleys.Clear();

            SendReply(player, $"Removed {count} custom Bradley(s)!");
        }

        [ChatCommand("custombradley.addlocation")]
        private void AddLocationCommand(BasePlayer player, string command, string[] args)
        {
            if (!permission.UserHasPermission(player.UserIDString, permissionUse))
            {
                SendReply(player, "You don't have permission to use this command.");
                return;
            }

            var location = new SpawnLocation
            {
                Position = new SerializableVector3(player.transform.position.x, player.transform.position.y, player.transform.position.z),
                Rotation = new SerializableVector3(0f, player.transform.rotation.eulerAngles.y, 0f)
            };

            config.SpawnLocations.Add(location);
            SaveConfig();

            SendReply(player, $"Added spawn location at your position: {player.transform.position}");
        }
        #endregion
    }
}
