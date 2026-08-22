using Mafi;
using Mafi.Core.Factory.Machines;
using Mafi.Core.Mods;
using Mafi.Core.Products;
using Mafi.Core.Prototypes;

namespace FusionHorizon.Data;

// Registers every custom recipe. RecipeProtoBuilder is a two-step API:
//   1. registrator.RecipeProtoBuilder.Start(recipeId) -> .AddInput(ProductProto,
//      Quantity, port?) / .AddOutput(...) (resolved ProductProto objects, not IDs) ->
//      .BuildAndAdd() returns a RecipeBindingBuilder, not the recipe itself.
//   2. .BindTo(MachineProto machine, Duration duration, multiplier?) on that builder
//      actually attaches the recipe (with its own duration) to a machine. The same
//      recipe can be bound to several machine tiers with different durations/
//      multipliers in one chain (see MedicalSupplies4Assembly below).
// The `port` argument on AddInput/AddOutput is optional: pass a specific letter to pin
// an ingredient to one physical port, or omit it to let any port of a matching type
// accept it (this is what vanilla's own multi-input assembly recipes do, giving the
// player free choice of which belt feeds which port).
//
// Port types are declared per machine layout (see MachineData.cs / vanilla source), and
// the ASCII symbol used in SetLayout is NOT a reliable indicator of type on its own —
// two different machines can use the same symbol for different port types. When binding
// a recipe to an existing machine, check what type each of its ports actually expects
// (Fluid, Countable/unit-item, Loose material, Molten) before assigning ports, or let
// the engine auto-resolve by leaving the port argument out.
internal class RecipesData : IModData {

