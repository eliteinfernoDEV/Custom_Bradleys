# Custom Bradley

A highly customizable Rust Oxide plugin that allows server administrators to spawn Bradley APCs with configurable size, health, damage, and loot drops.

## Features

- 🎯 **Scalable Size** - Adjust Bradley APC size from tiny to massive
- ❤️ **Configurable Health** - Set custom health values for desired difficulty
- ⚔️ **Adjustable Damage** - Control weapon damage output
- 🚫 **No NPCs** - Optionally disable scientist spawns
- 💨 **No Smoke Effects** - Remove smoke particles for cleaner visuals
- 📍 **Custom Spawn Locations** - Define exact spawn positions and rotations
- 🎁 **Custom Loot Drops** - Specify items and amounts instead of default crates
- 🔄 **Auto Respawn** - Automatically respawns Bradley after death (configurable timer)

## Installation

1. Download `CustomBradley.cs`
2. Place the file in your `oxide/plugins` directory
3. The plugin will automatically generate a configuration file on first load
4. Reload the plugin or restart your server

## Permissions

```
custombradley.use
```

Grant this permission to allow players/admins to use the plugin commands.

## Commands

| Command | Description | Permission Required |
|---------|-------------|-------------------|
| `/custombradley.spawn` | Spawns Bradley APCs at all configured locations | custombradley.use |
| `/custombradley.remove` | Removes all custom Bradley APCs | custombradley.use |
| `/custombradley.addlocation` | Adds your current position as a spawn location | custombradley.use |

## Configuration

The configuration file is automatically generated at `oxide/config/CustomBradley.json`

### Default Configuration

```json
{
  "BradleyScale": 1.0,
  "BradleyHealth": 1000.0,
  "BradleyDamage": 10.0,
  "DisableNPCs": true,
  "DisableSmoke": true,
  "SpawnLocations": [
    {
      "Position": {
        "x": 0.0,
        "y": 0.0,
        "z": 0.0
      },
      "Rotation": {
        "x": 0.0,
        "y": 0.0,
        "z": 0.0
      }
    }
  ],
  "DropItems": [
    {
      "ShortName": "metal.refined",
      "Amount": 100
    },
    {
      "ShortName": "scrap",
      "Amount": 500
    }
  ],
  "SpawnOnServerStart": true
}
```

### Configuration Options

#### Basic Settings

- **BradleyScale** (float, default: `1.0`)
  - Scale multiplier for Bradley size
  - Example: `2.0` = double size, `0.5` = half size

- **BradleyHealth** (float, default: `1000.0`)
  - Total health points for the Bradley
  - Higher values make it harder to destroy

- **BradleyDamage** (float, default: `10.0`)
  - Damage per shot from Bradley's weapons
  - Adjust for desired difficulty

- **DisableNPCs** (bool, default: `true`)
  - When `true`, prevents scientists from spawning
  - Set to `false` to enable NPC spawns

- **DisableSmoke** (bool, default: `true`)
  - When `true`, removes smoke particle effects
  - Set to `false` to enable smoke effects

- **SpawnOnServerStart** (bool, default: `true`)
  - When `true`, automatically spawns Bradleys on server start
  - Set to `false` to manually spawn using commands

#### Spawn Locations

Define multiple spawn points for Bradley APCs:

```json
"SpawnLocations": [
  {
    "Position": {
      "x": 100.0,
      "y": 50.0,
      "z": -200.0
    },
    "Rotation": {
      "x": 0.0,
      "y": 45.0,
      "z": 0.0
    }
  },
  {
    "Position": {
      "x": -500.0,
      "y": 30.0,
      "z": 300.0
    },
    "Rotation": {
      "x": 0.0,
      "y": 180.0,
      "z": 0.0
    }
  }
]
```

**Tip:** Use `/custombradley.addlocation` in-game to easily add spawn points at your current position.

#### Custom Loot Drops

Define what items drop when the Bradley is destroyed:

```json
"DropItems": [
  {
    "ShortName": "rifle.ak",
    "Amount": 1
  },
  {
    "ShortName": "ammo.rifle",
    "Amount": 128
  },
  {
    "ShortName": "metal.refined",
    "Amount": 200
  },
  {
    "ShortName": "scrap",
    "Amount": 1000
  }
]
```

