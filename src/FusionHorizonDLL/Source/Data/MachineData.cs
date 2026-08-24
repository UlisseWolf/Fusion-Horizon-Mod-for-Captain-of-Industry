using Mafi;
using Mafi.Base;
using Mafi.Collections.ImmutableCollections;
using Mafi.Core.Entities.Static;
using Mafi.Core.Entities.Static.Layout;
using Mafi.Core.Factory.Machines;
using Mafi.Core.Factory.NuclearReactors;
using Mafi.Core.Mods;
using Mafi.Core.Ports.Io;
using Mafi.Core.Products;
using Mafi.Core.Prototypes;
using FusionHorizon.Extensions;

namespace FusionHorizon.Data;

// Registers every custom machine and nuclear reactor. Two different registration paths:
//  - Ordinary machines: MachineProtoBuilder (.Start(name, id).Description(...)
//    .SetCategories(...).SetLayout(...).SetPrefabPath(...).BuildAndAdd()).
//  - Nuclear reactors: NuclearReactorProto has no fluent builder — it derives from
//    StaticEntityProto rather than MachineProto, so it's constructed directly with
//    `new NuclearReactorProto(...)` and added via registrator.PrototypesDb.Add(...),
//    the same way the base game itself registers NuclearReactor/NuclearReactorT2/
//    FastBreederReactor (see Mafi.Base.Prototypes.Buildings.NuclearReactorsData).
//
// SetLayout(...) takes an ASCII grid: each `[N]` is a tile type, port letters (A, B, C,
// ...) mark input (>) / output (<) connections with a shape suffix (@ fluid, # unit item,
// ~ loose material — shapes aren't universal across every machine, always check what
// existing recipes on that machine actually use before assuming). Layouts and prefab
// paths for reused vanilla machines/reactors below are copied verbatim from the base
// game's own registration code so the in-game building looks and connects identically.
internal class MachineData : IModData {

	public void RegisterData(ProtoRegistrator registrator) {
		RegisterHeatExchanger(registrator);
		RegisterIncinerationPlantT2(registrator);
		RegisterTritiumSeparator(registrator);
		RegisterCanduReactors(registrator);
		RegisterFusionReactor(registrator);
	}

	// Reuses vanilla ThermalDesalinator's layout/prefab (same building, none of its
	// desalination recipes — this mod binds its own steam-generation recipes instead,
	// see RecipesData.cs).
	private void RegisterHeatExchanger(ProtoRegistrator registrator) {
		registrator.MachineProtoBuilder
			.Start(
				ModTranslation.Get("build-machine.Heat_exchanger_candu.name", "Heat Exchanger"),
				ModIDs.Machines.HeatExchangerCandu)
			.Description(ModTranslation.Get("build-machine.Heat_exchanger_candu.description",
				"A heat exchanger is a device that transfers thermal energy between two fluids " +
				"while keeping them separate, allowing a circuit to be heated or cooled without " +
				"mixing the substances involved."))
			.SetCategories(Ids.ToolbarCategories.Water)
			.SetLayout(
				"A@>[3][3][3][3][3][3][3][3][3][3][3]>@W",
				"   [3][3][3][3][3][3][3][4][3][3][3]   ",
				"   [4][4][4][4][4][4][4][4][4][4][3]>@X",
				"B@>[3][3][3][3][3][3][3][3][3][3][3]   ")
			.SetPrefabPath("Assets/Base/Machines/Water/ThermalDesalinator.prefab")
			.BuildAndAdd();
	}