	public void RegisterData(ProtoRegistrator registrator) {
		var protosDb = registrator.PrototypesDb;

		// Small local helpers to resolve a vanilla product/machine proto from its
		// literal string id, since this mod only has strongly-typed IDs for its own
		// custom protos (see ModIDs.*.cs).
		ProductProto P(string id) => protosDb.GetOrThrow<ProductProto>(new ProductProto.ID(id));
		MachineProto M(string id) => protosDb.GetOrThrow<MachineProto>(new MachineProto.ID(id));

		ProductProto canduRod = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.CanduRod);
		ProductProto heavyWater = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.HeavyWater);
		ProductProto heavyWaterHigh = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.HeavyWaterHigh);
		ProductProto heavyWaterHighTritium = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.HeavyWaterHighTritium);
		ProductProto heavyWaterSp = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.HeavyWaterSp);
		ProductProto dupicRod = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.DupicRod);
		ProductProto dupicSpentFuel = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.DupicSpentFuel);
		ProductProto industrialIsotopes = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.IndustrialIsotopes);
		ProductProto depletedFissionProducts = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.DepletedFissionProducts);
		ProductProto plasmaModule = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.PlasmaModule);
		ProductProto medicalSupplies4 = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.MedicalSupplies4);
		ProductProto deuterium = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.Deuterium);
		ProductProto helium = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.Helium);
		ProductProto heliumHot = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.HeliumHot);
		ProductProto tritium = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.Tritium);
		ProductProto dtFuel = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.DtFuel);
		ProductProto plasmaDirty = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.PlasmaDirty);
		ProductProto plasmaRefined = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.PlasmaRefined);
		ProductProto plasma = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.Plasma);

		MachineProto heatExchangerCandu = protosDb.GetOrThrow<MachineProto>(ModIDs.Machines.HeatExchangerCandu);
		MachineProto incinerationPlantT2 = protosDb.GetOrThrow<MachineProto>(ModIDs.Machines.IncinerationPlantT2);
		MachineProto tritiumSeparator = protosDb.GetOrThrow<MachineProto>(ModIDs.Machines.TritiumSeparator);

		// ------------------------------------------------------------------
		// CANDU fuel/coolant chain
		// ------------------------------------------------------------------

		registrator.RecipeProtoBuilder
			.Start(ModIDs.Recipes.CanduRod)
			.Description(ModTranslation.Get("recipe.Recipe_Candu_Rod.description",
				"An industrial process that compresses yellowcake into a ceramic material and " +
				"inserts it into a steel cladding, forming a ready-to-use CANDU fuel rod."))
			.AddInput(P("Product_Yellowcake"), new Quantity(2), "E")
			.AddInput(P("Product_Steel"), new Quantity(1), "D")
			.AddOutput(canduRod, new Quantity(2), "Y")
			.BuildAndAdd()
			.BindTo(M("ChemicalPlant2"), 15.Seconds());

		registrator.RecipeProtoBuilder
			.Start(ModIDs.Recipes.HeavyWater)
			.Description(ModTranslation.Get("recipe.Recipe_heavy_water.description",
				"An isotopic exchange process in which water, high-pressure steam, and acid react " +
				"to concentrate deuterium. The separation yields heavy water as the enriched " +
				"fraction and produces sour water as a byproduct to be treated or recycled."))
			.AddInput(P("Product_Water"), new Quantity(120), "A")
			.AddInput(P("Product_SteamHi"), new Quantity(40), "B")
			.AddInput(P("Product_Acid"), new Quantity(15), "C")
			.AddOutput(heavyWater, new Quantity(100), "X")
			.AddOutput(P("Product_SourWater"), new Quantity(75), "Z")
			.BuildAndAdd()
			.BindTo(M("ChemicalPlant2"), 30.Seconds());

		registrator.RecipeProtoBuilder
			.Start(ModIDs.Recipes.PrimarySteamGenerator)
			.Description(ModTranslation.Get("recipe.Recipe_primary_steam_generator.description",
				"Primary Steam Generation is the process in which heat from the pressurized " +
				"heavy\u2011water primary loop is transferred to ordinary water, converting it into " +
				"high\u2011pressure steam while the heavy water exits cooled but unchanged."))
			.AddInput(heavyWaterHigh, new Quantity(192), "A")
			.AddInput(P("Product_Water"), new Quantity(192), "B")
			.AddOutput(heavyWater, new Quantity(192), "W")
			.AddOutput(P("Product_SteamHi"), new Quantity(192), "X")
			.BuildAndAdd()
			.BindTo(heatExchangerCandu, 30.Seconds());

		// Alternate recipe on the same machine: makes super-pressurized steam instead of
		// regular high-pressure steam, gated behind Super-Pressurized Heavy Water (only
		// obtainable from the Tritium Separator, see below) so it can't be produced
		// straight from CANDU I/II's cheap, early-game Heavy Water (High).
		registrator.RecipeProtoBuilder
			.Start(ModIDs.Recipes.PrimarySteamGeneratorSp)
			.Description(ModTranslation.Get("recipe.Recipe_primary_steam_generator_sp.description",
				"Primary Steam Generation is the process in which heat from the super\u2011" +
				"pressurized heavy\u2011water primary loop is transferred to ordinary water, " +
				"converting it into super\u2011pressurized steam while the heavy water exits " +
				"cooled down to its regular high\u2011pressure form."))
			.AddInput(heavyWaterSp, new Quantity(192), "A")
			.AddInput(P("Product_Water"), new Quantity(192), "B")
			.AddOutput(heavyWaterHigh, new Quantity(192), "W")
			.AddOutput(P("Product_SteamSp"), new Quantity(192), "X")
			.BuildAndAdd()
			.BindTo(heatExchangerCandu, 30.Seconds());

		// Splits CANDU 3's tritium-rich coolant into pure Tritium and Super-Pressurized
		// Heavy Water.
		registrator.RecipeProtoBuilder
			.Start(ModIDs.Recipes.TritiumSeparation)
			.Description(ModTranslation.Get("recipe.Recipe_tritium_separation.description",
				"An isotopic separation process that draws off the dissolved tritium from the " +
				"tritium-rich, high-pressure heavy water produced by a CANDU 3 reactor's coolant " +
				"loop, yielding pure tritium and recovering Super-Pressurized Heavy Water."))
			.AddInput(heavyWaterHighTritium, new Quantity(60), "A")
			.AddOutput(tritium, new Quantity(12), "X")
			.AddOutput(heavyWaterSp, new Quantity(60), "Y")
			.BuildAndAdd()
			.BindTo(tritiumSeparator, 15.Seconds());

		// ------------------------------------------------------------------
		// DUPIC fuel reprocessing (bound to the vanilla NuclearReprocessingPlant,
		// alongside its own SpentFuelReprocessing/SpentFuelToBlanket/etc. recipes)
		// ------------------------------------------------------------------

		// OREOX process: mechanically reprocesses vanilla Spent Fuel into new fuel rods
		// without ever chemically separating plutonium out — unlike vanilla's own
		// SpentFuelReprocessing, this recipe has no Plutonium output.
		registrator.RecipeProtoBuilder
			.Start(ModIDs.Recipes.DupicRodReprocessing)
			.Description(ModTranslation.Get("recipe.Recipe_dupic_rod_reprocessing.description",
				"The OREOX process (repeated oxidation/reduction cycles) mechanically pulverizes " +
				"and re-sinters spent light-water-reactor fuel into new DUPIC fuel rods, without " +
				"ever chemically separating out plutonium."))
			.AddInput(P("Product_SpentFuel"), new Quantity(8), "E")
			.AddInput(P("Product_Acid"), new Quantity(2), "C")
			.AddOutput(dupicRod, new Quantity(6), "X")
			.AddOutput(P("Product_FissionProduct"), new Quantity(2), "Z")
			.BuildAndAdd()
			.BindTo(M("NuclearReprocessingPlant"), 60.Seconds());

		// The only productive use for DUPIC Spent Fuel: mirrors vanilla's own
		// SpentFuelToBlanket/SpentMoxToBlanket, routing it into the Fast Breeder
		// Reactor's breeding-blanket feedstock chain instead of back into more DUPIC
		// Rods. Note BlanketFuel is a Fluid product despite the name, so its output port
		// must be the machine's Fluid output ("J"), not one of the Countable ones.
		registrator.RecipeProtoBuilder
			.Start(ModIDs.Recipes.DupicSpentFuelToBlanket)
			.Description(ModTranslation.Get("recipe.Recipe_dupic_spent_fuel_to_blanket.description",
				"Vitrifies DUPIC Spent Fuel — too isotopically degraded for further OREOX recycling " +
				"— into Blanket Fuel for a Fast Breeder Reactor's breeding blanket, exactly like " +
				"regular Spent Fuel or Spent MOX."))
			.AddInput(dupicSpentFuel, new Quantity(1), "E")
			.AddInput(P("Product_Acid"), new Quantity(1), "C")
			.AddInput(P("Product_MoltenGlass"), new Quantity(1), "A")
			.AddInput(P("Product_Salt"), new Quantity(1), "B")
			.AddOutput(P("Product_BlanketFuel"), new Quantity(1), "J")
			.AddOutput(P("Product_FissionProduct"), new Quantity(1), "Z")
			.BuildAndAdd()
			.BindTo(M("NuclearReprocessingPlant"), 30.Seconds());

		// ------------------------------------------------------------------
		// Isotope separation / nuclear medicine chain
		// ------------------------------------------------------------------

		// A fifth recipe on the same NuclearReprocessingPlant. The 10% extraction ratio
		// reflects that only a small mass fraction of real fission products is separable
		// this way — scale up with more reprocessing plants rather than raising this
		// ratio. Outputs Depleted Fission Products (not vanilla Fission Product again),
		// so the separated-out mass can't be fed back into this same recipe.
		registrator.RecipeProtoBuilder
			.Start(ModIDs.Recipes.IsotopeSeparation)
			.Description(ModTranslation.Get("recipe.Recipe_isotope_separation.description",
				"Separates fission platinum-group metals and sealed Cs-137/Sr-90 sources out of " +
				"Fission Products, yielding a small quantity of stable, reusable Industrial " +
				"Isotopes. Most of the input mass remains Depleted Fission Products, still " +
				"requiring radioactive waste storage."))
			.AddInput(P("Product_FissionProduct"), new Quantity(20), "E")
			.AddOutput(industrialIsotopes, new Quantity(2), "X")
			.AddOutput(depletedFissionProducts, new Quantity(18), "Y")
			.BuildAndAdd()
			.BindTo(M("NuclearReprocessingPlant"), 60.Seconds());

		// Bridges a Fluid ingredient (Plasma) into recipes that only accept Countable
		// inputs: bound to vanilla ChemicalPlant2, which supports 1 Fluid + 1 Countable
		// input simultaneously, converting Plasma into the Countable Plasma Module so the
		// Assembly recipe below only ever needs Countable ports.
		registrator.RecipeProtoBuilder
			.Start(ModIDs.Recipes.PlasmaModuleAssembly)
			.Description(ModTranslation.Get("recipe.Recipe_plasma_module_assembly.description",
				"Encases refined Plasma inside a sealed glass cartridge, producing a stable, " +
				"transportable Plasma Module ready to power advanced medical equipment."))
			.AddInput(plasma, new Quantity(4), "A")
			.AddInput(P("Product_Glass"), new Quantity(2), "D")
			.AddOutput(plasmaModule, new Quantity(4), "Y")
			.BuildAndAdd()
			.BindTo(M("ChemicalPlant2"), 20.Seconds());

		// Bound to the vanilla Assembly Plant family, same three highest tiers vanilla's
		// own MedicalSupplies3Assembly uses. Ports are deliberately left unpinned on all
		// three ingredients (matching vanilla's own multi-input assembly recipes), so any
		// of the 3 Countable input ports can accept any of the 3 ingredients.
		registrator.RecipeProtoBuilder
			.Start(ModIDs.Recipes.MedicalSupplies4Assembly)
			.Description(ModTranslation.Get("recipe.Recipe_medical_supplies_4_assembly.description",
				"Combines Industrial Isotopes for nuclear medicine with Plasma Modules for " +
				"advanced plasma-based equipment, upgrading Medical Supplies III into the most " +
				"advanced tier of medical care."))
			.AddInput(P("Product_MedicalSupplies3"), new Quantity(8))
			.AddInput(plasmaModule, new Quantity(2))
			.AddInput(industrialIsotopes, new Quantity(2))
			.AddOutput(medicalSupplies4, new Quantity(8))
			.BuildAndAdd()
			.BindTo(M("AssemblyElectrifiedT2"), 20.Seconds())
			.BindTo(M("AssemblyRoboticT1"), 10.Seconds())
			.BindTo(M("AssemblyRoboticT2"), 10.Seconds(), 2);

		// ------------------------------------------------------------------
		// Fusion chain
		// ------------------------------------------------------------------

		registrator.RecipeProtoBuilder
			.Start(ModIDs.Recipes.Deuterium)
			.AddInput(heavyWater, new Quantity(20), "A")
			.AddOutput(deuterium, new Quantity(15), "X")
			.AddOutput(P("Product_Oxygen"), new Quantity(5), "Y")
			.BuildAndAdd()
			.BindTo(M("ElectrolyzerT2"), 15.Seconds());

		registrator.RecipeProtoBuilder
			.Start(ModIDs.Recipes.Tritium)
			.AddInput(P("Product_RefinedLithium"), new Quantity(20))
			.AddInput(P("Product_BlanketFuelEnriched"), new Quantity(15))
			.AddOutput(tritium, new Quantity(19), "X")
			.AddOutput(helium, new Quantity(15), "J")
			.AddOutput(P("Product_FissionProduct"), new Quantity(1))
			.BuildAndAdd()
			.BindTo(M("UraniumEnrichmentPlant"), 15.Seconds());

		registrator.RecipeProtoBuilder
			.Start(ModIDs.Recipes.DtFuel)
			.Description(ModTranslation.Get("recipe.Recipe_dtfuel.description",
				"Fusion Core Fuel is produced through the controlled combination of deuterium and " +
				"tritium, which are purified and compressed to create a high-energy isotopic " +
				"mixture. This compound ensures stability during storage and maximum efficiency in " +
				"initiating fusion, making it the operational standard for advanced next-generation " +
				"reactors."))
			.AddInput(deuterium, new Quantity(15), "A")
			.AddInput(tritium, new Quantity(15), "B")
			.AddOutput(dtFuel, new Quantity(30), "X")
			.BuildAndAdd()
			.BindTo(M("ChemicalPlant2"), 15.Seconds());

		registrator.RecipeProtoBuilder
			.Start(ModIDs.Recipes.PrimarySteamGeneratorHelium)
			.Description(ModTranslation.Get("recipe.Recipe_primary_steam_generator_helium.description",
				"Primary Steam Generation is the process in which heat from the pressurized helium " +
				"primary loop is transferred to ordinary water, converting it into high\u2011pressure " +
				"steam while the heavy water exits cooled but unchanged."))
			.AddInput(heliumHot, new Quantity(192), "A")
			.AddInput(P("Product_Water"), new Quantity(192), "B")
			.AddOutput(helium, new Quantity(192), "W")
			.AddOutput(P("Product_SteamSp"), new Quantity(192), "X")
			.BuildAndAdd()
			.BindTo(heatExchangerCandu, 30.Seconds());

		registrator.RecipeProtoBuilder
			.Start(ModIDs.Recipes.RefinedPlasma)
			.Description(ModTranslation.Get("recipe.Recipe_refinedplasma.description",
				"A high-pressure filtration process that treats contaminated plasma by selectively " +
				"separating deuterium and tritium, producing a recoverable and reusable mixture. " +
				"The residual plasma is partially purified and ready for further refinement."))
			.AddInput(plasmaDirty, new Quantity(15))
			.AddOutput(plasmaRefined, new Quantity(12), "X")
			.AddOutput(dtFuel, new Quantity(3), "J")
			.BuildAndAdd()
			.BindTo(M("UraniumEnrichmentPlant"), 15.Seconds());

		registrator.RecipeProtoBuilder
			.Start(ModIDs.Recipes.Plasma)
			.Description(ModTranslation.Get("recipe.Recipe_plasma.description",
				"An advanced purification process that treats refined plasma by selectively " +
				"removing residual helium and the remaining impurities. The resulting plasma " +
				"achieves a stable and controlled composition, making it fully suitable for use in " +
				"non-nuclear industrial processes."))
			.AddInput(plasmaRefined, new Quantity(12))
			.AddOutput(plasma, new Quantity(10), "X")
			.AddOutput(helium, new Quantity(2), "J")
			.BuildAndAdd()
			.BindTo(M("UraniumEnrichmentPlant"), 15.Seconds());

		// ------------------------------------------------------------------
		// Plasma incineration
		// ------------------------------------------------------------------

		registrator.RecipeProtoBuilder
			.Start(ModIDs.Recipes.PlasmaTrash)
			.Description(ModTranslation.Get("recipe.Recipe_plasma_trash.description",
				"It uses plasma to incinerate waste, converting inorganic waste into slag and " +
				"organic waste into combustible gas. Water is used to cool the system, producing " +
				"super-pressurized steam."))
			.AddInput(plasma, new Quantity(2), "C")
			.AddInput(P("Product_Waste"), new Quantity(48))
			.AddInput(P("Product_Water"), new Quantity(6), "D")
			.AddOutput(P("Product_Slag"), new Quantity(12), "Z")
			.AddOutput(P("Product_SteamSp"), new Quantity(6), "X")
			.AddOutput(P("Product_FuelGas"), new Quantity(12), "Y")
			.BuildAndAdd()
			.BindTo(incinerationPlantT2, 20.Seconds());

		registrator.RecipeProtoBuilder
			.Start(ModIDs.Recipes.PlasmaTrashCompact)
			.Description(ModTranslation.Get("recipe.Recipe_plasma_trash_compact.description",
				"It uses plasma to incinerate waste pressed, converting inorganic waste into slag " +
				"and organic waste into combustible gas. Water is used to cool the system, " +
				"producing super-pressurized steam."))
			.AddInput(plasma, new Quantity(2), "C")
			.AddInput(P("Product_WastePressed"), new Quantity(16))
			.AddInput(P("Product_Water"), new Quantity(6), "D")
			.AddOutput(P("Product_Slag"), new Quantity(12), "Z")
			.AddOutput(P("Product_SteamSp"), new Quantity(6), "X")
			.AddOutput(P("Product_FuelGas"), new Quantity(12), "Y")
			.BuildAndAdd()
			.BindTo(incinerationPlantT2, 20.Seconds());

		// "Vent to atmosphere" recipes for excess helium: no outputs at all, just an
		// input, enabled via EnableEmptyRecipe(). Bound to the vanilla smoke stacks.
		registrator.RecipeProtoBuilder
			.Start(ModIDs.Recipes.HeliumDischargedT2)
			.EnableEmptyRecipe()
			.AddInput(helium, new Quantity(30))
			.BuildAndAdd()
			.BindTo(M("SmokeStackLarge"), 30.Seconds());

		registrator.RecipeProtoBuilder
			.Start(ModIDs.Recipes.HeliumDischargedT1)
			.EnableEmptyRecipe()
			.AddInput(helium, new Quantity(30))
			.BuildAndAdd()
			.BindTo(M("SmokeStack"), 30.Seconds());

		// ------------------------------------------------------------------
		// Misc recipes (hook into the vanilla "ResearchSugarCane" node — see the TODO
		// in ResearchData.cs)
		// ------------------------------------------------------------------

		registrator.RecipeProtoBuilder
			.Start(ModIDs.Recipes.Bioethanol)
			.Description(ModTranslation.Get("recipe.Recipe_Bioethanol.description",
				"Bioethanol is an ethyl alcohol derived from plants, produced through the " +
				"fermentation of biomass such as corn, sugarcane, grains, or agricultural residues. " +
				"It is considered a renewable biofuel and is used both as a gasoline additive and " +
				"in household stoves and fireplaces, thanks to its clean combustion, which produces " +
				"mainly water vapor and carbon dioxide."))
			.AddInput(P("Product_Sugar"), new Quantity(8), "C")
			.AddInput(P("Product_CarbonDioxide"), new Quantity(8), "A")
			.AddInput(P("Product_Water"), new Quantity(8), "B")
			.AddOutput(P("Product_Ethanol"), new Quantity(12), "Y")
			.AddOutput(P("Product_WasteWater"), new Quantity(12), "Z")
			.BuildAndAdd()
			.BindTo(M("FermentationTank"), 30.Seconds());

		registrator.RecipeProtoBuilder
			.Start(ModIDs.Recipes.OrganicPesticide)
			.Description(ModTranslation.Get("recipe.Recipe_Organic_pesticide.description",
				"Organic pesticides are pest control products derived from natural ingredients of " +
				"botanical, microbial, or mineral origin. Although they are chemical compounds, " +
				"they are derived from natural sources and tend to break down more quickly in the " +
				"environment than synthetic pesticides, making them generally less persistent and " +
				"less harmful to ecosystems and human health."))
			.AddInput(P("Product_Ethanol"), new Quantity(4))
			.AddInput(P("Product_Biomass"), new Quantity(4))
			.AddOutput(P("Product_Pesticide"), new Quantity(8))
			.BuildAndAdd()
			.BindTo(M("BioProcessor"), 30.Seconds());
	}
}