**Note:** Items drop directly on the ground, replacing the default crate spawns.

### Common Item Short Names

| Item | Short Name |
|------|------------|
| High Quality Metal | `metal.refined` |
| Scrap | `scrap` |
| AK-47 | `rifle.ak` |
| Bolt Action Rifle | `rifle.bolt` |
| Rocket Launcher | `rocket.launcher` |
| Explosives | `explosives` |
| C4 | `explosive.timed` |
| Metal Fragments | `metal.fragments` |

For a complete list, see [Rust Item List](https://www.corrosionhour.com/rust-item-list/).

## Usage Examples

### Example 1: Giant Boss Bradley

```json
{
  "BradleyScale": 3.0,
  "BradleyHealth": 10000.0,
  "BradleyDamage": 50.0,
  "DisableNPCs": true,
  "DisableSmoke": true,
  "DropItems": [
    {
      "ShortName": "rifle.ak",
      "Amount": 3
    },
    {
      "ShortName": "explosive.timed",
      "Amount": 5
    },
    {
      "ShortName": "scrap",
      "Amount": 5000
    }
  ]
}
```

### Example 2: Mini Bradley Swarm

Create multiple small, weaker Bradleys:

```json
{
  "BradleyScale": 0.5,
  "BradleyHealth": 300.0,
  "BradleyDamage": 5.0,
  "SpawnLocations": [
    {"Position": {"x": 100, "y": 50, "z": 100}, "Rotation": {"x": 0, "y": 0, "z": 0}},
    {"Position": {"x": 110, "y": 50, "z": 100}, "Rotation": {"x": 0, "y": 0, "z": 0}},
    {"Position": {"x": 120, "y": 50, "z": 100}, "Rotation": {"x": 0, "y": 0, "z": 0}}
  ],
  "DropItems": [
    {
      "ShortName": "scrap",
      "Amount": 200
    }
  ]
}
```

### Example 3: PvP Event Bradley

```json
{
  "BradleyScale": 1.5,
  "BradleyHealth": 3000.0,
  "BradleyDamage": 25.0,
  "DisableNPCs": true,
  "DisableSmoke": false,
  "DropItems": [
    {
      "ShortName": "rifle.ak",
      "Amount": 2
    },
    {
      "ShortName": "rifle.lr300",
      "Amount": 1
    },
    {
      "ShortName": "ammo.rifle",
      "Amount": 256
    },
    {
      "ShortName": "metal.refined",
      "Amount": 500
    }
  ],
  "SpawnOnServerStart": false
}
```

## How It Works

1. **Spawning**: Bradleys spawn at configured locations when the server starts (if enabled) or when using the spawn command
2. **Customization**: All visual and gameplay properties are applied when the Bradley spawns
3. **Death Handling**: When destroyed, default loot crates are removed and replaced with your configured items
4. **Auto Respawn**: After 5 minutes (300 seconds), the Bradley automatically respawns at its original location

## Troubleshooting

### Bradley doesn't spawn
- Check that spawn coordinates are valid (not in rock/water)
- Ensure the plugin is loaded: `oxide.reload CustomBradley`
- Check server console for errors

### Loot doesn't drop
- Verify item short names are correct
- Check server console for "Failed to create item" errors
- Ensure items are valid in your Rust version

### Bradley is invisible or glitched
- Try adjusting the scale (avoid extreme values like 0.01 or 100)
- Restart the server after config changes
- Remove and respawn using commands

## Compatibility

- **Rust Version**: Latest stable
- **Oxide Version**: 2.0+
- **Dependencies**: None

## Support

If you encounter issues or have suggestions:
- Open an issue on GitHub
- Check existing issues for solutions
- Include your config file and server console errors when reporting bugs

## License

This plugin is provided as-is for use on Rust servers. Feel free to modify for your server's needs.

## Credits

Created for the Rust community. Contributions and improvements welcome!

---

**⚠️ Important Notes:**
- Always test configuration changes in a development environment first
- Backup your config before making major changes
- Some features may require server restart to take full effect
- Custom Bradley spawns do not replace the default Launch Site Bradley
