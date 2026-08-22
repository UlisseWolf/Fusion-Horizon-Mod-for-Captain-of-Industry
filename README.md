# Fusion Horizon

A mod for [**Captain of Industry**](https://store.steampowered.com/app/1594320/Captain_of_Industry/) that unlocks nuclear fusion and a full extended nuclear tech tree — from CANDU-style heavy-water fission reactors through fuel reprocessing to a working fusion reactor, plus late-game applications in nuclear medicine.

- **Mod ID:** `Fusion_Horizon`
- **Author:** Ulisse Wolf
- **Minimum game version:** 0.8.6
- **License:** [COI-Open v1.0](LICENSE)
- **Dependencies:** `worldgen-plus-plus`, `recipes-plus-plus` (distributed through the in-game [Mods browser](https://coigame.com/Mods), not GitHub)

> Fusion Horizon started as a Python/CustomAssets mod and has since been fully ported to a native C# DLL mod, following the format described in [Captain-of-industry-modding](https://github.com/MaFi-Games/Captain-of-industry-modding). The legacy version is kept in [`old/`](old) for reference and save-compatibility history; all active development happens in [`src/`](src).

## Features

### Fission — the CANDU line
- **CANDU I / II / III** heavy-water nuclear reactors, chained as real in-place upgrade tiers (CANDU I → II via `SetNextTier`).
- **CANDU III** runs its entire coolant/steam circuit on tritium-rich heavy water instead of the regular kind, requiring a **Tritium Separator** to recover both Tritium and reusable Super-Pressurized Heavy Water.
- **DUPIC fuel reprocessing** (*Direct Use of spent PWR fuel In CANDU*): mechanically reprocesses vanilla Spent Fuel into DUPIC Rods via the real-world OREOX process, without ever chemically separating plutonium. Burns only in CANDU II/III; the resulting DUPIC Spent Fuel is isotopically degraded and instead feeds a Fast Breeder Reactor's breeding blanket.
- **Isotope Separation**: an endgame-of-fission technology that separates fission platinum-group metals and sealed Cs-137/Sr-90 sources out of vanilla Fission Products, yielding reusable Industrial Isotopes and Depleted Fission Products.

### Fusion
- A working **Fusion Nuclear Reactor**, fed by a full Deuterium/Tritium/Helium/Plasma production chain, generating up to 240 MW.
- **Industrial Plasma**: purified plasma for non-nuclear industrial use — powers a plasma-fueled Incinerator II and sealed Plasma Modules.

### Nuclear medicine
- **Medical Supplies IV**, the most advanced tier of medical care, combining Industrial Isotopes (nuclear medicine — diagnostics, radiotherapy) with Plasma Modules (advanced plasma-based medical equipment). Feeds directly into the vanilla Clinic via the same dynamic `MedicalSuppliesParam` mechanism vanilla tiers I–III use.

### Everything else
- New custom machines (Heat Exchanger, Tritium Separator) alongside recipes bound to several existing vanilla machines (Nuclear Reprocessing Plant, Chemical Plant II, Assembly Plant) rather than inventing redundant buildings.
- Full localization: **English, Italian, French, Spanish, German, Portuguese.**

## Installation

1. Make sure the dependencies (`worldgen-plus-plus`, `recipes-plus-plus`) are installed first.
2. Grab the latest release, or build from source (see below).
3. Drop the mod folder into your Captain of Industry `Mods` directory so that `manifest.json` sits at `.../Mods/Fusion_Horizon/manifest.json`.
4. Enable **Fusion Horizon** from the in-game mod list.

## Building from source

The active mod lives in [`src/FusionHorizonDLL`](src/FusionHorizonDLL) and follows the standard [Captain-of-industry-modding](https://github.com/MaFi-Games/Captain-of-industry-modding) DLL mod layout.

```
git clone <this-repo>
cd src/FusionHorizonDLL
dotnet build
```

By default the project builds straight into your local Mods folder (`%APPDATA%\Captain of Industry\Mods\Fusion_Horizon\`) via `DeployToModsFolder` in the `.csproj` — set it to `false` if you'd rather deploy manually. Some assets (custom 3D models and textures baked into `AssetBundles/`) are produced by a companion Unity project, not by `dotnet build` alone; see the comments in `Source/Data/ProductData.cs` for which products currently use placeholder vs. dedicated models.

## Repository layout

```
├── src/
│   └── FusionHorizonDLL/     — active C# DLL mod (this is what you build/ship)
│       ├── Source/           — mod logic (products, recipes, research, machines)
│       ├── Assets/           — source textures/models feeding the Unity companion project
│       ├── AssetBundles/     — compiled Unity AssetBundles referenced by the mod
│       └── Translations/     — en / it / fr / es / de / pt JSON translation files
└── old/
    └── Fusion_Horizon/       — legacy CustomAssets/Python version, kept for reference
```

## Localization

Translation files live under `src/FusionHorizonDLL/Translations/<lang>.json`, keyed by a stable `<category>.<protoId>.<field>` convention (e.g. `research.Research_Isotope_Separation.name`) rather than by the English source text, so editing English copy later never silently breaks a translation lookup. Contributions for additional languages are welcome — just mirror the key set in `en.json`.

## License

This project is licensed under the **Captain of Industry Open License (COI-Open) v1.0** — see [`LICENSE`](LICENSE) for the full text, or the canonical source at [coigame.com/Legal/CoI-Open](https://coigame.com/Legal/CoI-Open).

In short: this Work may be freely used, modified, and shared within the Captain of Industry modding community — but only in connection with Captain of Industry itself, never repurposed into an unrelated project. Any modified version must remain under this same license, and credit to the original author must be preserved.

## Credits

- **Ulisse Wolf** — original CustomAssets/Python mod and DLL conversion.
- Built against the [Captain-of-industry-modding](https://github.com/MaFi-Games/Captain-of-industry-modding) template, in the style of **CoI.MetallurgyPlus**.
