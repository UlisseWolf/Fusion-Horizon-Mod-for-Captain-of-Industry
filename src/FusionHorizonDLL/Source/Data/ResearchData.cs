using Mafi;
using Mafi.Base;
using Mafi.Core.Buildings.Storages.NuclearWaste;
using Mafi.Core.Mods;
using Mafi.Core.Population;
using Mafi.Core.Products;
using Mafi.Core.Prototypes;
using Mafi.Core.Research;

namespace FusionHorizon.Data;

// Registers every custom research node. ResearchNodeProtoBuilder: .Start(name, id,
// costMonths) -> .Description(...) -> .SetGridPosition(x, y) (screen position in the
// research tree UI) -> .AddParents(...) (one or more prerequisite nodes — mix custom and
// vanilla freely) -> .AddProductToUnlock(...)/.AddRecipeToUnlock(...)/
// .AddMachineToUnlock(...)/.AddLayoutEntityToUnlock(...) (the last for reactors, which
// are LayoutEntityProto rather than MachineProto) -> .BuildAndAdd().
//
// Two techniques used below for things the builder chain alone can't do:
//  - ResearchNodeProto.AddParent(...) is a public instance method (distinct from the
//    builder's .AddParents(...)) that can be called on an ALREADY-BUILT node — including
//    a vanilla one — to add an extra prerequisite after the fact. It's only guarded by an
//    "IsInitialized" check, and every mod's protos are finalized together in one pass
//    after all IModData.RegisterData calls finish, so this is safe to call here. It's the
//    same technique the base game's own research-graph wiring uses internally.
//  - Proto.AddParam(...) attaches an optional behavior param (e.g. MedicalSuppliesParam,
//    RadioactiveWasteParam) to an already-registered proto, for systems that scan the
//    proto database for a param rather than requiring a fixed product list.
internal class ResearchData : IModData {