	// Reuses vanilla IncinerationPlant's layout/prefab, plus one extra output port ("Z",
	// loose material) not present on the original. There's no builder method to add a
	// port after SetLayout(...), so LayoutEntityExtensions.AddExtraPorts (a small
	// reflection helper, see Source/Extensions) patches the built proto directly — the
	// same technique other DLL mods use when a builder doesn't expose something the
	// underlying proto actually supports.
	private void RegisterIncinerationPlantT2(ProtoRegistrator registrator) {
		var proto = registrator.MachineProtoBuilder
			.Start(
				ModTranslation.Get("build-machine.Incineration_plant_T2.name", "Incineration Plant II"),
				ModIDs.Machines.IncinerationPlantT2)
			.Description(ModTranslation.Get("build-machine.Incineration_plant_T2.description",
				"It incinerates waste with far greater efficiency than a traditional burner and " +
				"incinerator. The process has a positive energy balance and produces steam. " +
				"Thanks to the plasma, no pollution is generated, only slag from inorganic waste."))
			.SetCategories(Ids.ToolbarCategories.Waste_Solid)
			.SetLayout(
				"   [2][2][2][5][5][5][8][8][8][8][8][8][8][6]   ",
				"C@>[2][2][2][5][5][5][8][8][8][8][8][8][8][6]>@Y",
				"   [2][2][2][5][5][5][8][8][8][8][8][8][8][6]   ",
				"   [2][7][7][7][7][7][8][8][8][8][8][8][8][6]   ",
				"A#>[2][7][7][7][7][7][8][8][8][8][8][8][8][6]   ",
				"B~>[2][7][7][7][7][7][8][8][8][8][8][8][8][6]   ",
				"   [2][7][7][7][7][7][8][8][8][8][8][8][8][6]   ",
				"D@>[2][2][2][3][3][3][8][8][8][8][8][8][8][6]>@X")
			.SetPrefabPath("Assets/Base/Machines/Waste/IncinerationPlant.prefab")
			.BuildAndAdd();

		IoPortShapeProto looseConveyorShape = registrator.PrototypesDb
			.GetOrThrow<IoPortShapeProto>(Ids.IoPortShapes.LooseMaterialConveyor);

		proto.AddExtraPorts(new IoPortTemplate(
			spec: new PortSpec('Z', IoPortType.Output, looseConveyorShape, canOnlyConnectToTransports: false),
			relativePosition: new RelTile3i(8, 0, 0),
			relativeDirection: Direction90.MinusY));
	}

	// Placeholder machine, reusing vanilla ElectrolyzerT2's layout/prefab (one fluid
	// input, two fluid outputs — matches what this recipe needs). Splits CANDU 3's
	// tritium-rich coolant back into pure Tritium and regular high-pressure Heavy Water.
	private void RegisterTritiumSeparator(ProtoRegistrator registrator) {
		registrator.MachineProtoBuilder
			.Start(
				ModTranslation.Get("build-machine.Tritium_separator.name", "Tritium Separator"),
				ModIDs.Machines.TritiumSeparator)
			.Description(ModTranslation.Get("build-machine.Tritium_separator.description",
				"Separates the tritium-rich, high-pressure heavy water drawn from a CANDU 3 " +
				"reactor's coolant loop into pure Tritium and regular high-pressure Heavy Water, " +
				"ready to be reused."))
			.SetCategories(Ids.ToolbarCategories.Power_Nuclear)
			.SetLayout(
				"   [3][3][3][3][3][3][3][3]   ",
				"   [3][3][3][3][3][3][3][3]>@X",
				"A@>[3][3][3][3][3][3][3][3]   ",
				"   [3][3][3][3][3][3][3][3]>@Y",
				"   [3][3][3][3][3][3][3][3]   ")
			.SetPrefabPath("Assets/Base/Machines/Water/ElectrolyzerT2.prefab")
			.BuildAndAdd();
	}