	public void RegisterData(ProtoRegistrator registrator) {
		var protosDb = registrator.PrototypesDb;

		ResearchNodeProto vanillaNuclearReactor = protosDb.GetOrThrow<ResearchNodeProto>(Ids.Research.NuclearReactor);
		ResearchNodeProto vanillaNuclearReactor2 = protosDb.GetOrThrow<ResearchNodeProto>(Ids.Research.NuclearReactor2);
		ResearchNodeProto vanillaFastBreederReactor = protosDb.GetOrThrow<ResearchNodeProto>(Ids.Research.NuclearReactor3);
		ResearchNodeProto vanillaMedicalSupplies3 = protosDb.GetOrThrow<ResearchNodeProto>(Ids.Research.MedicalSupplies3);

		// ------------------------------------------------------------------------
		ResearchNodeProto canduReactor = registrator.ResearchNodeProtoBuilder
			.Start(
				ModTranslation.Get("research.Research_Candu_Reactor.name", "Nuclear Reactor CANDU"),
				ModIDs.Research.CanduReactor, costMonths: 170)
			.Description(ModTranslation.Get("research.Research_Candu_Reactor.description",
				"A thermal reactor that sustains a nuclear chain reaction using natural uranium fuel " +
				"rods. The CANDU nuclear reactor is moderated by heavy water, which requires a heat " +
				"exchanger to produce steam. Warning: CANDU fuel produces spent fuel, which is " +
				"radioactive and can harm the public if not stored in a specialized facility."))
			.SetGridPosition(new Vector2i(132, 31))
			.AddParents(vanillaNuclearReactor)
			.AddProductIcon(ModIDs.Products.CanduRod)
			.AddProductToUnlock(ModIDs.Products.HeavyWater)
			.AddProductToUnlock(ModIDs.Products.HeavyWaterHigh)
			.AddProductToUnlock(ModIDs.Products.CanduRod)
			.AddRecipeToUnlock(ModIDs.Recipes.HeavyWater)
			.AddRecipeToUnlock(ModIDs.Recipes.CanduRod)
			.AddRecipeToUnlock(ModIDs.Recipes.PrimarySteamGenerator)
			.AddMachineToUnlock(ModIDs.Machines.HeatExchangerCandu)
			.AddLayoutEntityToUnlock(ModIDs.Machines.CanduReactorT1)
			.BuildAndAdd();

		// ------------------------------------------------------------------------
		// Sits between vanilla "Nuclear reactor II" and "Fast breeder reactor": its own
		// parent is Nuclear reactor II, and it's ALSO added as an extra prerequisite of
		// the vanilla Fast breeder reactor node below via AddParent(...), so the chain
		// becomes Nuclear reactor II -> DUPIC -> Fast breeder reactor.
		ResearchNodeProto dupicRod = registrator.ResearchNodeProtoBuilder
			.Start(
				ModTranslation.Get("research.Research_Dupic_Rod.name", "DUPIC Fuel Reprocessing"),
				ModIDs.Research.DupicRod, costMonths: 280)
			.Description(ModTranslation.Get("research.Research_Dupic_Rod.description",
				"DUPIC (Direct Use of spent PWR fuel In CANDU) mechanically reprocesses vanilla " +
				"Spent Fuel into new fuel rods without ever chemically separating out plutonium, " +
				"using the existing Nuclear Reprocessing Plant. DUPIC Rods burn out faster than " +
				"dedicated CANDU or MOX rods, but let CANDU II and CANDU III reactors run directly " +
				"on reprocessed Spent Fuel. Burning a DUPIC Rod yields DUPIC Spent Fuel, which is " +
				"too degraded for further OREOX recycling and instead feeds a Fast Breeder " +
				"Reactor's breeding blanket."))
			.SetGridPosition(new Vector2i(150, 31))
			.AddParents(vanillaNuclearReactor2)
			.AddProductIcon(ModIDs.Products.DupicRod)
			.AddProductToUnlock(ModIDs.Products.DupicRod)
			.AddProductToUnlock(ModIDs.Products.DupicSpentFuel)
			.AddRecipeToUnlock(ModIDs.Recipes.DupicRodReprocessing)
			.AddRecipeToUnlock(ModIDs.Recipes.DupicSpentFuelToBlanket)
			.BuildAndAdd();

		vanillaFastBreederReactor.AddParent(dupicRod);

		// ------------------------------------------------------------------------
		ResearchNodeProto fusionReactor = registrator.ResearchNodeProtoBuilder
			.Start(
				ModTranslation.Get("research.Research_Fusion_Reactor.name", "Fusion Nuclear Reactor"),
				ModIDs.Research.FusionReactor, costMonths: 650)
			.Description(ModTranslation.Get("research.Research_Fusion_Reactor.description",
				"Research into nuclear fusion reactors unlocks the production chain for tritium and " +
				"deuterium, which are used as fuel in nuclear fusion reactors. It also unlocks the " +
				"production of helium, which is used as a coolant in nuclear fusion reactors."))
			.SetGridPosition(new Vector2i(164, 50))
			.AddParents(vanillaFastBreederReactor)
			.AddProductIcon(ModIDs.Products.DtFuel)
			.AddProductToUnlock(ModIDs.Products.Deuterium)
			.AddProductToUnlock(ModIDs.Products.Tritium)
			.AddProductToUnlock(ModIDs.Products.Helium)
			.AddProductToUnlock(ModIDs.Products.HeliumHot)
			.AddProductToUnlock(ModIDs.Products.DtFuel)
			.AddRecipeToUnlock(ModIDs.Recipes.Deuterium)
			.AddRecipeToUnlock(ModIDs.Recipes.Tritium)
			.AddRecipeToUnlock(ModIDs.Recipes.DtFuel)
			.AddRecipeToUnlock(ModIDs.Recipes.PrimarySteamGeneratorHelium)
			.AddRecipeToUnlock(ModIDs.Recipes.HeliumDischargedT1)
			.AddRecipeToUnlock(ModIDs.Recipes.HeliumDischargedT2)
			.AddLayoutEntityToUnlock(ModIDs.Machines.FusionReactorT1)
			.BuildAndAdd();

		// ------------------------------------------------------------------------
		ResearchNodeProto canduReactor3 = registrator.ResearchNodeProtoBuilder
			.Start(
				ModTranslation.Get("research.Research_Candu_Reactor_3.name", "Nuclear Reactor CANDU III"),
				ModIDs.Research.CanduReactor3, costMonths: 750)
			.Description(ModTranslation.Get("research.Research_Candu_Reactor_3.description",
				"A further-refined CANDU reactor, more powerful than CANDU II. Its coolant comes " +
				"out enriched with dissolved tritium, so this research also unlocks the Tritium " +
				"Separator, needed to recover both tritium and reusable Super-Pressurized Heavy " +
				"Water from it."))
			.SetGridPosition(new Vector2i(172, 50))
			.AddParents(fusionReactor)
			.AddProductIcon(ModIDs.Products.HeavyWaterHighTritium)
			.AddProductToUnlock(ModIDs.Products.HeavyWaterHighTritium)
			.AddProductToUnlock(ModIDs.Products.HeavyWaterSp)
			.AddRecipeToUnlock(ModIDs.Recipes.TritiumSeparation)
			.AddRecipeToUnlock(ModIDs.Recipes.PrimarySteamGeneratorSp)
			.AddMachineToUnlock(ModIDs.Machines.TritiumSeparator)
			.AddLayoutEntityToUnlock(ModIDs.Machines.CanduReactorT3)
			.BuildAndAdd();

		// ------------------------------------------------------------------------
		// Display name only ("Industrial Plasma") differs from the internal id
		// (Research_Plasma_Inceneritor) — renaming a node's copy never requires
		// touching its id, since the id is invisible to the player and other nodes
		// only ever reference it by the strongly-typed ModIDs constant.
		ResearchNodeProto plasmaIncinerator = registrator.ResearchNodeProtoBuilder
			.Start(
				ModTranslation.Get("research.Research_Plasma_Inceneritor.name", "Industrial Plasma"),
				ModIDs.Research.PlasmaIncinerator, costMonths: 800)
			.Description(ModTranslation.Get("research.Research_Plasma_Inceneritor.description",
				"This research enables the extraction of plasma from the nuclear fusion reactor, and " +
				"through purification cycles, it produces plasma that can be used in non-nuclear " +
				"industrial systems — from Incinerator II, which allows plasma to be used as fuel " +
				"for burning waste, to sealed Plasma Modules for advanced medical equipment."))
			.SetGridPosition(new Vector2i(178, 50))
			.AddParents(fusionReactor)
			.AddProductIcon(ModIDs.Products.Plasma)
			.AddProductToUnlock(ModIDs.Products.PlasmaDirty)
			.AddProductToUnlock(ModIDs.Products.PlasmaRefined)
			.AddProductToUnlock(ModIDs.Products.Plasma)
			.AddRecipeToUnlock(ModIDs.Recipes.RefinedPlasma)
			.AddRecipeToUnlock(ModIDs.Recipes.Plasma)
			.AddMachineToUnlock(ModIDs.Machines.IncinerationPlantT2)
			.BuildAndAdd();

		// ------------------------------------------------------------------------
		// Parented directly to the vanilla Fast breeder reactor node, not to CANDU
		// III/Fusion, since this is a pure-fission technology.
		ResearchNodeProto isotopeSeparation = registrator.ResearchNodeProtoBuilder
			.Start(
				ModTranslation.Get("research.Research_Isotope_Separation.name", "Isotope Separation"),
				ModIDs.Research.IsotopeSeparation, costMonths: 550)
			.Description(ModTranslation.Get("research.Research_Isotope_Separation.description",
				"Separates fission platinum-group metals and sealed Cs-137/Sr-90 sources out of " +
				"Fission Products, yielding stable, reusable Industrial Isotopes. Most of the " +
				"processed material remains Depleted Fission Products, still requiring radioactive " +
				"waste storage, but for somewhat less time than untreated Fission Products."))
			.SetGridPosition(new Vector2i(185, 31))
			.AddParents(vanillaFastBreederReactor)
			.AddProductIcon(ModIDs.Products.IndustrialIsotopes)
			.AddProductToUnlock(ModIDs.Products.IndustrialIsotopes)
			.AddProductToUnlock(ModIDs.Products.DepletedFissionProducts)
			.AddRecipeToUnlock(ModIDs.Recipes.IsotopeSeparation)
			.BuildAndAdd();

		// Attaches RadioactiveWasteParam after registration: the vanilla Radioactive
		// Waste Storage decays a product carrying this param into another product
		// (here, into vanilla RetiredWaste) over the given number of years.
		ProductProto depletedFissionProductsProto = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.DepletedFissionProducts);
		ProductProto retiredWaste = protosDb.GetOrThrow<ProductProto>(Ids.Products.RetiredWaste);
		depletedFissionProductsProto.AddParam(new RadioactiveWasteParam(50, retiredWaste.Id));

		// ------------------------------------------------------------------------
		// Three parents, none redundant with another: Isotope Separation, vanilla
		// "Medical supplies III", and Industrial Plasma.
		ResearchNodeProto medicalSupplies4 = registrator.ResearchNodeProtoBuilder
			.Start(
				ModTranslation.Get("research.Research_Medical_Supplies_4.name", "Medical Supplies IV"),
				ModIDs.Research.MedicalSupplies4, costMonths: 900)
			.Description(ModTranslation.Get("research.Research_Medical_Supplies_4.description",
				"Combines Industrial Isotopes for nuclear medicine with Plasma Modules for advanced " +
				"plasma-based medical equipment, unlocking Medical Supplies IV — the most advanced " +
				"tier of healthcare, provided directly to Clinics."))
			.SetGridPosition(new Vector2i(195, 40))
			.AddParents(isotopeSeparation, vanillaMedicalSupplies3, plasmaIncinerator)
			.AddProductIcon(ModIDs.Products.MedicalSupplies4)
			.AddProductToUnlock(ModIDs.Products.MedicalSupplies4)
			.AddProductToUnlock(ModIDs.Products.PlasmaModule)
			.AddRecipeToUnlock(ModIDs.Recipes.PlasmaModuleAssembly)
			.AddRecipeToUnlock(ModIDs.Recipes.MedicalSupplies4Assembly)
			.BuildAndAdd();

		// MedicalSuppliesParam is what makes the vanilla Clinic accept a product as
		// healthcare input at all: HospitalProto scans every registered ProductProto for
		// this param at init, so no changes to the Clinic itself are needed. Values here
		// extend vanilla's own linear tier I->III progression by one more step.
		ProductProto medicalSupplies4Proto = protosDb.GetOrThrow<ProductProto>(ModIDs.Products.MedicalSupplies4);
		medicalSupplies4Proto.AddParam(new MedicalSuppliesParam(1.25.Upoints(), 30.Percent(), 1.9.Percent()));

		// TODO: hook Recipe_Bioethanol / Recipe_Organic_pesticide onto the EXISTING
		// vanilla research node "ResearchSugarCane" instead of leaving them unlocked
		// from the start. ResearchNodeProtoBuilderExtensions' Add*ToUnlock methods are
		// extension methods on ResearchNodeProtoBuilder.State, usable only inside a
		// .Start(...)...BuildAndAdd() chain for a NEW node — there's no equivalent for
		// appending unlocks to an already-registered node. Options: reflection on the
		// private backing field, or simply leave these two unlocked from the start.
	}
}