	// CANDU I/II/III heavy-water fission reactors. Reuses vanilla NuclearReactor /
	// NuclearReactorT2's own layouts/prefabs. NuclearReactorProto models two independent
	// water loops: waterInPerStep/steamOutPerStep (the main power-generating steam loop,
	// ports set by waterInPorts/steamOutPorts) and coolantIn/coolantOut (a secondary
	// core-cooling loop, its own dedicated ports) — both run on heavy water here instead
	// of vanilla's plain water, which is what makes this a "CANDU-style" reactor.
	private void RegisterCanduReactors(ProtoRegistrator registrator) {
		var protosDb = registrator.PrototypesDb;

		ProductProto uraniumRod = protosDb.GetOrThrow<ProductProto>(Ids.Products.UraniumRod);
		ProductProto spentFuel = protosDb.GetOrThrow<ProductProto>(Ids.Products.SpentFuel);
		ProductProto moxRod = protosDb.GetOrThrow<ProductProto>(Ids.Products.MoxRod);
		ProductProto spentMox = protosDb.GetOrThrow<ProductProto>(Ids.Products.SpentMox);
		ProductProto dupicRod = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.DupicRod);
		ProductProto dupicSpentFuel = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.DupicSpentFuel);
		ProductProto heavyWater = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.HeavyWater);
		ProductProto heavyWaterHigh = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.HeavyWaterHigh);
		ProductProto heavyWaterHighTritium = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.HeavyWaterHighTritium);
		ProductProto canduRod = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.CanduRod);

		// Layout shared by T1/T2/T3 (T3 reuses T2's model as a placeholder). "-N]" tokens
		// are custom underground/foundation tiles, defined per tier below via
		// EntityLayoutParams/CustomLayoutToken — copied from vanilla's own reactor data.
		string[] layout = new string[] {
			"      [4][4][4][7][7][7][7][7][7][7][7][5][5][5]   ",
			"      [4][5][5][7][7][7][7][7][7][7][7][5][5][5]   ",
			"      [4][5][6][7][7][7][7][7][7][7][7][6][5][5]   ",
			"      [4][5][6][7][7][7][7][7][7][7][7][6][5][5]   ",
			"      [4][4][4][7][7][7][7][7][7][7][7][5][5][5]   ",
			"      [4][4][4][7][7][7][7][7][7][7][7][5][5][5]   ",
			"      [4][4][4][7][7][7][7][7][7][7][7][5][5][5]   ",
			"      [4][4][4][7][7][7][7][7][7][7][7][5][5][5]   ",
			"      [4][4][4][7][7][7][7][7][7][7][7][5][5][5]   ",
			"      [4][4][4][7][7][7][7][7][7][7][7][5][5][5]   ",
			"      [4][4][6][7][7][7][7][7][7][7][7][5][5][5]   ",
			"   [3][4][4][6][7][7][7][7][7][7][7][7][5][5][5]   ",
			"F#>[3][4][4][4][7][7][7][7][7][7][7][7][5][5][5]   ",
			"   [3][4][4][4][7][7][7][7][7][7][7][7][5][5][5]   ",
			"S#<[3][4][4][4][4][4][6][6][6][6][4][4][4][4][4]   ",
			"   [3][4][4][4][4][5][5][5][5][5][5][4][4][4][4][3]",
			"   [3][4][4][4][6][6][6][6][6][6][6][5][4][4][4][3]",
			"   [3][4][4][6][6]-3]-3]-3]-3][6][6][6][5][5][4][3]",
			"   [3][4][5][6]-3]-3]-5]-5]-3]-3]-3][6][6][5][4][3]",
			"   [3][4]-3]-3]-3]-5]-5]-5]-5]-5]-3]-3]-3][5][4][3]",
			"   [3][4]-4]-4]-5]-5]-5]-5]-5]-5]-4]-4]-3][5][4][3]",
			"   [3][4]-4]-4]-5]-5]-5]-5]-5]-5]-4]-4]-3][5][4][3]",
			"   [3][4]-3]-3]-5]-5]-5]-5]-5]-5]-3]-3]-3][5][4][3]",
			"   [3][4][5]-3]-3]-5]-5]-5]-5]-5]-3][6][6][5][4][3]",
			"   [3][4][4][5]-3]-3]-3]-3]-3]-3]-3][6][5][5][4][3]",
			"      [4][4][4][5][6][6][6][6][6][6][5][4][4][4]   ",
			"         [4][4][4][5][5][5][5][5][5][4][4][4]      ",
			"      D@>[4][4][4][4][4][4][4][4][4][4][4][4]W@>   ",
			"         [4][4][4][4][4][4][4][4][4][4][4][4]      ",
			"      A@>[4][4][4][4][4][4][4][4][4][4][4][4]X@>   ",
			"      B@>[4][4][4][4][4][4][4][4][4][4][4][4]Y@>   ",
			"         [4][4][4][4][4][4][4][4][4][4][4][4]      "
		};

		var layoutParamsT1 = new EntityLayoutParams(null, new CustomLayoutToken[] {
			new CustomLayoutToken("-0]", (EntityLayoutParams p, int h) =>
				new LayoutTokenSpec(-h, 5, LayoutTileConstraint.None, -h))
		});
		var layoutParamsT2 = new EntityLayoutParams(null, new CustomLayoutToken[] {
			new CustomLayoutToken("-0]", (EntityLayoutParams p, int h) =>
				new LayoutTokenSpec(-h, 6, LayoutTileConstraint.None, -h))
		});

		// -------------------- CANDU T1 --------------------
		NuclearReactorProto canduT1 = registrator.PrototypesDb.Add(new NuclearReactorProto(
			id: ModIDs.Machines.CanduReactorT1,
			strings: Proto.CreateStr(ModIDs.Machines.CanduReactorT1,
				ModTranslation.Get("nuclear-reactor.Candu_Reactor_T1_build.name", "CANDU Nuclear Reactor"),
				ModTranslation.Get("nuclear-reactor.Candu_Reactor_T1_build.description",
					"A thermal CANDU reactor that sustains a nuclear chain reaction using enriched " +
					"uranium rods or natural uranium rods. The reaction releases a large amount of " +
					"energy, which is used to generate steam. At maximum power, this plant can " +
					"generate up to 90 MW of electricity. Warning: Spent fuel is radioactive and can " +
					"pose a health risk to the public if not stored in a specialized facility.")),
			layout: registrator.LayoutParser.ParseLayoutOrThrow(layoutParamsT1, layout),
			costs: Costs.Buildings.NuclearReactor.MapToEntityCosts(registrator),
			maxPowerLevel: 3,
			fuelCapacity: new Quantity(40),
			minFuelToOperate: new Quantity(16),
			waterInPerStep: heavyWater.WithQuantity(16),
			steamOutPerStep: heavyWaterHigh.WithQuantity(16),
			waterInPorts: "AB",
			steamOutPorts: "XY",
			processDuration: 10.Seconds(),
			fuelPairs: ImmutableArray.Create(
				new NuclearReactorProto.FuelData(uraniumRod, spentFuel, 120.Seconds()),
				new NuclearReactorProto.FuelData(canduRod, spentFuel, 60.Seconds())),
			fuelInPort: 'F',
			fuelOutPort: 'S',
			coolantIn: heavyWater,
			coolantOut: heavyWaterHigh,
			coolantInPort: 'D',
			coolantOutPort: 'W',
			leakRadiationOnMeltdown: true,
			destroyFuelOnMeltdown: false,
			computingConsumed: Computing.Zero,
			enrichment: Option.None,
			graphics: new NuclearReactorProto.Gfx(
				prefabPath: "Assets/Base/Buildings/NuclearReactors/NuclearReactorT1.prefab",
				categories: registrator.GetCategoriesProtos(Ids.ToolbarCategories.Power_Nuclear),
				soundPrefabPath: "Assets/Base/Buildings/NuclearReactors/Shared/ReactorSound.prefab",
				fuelIconPath: "Assets/Base/Buildings/NuclearReactors/Shared/MoxReactorFuelIcon.svg",
				maxEmissionIntensity: 3.5f)));

		// -------------------- CANDU T2 --------------------
		NuclearReactorProto canduT2 = registrator.PrototypesDb.Add(new NuclearReactorProto(
			id: ModIDs.Machines.CanduReactorT2,
			strings: Proto.CreateStr(ModIDs.Machines.CanduReactorT2,
				ModTranslation.Get("nuclear-reactor.Candu_Reactor_T2_Build.name", "CANDU Nuclear Reactor II"),
				ModTranslation.Get("nuclear-reactor.Candu_Reactor_T2_Build.description",
					"An advanced thermal CANDU reactor with higher energy efficiency, capable of using " +
					"MOX fuel and automatically adjusting its power level (if rated power is " +
					"available). At maximum power, this plant can generate up to 120 MW of electricity.")),
			layout: registrator.LayoutParser.ParseLayoutOrThrow(layoutParamsT2, layout),
			costs: Costs.Buildings.NuclearReactorT2.MapToEntityCosts(registrator),
			maxPowerLevel: 4,
			fuelCapacity: new Quantity(40),
			minFuelToOperate: new Quantity(16),
			waterInPerStep: heavyWater.WithQuantity(16),
			steamOutPerStep: heavyWaterHigh.WithQuantity(16),
			waterInPorts: "AB",
			steamOutPorts: "XY",
			processDuration: 10.Seconds(),
			fuelPairs: ImmutableArray.Create(
				new NuclearReactorProto.FuelData(moxRod, spentMox, 120.Seconds()),
				new NuclearReactorProto.FuelData(uraniumRod, spentFuel, 120.Seconds()),
				new NuclearReactorProto.FuelData(canduRod, spentFuel, 60.Seconds()),
				new NuclearReactorProto.FuelData(dupicRod, dupicSpentFuel, 80.Seconds())),
			fuelInPort: 'F',
			fuelOutPort: 'S',
			coolantIn: heavyWater,
			coolantOut: heavyWaterHigh,
			coolantInPort: 'D',
			coolantOutPort: 'W',
			leakRadiationOnMeltdown: true,
			destroyFuelOnMeltdown: false,
			computingConsumed: Computing.FromTFlops(12),
			enrichment: Option.None,
			graphics: new NuclearReactorProto.Gfx(
				prefabPath: "Assets/Base/Buildings/NuclearReactors/NuclearReactorT2.prefab",
				categories: registrator.GetCategoriesProtos(Ids.ToolbarCategories.Power_Nuclear),
				soundPrefabPath: "Assets/Base/Buildings/NuclearReactors/Shared/ReactorSound.prefab",
				fuelIconPath: "Assets/Base/Buildings/NuclearReactors/Shared/MoxReactorFuelIcon.svg",
				maxEmissionIntensity: 3.5f)));

		// A real in-place upgrade tier, same pattern vanilla uses for
		// NuclearReactor -> NuclearReactorT2 (StaticEntityProto.SetNextTier(...)).
		canduT1.SetNextTier(canduT2);

		// -------------------- CANDU T3 --------------------
		// Reuses CANDU T2's layout/prefab as a placeholder. Unlike T1/T2 (where all 3
		// water ports per side carry the same fluid), CANDU 3's whole circuit — main loop
		// (X/Y) and coolant loop (W) alike — outputs the tritium-rich variant, requiring
		// the Tritium Separator to recover usable Heavy Water and Tritium from it.
		NuclearReactorProto canduT3 = registrator.PrototypesDb.Add(new NuclearReactorProto(
			id: ModIDs.Machines.CanduReactorT3,
			strings: Proto.CreateStr(ModIDs.Machines.CanduReactorT3,
				ModTranslation.Get("nuclear-reactor.Candu_Reactor_T3_build.name", "CANDU Nuclear Reactor III"),
				ModTranslation.Get("nuclear-reactor.Candu_Reactor_T3_build.description",
					"A further-refined thermal CANDU reactor, more powerful than CANDU II, capable of " +
					"using MOX fuel and automatically adjusting its power level (if rated power is " +
					"available). Its more energetic core enriches its entire water circuit with " +
					"dissolved tritium, so all three coolant/steam outlets exit as tritium-rich " +
					"high-pressure heavy water instead of the regular kind, requiring a Tritium " +
					"Separator to recover both substances. At maximum power, this plant can generate " +
					"up to 240 MW of electricity.")),
			layout: registrator.LayoutParser.ParseLayoutOrThrow(layoutParamsT2, layout),
			costs: Costs.Buildings.NuclearReactorT2.MapToEntityCosts(registrator),
			maxPowerLevel: 8,
			fuelCapacity: new Quantity(80),
			minFuelToOperate: new Quantity(32),
			waterInPerStep: heavyWater.WithQuantity(16),
			steamOutPerStep: heavyWaterHighTritium.WithQuantity(16),
			waterInPorts: "AB",
			steamOutPorts: "XY",
			processDuration: 10.Seconds(),
			fuelPairs: ImmutableArray.Create(
				new NuclearReactorProto.FuelData(moxRod, spentMox, 120.Seconds()),
				new NuclearReactorProto.FuelData(uraniumRod, spentFuel, 120.Seconds()),
				new NuclearReactorProto.FuelData(canduRod, spentFuel, 60.Seconds()),
				new NuclearReactorProto.FuelData(dupicRod, dupicSpentFuel, 80.Seconds())),
			fuelInPort: 'F',
			fuelOutPort: 'S',
			coolantIn: heavyWater,
			coolantOut: heavyWaterHighTritium,
			coolantInPort: 'D',
			coolantOutPort: 'W',
			leakRadiationOnMeltdown: true,
			destroyFuelOnMeltdown: false,
			computingConsumed: Computing.FromTFlops(24),
			enrichment: Option.None,
			graphics: new NuclearReactorProto.Gfx(
				prefabPath: "Assets/Base/Buildings/NuclearReactors/NuclearReactorT2.prefab",
				categories: registrator.GetCategoriesProtos(Ids.ToolbarCategories.Power_Nuclear),
				soundPrefabPath: "Assets/Base/Buildings/NuclearReactors/Shared/ReactorSound.prefab",
				fuelIconPath: "Assets/Base/Buildings/NuclearReactors/Shared/MoxReactorFuelIcon.svg",
				maxEmissionIntensity: 3.5f)));

		canduT2.SetNextTier(canduT3);
	}

	// Fusion reactor, reusing vanilla FastBreederReactor's layout/prefab. Same water-loop
	// pattern as the CANDU reactors, but running on Helium instead of Heavy Water.
	// Additionally demonstrates NuclearReactorProto's "enrichment" mechanic: a third,
	// independent input/output pair (ports Q/E here) that gradually converts one product
	// into another over several process steps, separate from the fuel-burning cycle.
	private void RegisterFusionReactor(ProtoRegistrator registrator) {
		var protosDb = registrator.PrototypesDb;

		ProductProto helium = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.Helium);
		ProductProto heliumHot = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.HeliumHot);
		ProductProto dtFuel = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.DtFuel);
		ProductProto plasmaDirty = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.PlasmaDirty);

		string[] layout = new string[] {
			"   [3][3][3][3][3][3][8][8][8][8][8][8][8][8][8][8][8][5][5][5][2]",
			"   [3][3][3][3][3][3][8][8][8][8][8][8][8][8][8][8][8][5][5][5][2]",
			"   [6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][5][5][5][2]",
			"   [6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][5][5][5][2]",
			"   [6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][5][5][5][2]",
			"   [6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][6][6][2]",
			"   [6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][6][6][2]",
			"F@>[6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][6][6][2]",
			"S@<[6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][6][6][2]",
			"   [6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][6][6][2]",
			"   [6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][6][6][2]",
			"Q@>[6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][6][6][2]",
			"E@<[6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][6][6][2]",
			"   [6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][6][6][2]",
			"   [6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][6][6][2]",
			"   [6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][6][6][2]",
			"   [6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][4][4][2]",
			"   [6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][4][4][2]",
			"   [6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][4][4][2]",
			"   [6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][4][4][2]",
			"   [9![9![9![4][4][4][8][8][8][8][8][8][8][8][8][8][8][6][4][4][2]",
			"   [9![9![9![4][4][4][8][8][8][8][8][8][8][8][8][8][8][6][4][4][2]",
			"   [2][3][3][4][4][4][8][8][8][8][8][8][8][8][8][8][8][6][4][4][2]",
			"   [2][3][3][4][4][4][8][8][8][8][8][8][8][8][8][8][8][6][4][4][2]",
			"   [2][3][3][4][4][4][8][8][8][8][8][8][8][8][8][8][8][6][4][4][2]",
			"   [2][3][3][4][4][4][8][8][8][8][8][8][8][8][8][8][8][6][4][4][2]",
			"   [2][3][3][4][4][4][8][8][8][8][8][8][8][8][8][8][8][6][4][4][2]",
			"   [2][3][3][4][4][4][7][7][7][7][7][7][7][7][7][7][7][6][4][4][2]",
			"   [2][3][3][4][4][4][7][7][7][7][7][7][7][7][7][7][7][7][4][4][2]",
			"   [2][3][3][4][4][4][7][7][7][7][9][9][9][9][9][9][7][7][7][4][2]",
			"   [2][3][3][4][4][4][7][7][7][9][9][9][9][9][9][9][9][7][7][7][2]",
			"   [2][3][3][4][4][4][7][7][9][7![7![7![7![7![7![7![7![9][7][7][2]",
			"   [2][3][3][4][4][4][7][7][9][7![7![7![7![7![7![7![7![9][7][7][2]",
			"   [2][3][3][4][4][4][7][7][9][7![7![7![7![7![7![7![7![9][7][7][2]",
			"   [2][3][3][4][4][4][7][7][9][7![7![7![7![7![7![7![7![9][7][7][2]",
			"   [2][3][3][4][4][4][7][7][9][7![7![7![7![7![7![7![7![9][7][7]   ",
			"                  [4][7][7][9][9][9][7![7![7![7![9][9][9][7][7]   ",
			"                     [7][7][7][9][9][9][9][9][9][9][9][7][7][7]   ",
			"                        [7][7][7][9][9][9][9][9][9][7][7][7]      ",
			"                        [7][7][7][7][7][7][7][7][7][7][7][7]      ",
			"                     D@>[4][4][4][7][7][7][7][7][7][4][4][4]W@>   ",
			"                        [4][4][4][4][4][4][4][4][4][4][4][4]      ",
			"                     A@>[4][4][4][4][4][4][4][4][4][4][4][4]X@>   ",
			"                     B@>[4][4][4][4][4][4][4][4][4][4][4][4]Y@>   ",
			"                        [4][4][4][4][4][4][4][4][4][4][4][4]      "
		};

		var layoutParams = new EntityLayoutParams(null, new CustomLayoutToken[] {
			new CustomLayoutToken("[0!", (EntityLayoutParams p, int h) => {
				int heightToExcl = h + 4;
				int? terrainSurfaceHeight = 0;
				Proto.ID? surfaceId = p.HardenedFloorSurfaceId;
				return new LayoutTokenSpec(0, heightToExcl, LayoutTileConstraint.None, terrainSurfaceHeight,
					null, null, null, null, surfaceId);
			})
		});

		registrator.PrototypesDb.Add(new NuclearReactorProto(
			id: ModIDs.Machines.FusionReactorT1,
			strings: Proto.CreateStr(ModIDs.Machines.FusionReactorT1,
				ModTranslation.Get("nuclear-reactor.Fusion_Reactor_t1.name", "Fusion Nuclear Reactor"),
				ModTranslation.Get("nuclear-reactor.Fusion_Reactor_t1.description",
					"The nuclear fusion reactor is an advanced system designed to fuse light nuclei, " +
					"such as deuterium and tritium, generating enormous amounts of clean energy. It " +
					"works by creating extreme conditions of temperature and pressure that allow the " +
					"nuclei to fuse, releasing energy in a continuous and controlled manner. It " +
					"represents the most promising technology for achieving a stable, safe, and " +
					"virtually inexhaustible energy source. It operates at higher temperatures to " +
					"produce very high-pressure steam (800 \u00b0C). If the core overheats and " +
					"emergency cooling is unavailable, the reactor automatically shuts down by " +
					"draining the molten fuel, resulting in the total loss of the fuel and damage to " +
					"the reactor. At maximum power, this plant can supply up to 240 MW of electrical " +
					"energy.")),
			layout: registrator.LayoutParser.ParseLayoutOrThrow(layoutParams, layout),
			costs: Costs.Buildings.NuclearReactorT3.MapToEntityCosts(registrator),
			maxPowerLevel: 4,
			fuelCapacity: new Quantity(160),
			minFuelToOperate: new Quantity(80),
			waterInPerStep: helium.WithQuantity(16),
			steamOutPerStep: heliumHot.WithQuantity(16),
			waterInPorts: "AB",
			steamOutPorts: "XY",
			processDuration: 10.Seconds(),
			fuelPairs: ImmutableArray.Create(
				new NuclearReactorProto.FuelData(dtFuel, helium, 15.Seconds())),
			fuelInPort: 'F',
			fuelOutPort: 'S',
			coolantIn: helium,
			coolantOut: heliumHot,
			coolantInPort: 'D',
			coolantOutPort: 'W',
			leakRadiationOnMeltdown: false,
			destroyFuelOnMeltdown: true,
			computingConsumed: Computing.FromTFlops(18),
			enrichment: Option.Some(new NuclearReactorProto.EnrichmentData(
				inputProduct: dtFuel,
				inPort: 'Q',
				outputProduct: plasmaDirty,
				outPort: 'E',
				processedPerLevel: new PartialQuantity(1),
				buffersCapacity: new Quantity(320),
				destroyContentOnMeltdown: true,
				enrichmentSteps: ImmutableArray.Create(
					new NuclearReactorProto.EnrichmentStepData(50.Percent(), 0, 1),
					new NuclearReactorProto.EnrichmentStepData(Percent.Hundred, 1, 1),
					new NuclearReactorProto.EnrichmentStepData(Percent.Hundred, 3, 4)),
				defaultEnrichmentStep: 1)),
			graphics: new NuclearReactorProto.Gfx(
				prefabPath: "Assets/Base/Buildings/NuclearReactors/FastReactor.prefab",
				categories: registrator.GetCategoriesProtos(Ids.ToolbarCategories.Power_Nuclear),
				soundPrefabPath: "Assets/Base/Buildings/NuclearReactors/Shared/ReactorSound.prefab",
				fuelIconPath: Option<string>.None,
				maxEmissionIntensity: 9f)));
	}
}
