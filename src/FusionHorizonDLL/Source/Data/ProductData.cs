using Mafi;
using Mafi.Core.Mods;
using Mafi.Core.Products;
using Mafi.Core.Prototypes;

namespace FusionHorizon.Data;

// IModData registers a group of related protos; IMod.RegisterPrototypes calls each
// IModData class in turn (see FusionHorizonMod.cs). This class registers every custom
// product: FluidProductProto for pipe/tank fluids, CountableProductProto for belt/crate
// unit items. Both are added via registrator.PrototypesDb.Add(new ...Proto(...)).
//
// Display strings go through Proto.CreateStr(id, name, description); each name/
// description is wrapped in ModTranslation.Get("<category>.<id>.<field>", englishText) so
// translated copy replaces it automatically once a matching key exists in Translations/.
//
// Gfx has two distinct constructors for CountableProductProto: (a) prefabPath +
// customIconPath, for a product with its own dedicated Unity prefab, or (b) meshFamily +
// textures (via ProductTextures), for a product that reuses one of the vanilla shared
// meshes (Barrel, Puck, Box, ...) with a custom texture. Every product below now uses (a).
internal class ProductData : IModData {

	public void RegisterData(ProtoRegistrator registrator) {

		// ----------------------------------------------------------------------------
		// Fluids
		// ----------------------------------------------------------------------------

		registrator.PrototypesDb.Add(new FluidProductProto(
			id: ModIDs.Products.HeavyWater,
			strings: Proto.CreateStr(ModIDs.Products.HeavyWater,
				ModTranslation.Get("product_fluid.Product_heavy_water.name", "Heavy Water"),
				ModTranslation.Get("product_fluid.Product_heavy_water.description",
					"Heavy Water is a deuterium\u2011enriched form of water used as a neutron moderator in " +
					"advanced nuclear reactors, enabling stable and efficient fission processes.")),
			isStorable: true,
			canBeDiscarded: true,
			graphics: new FluidProductProto.Gfx(
				prefabPath: Option<string>.None,
				customIconPath: "Assets/FusionHorizon/ProductIcons/HeavyWater.png",
				color: new ColorRgba(111, 175, 203, 255),
				transportColor: new ColorRgba(28, 84, 104, 255),
				transportAccentColor: new ColorRgba(143, 232, 255, 255)),
			isWaste: false));

		registrator.PrototypesDb.Add(new FluidProductProto(
			id: ModIDs.Products.HeavyWaterHigh,
			strings: Proto.CreateStr(ModIDs.Products.HeavyWaterHigh,
				ModTranslation.Get("product_fluid.Product_heavy_water_high.name", "Heavy Water (High)"),
				ModTranslation.Get("product_fluid.Product_heavy_water_high.description",
					"Heavy Water (High) is a form of deuterium-water maintained at high pressures in the " +
					"primary circuit of a CANDU reactor, where it serves as both a moderator and a coolant, " +
					"exiting the reactor as a dense, hot, and energized fluid after absorbing heat from the core.")),
			isStorable: false,
			canBeDiscarded: false,
			graphics: new FluidProductProto.Gfx(
				prefabPath: Option<string>.None,
				customIconPath: "Assets/FusionHorizon/ProductIcons/HeavyWaterHigh.png",
				color: new ColorRgba(120, 191, 208, 255),
				transportColor: new ColorRgba(28, 84, 104, 255),
				transportAccentColor: new ColorRgba(245, 177, 134, 255)),
			isWaste: false));

		// CANDU 3's coolant loop output: tritium-rich heavy water, processed by the
		// Tritium Separator machine into pure Tritium + regular Heavy Water (High).
		registrator.PrototypesDb.Add(new FluidProductProto(
			id: ModIDs.Products.HeavyWaterHighTritium,
			strings: Proto.CreateStr(ModIDs.Products.HeavyWaterHighTritium,
				ModTranslation.Get("product_fluid.Product_heavy_water_high_tritium.name",
					"Heavy Water (High, Tritium-rich)"),
				ModTranslation.Get("product_fluid.Product_heavy_water_high_tritium.description",
					"A high-pressure form of heavy water drawn from the coolant loop of a CANDU 3 " +
					"reactor. Prolonged neutron exposure has enriched it with dissolved tritium, so " +
					"it must be processed by a Tritium Separator to recover the tritium and the " +
					"regular high-pressure Heavy Water before either can be reused.")),
			isStorable: false,
			canBeDiscarded: false,
			graphics: new FluidProductProto.Gfx(
				prefabPath: Option<string>.None,
				customIconPath: "Assets/FusionHorizon/ProductIcons/HeavyWaterHighTritium.png",
				color: new ColorRgba(125, 140, 199, 255),
				transportColor: new ColorRgba(70, 60, 120, 255),
				transportAccentColor: new ColorRgba(196, 157, 212, 255)),
			isWaste: false));

		// Exclusive Tritium Separator byproduct; gates the super-pressurized-steam recipe
		// so it can't be produced cheaply from ordinary Heavy Water (High).
		registrator.PrototypesDb.Add(new FluidProductProto(
			id: ModIDs.Products.HeavyWaterSp,
			strings: Proto.CreateStr(ModIDs.Products.HeavyWaterSp,
				ModTranslation.Get("product_fluid.Product_heavy_water_sp.name",
					"Heavy Water (Super-Pressurized)"),
				ModTranslation.Get("product_fluid.Product_heavy_water_sp.description",
					"An even denser, super-pressurized form of heavy water, recovered by a Tritium " +
					"Separator alongside pure Tritium when splitting the tritium-rich coolant drawn " +
					"from a CANDU 3 reactor. Its extra pressure lets a heat exchanger raise ordinary " +
					"water all the way to super-pressurized steam.")),
			isStorable: false,
			canBeDiscarded: false,
			graphics: new FluidProductProto.Gfx(
				prefabPath: Option<string>.None,
				customIconPath: "Assets/FusionHorizon/ProductIcons/HeavyWaterHighSuper.png",
				color: new ColorRgba(64, 224, 196, 255),
				transportColor: new ColorRgba(12, 92, 82, 255),
				transportAccentColor: new ColorRgba(255, 199, 89, 255)),
			isWaste: false));

		registrator.PrototypesDb.Add(new FluidProductProto(
			id: ModIDs.Products.Deuterium,
			strings: Proto.CreateStr(ModIDs.Products.Deuterium,
				ModTranslation.Get("product_fluid.Product_deuterium.name", "Deuterium"),
				ModTranslation.Get("product_fluid.Product_deuterium.description",
					"Deuterium is the heavy isotope of hydrogen, which exists in gaseous form as D\u2082 " +
					"molecules. It is a light, stable, and non-radioactive gas used in nuclear and " +
					"scientific applications due to its unique isotopic properties.")),
			isStorable: true,
			canBeDiscarded: false,
			graphics: new FluidProductProto.Gfx(
				prefabPath: Option<string>.None,
				customIconPath: "Assets/FusionHorizon/ProductIcons/Deuterium.png",
				color: new ColorRgba(67, 211, 219, 255),
				transportColor: new ColorRgba(40, 111, 216, 255),
				transportAccentColor: new ColorRgba(195, 207, 247, 255)),
			isWaste: false));

		registrator.PrototypesDb.Add(new FluidProductProto(
			id: ModIDs.Products.Helium,
			strings: Proto.CreateStr(ModIDs.Products.Helium,
				ModTranslation.Get("product_fluid.Product_helium.name", "Helium"),
				ModTranslation.Get("product_fluid.Product_helium.description",
					"Helium is an extremely light, inert, and non-reactive noble gas. It is stable, " +
					"non-flammable, and is used in cryogenic, scientific, and industrial applications " +
					"due to its low density and complete lack of chemical reactivity.")),
			isStorable: true,
			canBeDiscarded: true,
			graphics: new FluidProductProto.Gfx(
				prefabPath: Option<string>.None,
				customIconPath: "Assets/FusionHorizon/ProductIcons/Helium.png",
				color: new ColorRgba(138, 156, 160, 255),
				transportColor: new ColorRgba(71, 149, 154, 255),
				transportAccentColor: new ColorRgba(5, 66, 54, 255)),
			isWaste: false));

		registrator.PrototypesDb.Add(new FluidProductProto(
			id: ModIDs.Products.HeliumHot,
			strings: Proto.CreateStr(ModIDs.Products.HeliumHot,
				ModTranslation.Get("product_fluid.Product_helium_hot.name", "Helium (High)"),
				ModTranslation.Get("product_fluid.Product_helium_hot.description",
					"Helium (High) at High Pressure is a compressed noble gas brought to high temperatures " +
					"while retaining its complete chemical inertness. Under these conditions, it becomes an " +
					"extremely stable thermal fluid, ideal for advanced cooling systems, heat transfer, and " +
					"industrial processes that require a light, clean gas resistant to extreme temperatures.")),
			isStorable: false,
			canBeDiscarded: false,
			graphics: new FluidProductProto.Gfx(
				prefabPath: Option<string>.None,
				customIconPath: "Assets/FusionHorizon/ProductIcons/HeliumHigh.png",
				color: new ColorRgba(138, 156, 160, 255),
				transportColor: new ColorRgba(71, 149, 154, 255),
				transportAccentColor: new ColorRgba(245, 177, 134, 255)),
			isWaste: false));

		registrator.PrototypesDb.Add(new FluidProductProto(
			id: ModIDs.Products.Tritium,
			strings: Proto.CreateStr(ModIDs.Products.Tritium,
				ModTranslation.Get("product_fluid.Product_tritium.name", "Tritium"),
				ModTranslation.Get("product_fluid.Product_tritium.description",
					"Tritium is a radioactive isotope of hydrogen, consisting of one proton and two " +
					"neutrons. It is a light, unstable gas that emits weak beta radiation and is used in " +
					"nuclear and scientific applications, as well as in the production of self-sustaining " +
					"light sources.")),
			isStorable: true,
			canBeDiscarded: false,
			graphics: new FluidProductProto.Gfx(
				prefabPath: Option<string>.None,
				customIconPath: "Assets/FusionHorizon/ProductIcons/Tritium.png",
				color: new ColorRgba(130, 77, 194, 255),
				transportColor: new ColorRgba(176, 39, 166, 255),
				transportAccentColor: new ColorRgba(196, 157, 212, 255)),
			isWaste: false));

		registrator.PrototypesDb.Add(new FluidProductProto(
			id: ModIDs.Products.DtFuel,
			strings: Proto.CreateStr(ModIDs.Products.DtFuel,
				ModTranslation.Get("product_fluid.Product_dtfuel.name", "Fusion Core Fuel"),
				ModTranslation.Get("product_fluid.Product_dtfuel.description",
					"Fusion Core Fuel is an advanced fuel for fusion reactors, consisting of an optimized " +
					"mixture of deuterium and tritium. It provides high energy density and enables highly " +
					"efficient fusion reactions, serving as the foundation for next-generation energy systems.")),
			isStorable: true,
			canBeDiscarded: false,
			graphics: new FluidProductProto.Gfx(
				prefabPath: Option<string>.None,
				customIconPath: "Assets/FusionHorizon/ProductIcons/DTFuel.png",
				color: new ColorRgba(141, 63, 227, 255),
				transportColor: new ColorRgba(58, 95, 138, 255),
				transportAccentColor: new ColorRgba(252, 196, 255, 255)),
			isWaste: false));

		registrator.PrototypesDb.Add(new FluidProductProto(
			id: ModIDs.Products.PlasmaDirty,
			strings: Proto.CreateStr(ModIDs.Products.PlasmaDirty,
				ModTranslation.Get("product_fluid.Product_plasmadirty.name", "Plasma Dirty"),
				ModTranslation.Get("product_fluid.Product_plasmadirty.description",
					"An ionized fluid extracted from a fusion reactor, containing gaseous impurities such " +
					"as deuterium, helium, and tritium. The mixture exhibits unstable behavior and " +
					"irregular luminosity due to the presence of light nuclei that have not fully fused. " +
					"It is used as a byproduct of the process or for studies of isotopic recombination.")),
			isStorable: false,
			canBeDiscarded: false,
			graphics: new FluidProductProto.Gfx(
				prefabPath: Option<string>.None,
				customIconPath: "Assets/FusionHorizon/ProductIcons/PlasmaDirty.png",
				color: new ColorRgba(115, 0, 138, 255),
				transportColor: new ColorRgba(61, 90, 132, 255),
				transportAccentColor: new ColorRgba(43, 50, 61, 255)),
			isWaste: false));

		registrator.PrototypesDb.Add(new FluidProductProto(
			id: ModIDs.Products.PlasmaRefined,
			strings: Proto.CreateStr(ModIDs.Products.PlasmaRefined,
				ModTranslation.Get("product_fluid.Product_plasmarefined.name", "Refined Plasma"),
				ModTranslation.Get("product_fluid.Product_plasmarefined.description",
					"High-pressure plasma obtained by purifying raw plasma. It still contains residual " +
					"impurities and helium, making it more stable but not sufficiently pure. It requires " +
					"a further refining step before it can be used in non-nuclear industrial processes.")),
			isStorable: false,
			canBeDiscarded: false,
			graphics: new FluidProductProto.Gfx(
				prefabPath: Option<string>.None,
				customIconPath: "Assets/FusionHorizon/ProductIcons/PlasmaRefined.png",
				color: new ColorRgba(185, 0, 222, 255),
				transportColor: new ColorRgba(95, 143, 212, 255),
				transportAccentColor: new ColorRgba(0, 62, 157, 255)),
			isWaste: false));

		registrator.PrototypesDb.Add(new FluidProductProto(
			id: ModIDs.Products.Plasma,
			strings: Proto.CreateStr(ModIDs.Products.Plasma,
				ModTranslation.Get("product_fluid.Product_plasma.name", "Plasma"),
				ModTranslation.Get("product_fluid.Product_plasma.description",
					"Fully purified plasma obtained from the final stage of refining treated plasma. It is " +
					"free of impurities and has a stable composition, making it suitable for use in " +
					"advanced industrial processes.")),
			isStorable: true,
			canBeDiscarded: true,
			graphics: new FluidProductProto.Gfx(
				prefabPath: Option<string>.None,
				customIconPath: "Assets/FusionHorizon/ProductIcons/Plasma.png",
				color: new ColorRgba(226, 88, 255, 255),
				transportColor: new ColorRgba(163, 0, 163, 255),
				transportAccentColor: new ColorRgba(255, 133, 169, 255)),
			isWaste: false));

		// ----------------------------------------------------------------------------
		// Unit products
		// ----------------------------------------------------------------------------

		const string CANDU_ROD_PREFAB_PATH = "Assets/FusionHorizon/Model/CanduRod_Final.prefab";

		registrator.PrototypesDb.Add(new CountableProductProto(
			id: ModIDs.Products.CanduRod,
			strings: Proto.CreateStr(ModIDs.Products.CanduRod,
				ModTranslation.Get("product_unit.Product_Candu_rod.name", "CANDU Rod"),
				ModTranslation.Get("product_unit.Product_Candu_rod.description",
					"A CANDU fuel rod consisting of compacted uranium-bearing material enclosed in a " +
					"sturdy steel cladding, ready for use in heavy-water reactors.")),
			maxQuantityPerTransportedProduct: new Quantity(3),
			isStorable: true,
			graphics: new CountableProductProto.Gfx(
				prefabPath: CANDU_ROD_PREFAB_PATH,
				customIconPath: "Assets/FusionHorizon/ProductIcons/CANDURod_Icon.png",
				packingMode: CountableProductStackingMode.Auto,
				allowPackingNoise: false),
			isWaste: false));

		const string DUPIC_ROD_PREFAB_PATH = "Assets/FusionHorizon/Model/DupicRod_Final.prefab";

		// Fabricated directly from vanilla Spent Fuel (see Recipe_dupic_rod_reprocessing);
		// usable only by CANDU II/III (see MachineData.cs fuelPairs).
		registrator.PrototypesDb.Add(new CountableProductProto(
			id: ModIDs.Products.DupicRod,
			strings: Proto.CreateStr(ModIDs.Products.DupicRod,
				ModTranslation.Get("product_unit.Product_dupic_rod.name", "DUPIC Rod"),
				ModTranslation.Get("product_unit.Product_dupic_rod.description",
					"A fuel rod fabricated directly from spent light-water-reactor fuel via the OREOX " +
					"process — repeated oxidation/reduction cycles that mechanically pulverize and " +
					"re-sinter the spent fuel into new pellets, without ever chemically separating out " +
					"plutonium. Still fissile enough for a heavy-water-moderated CANDU reactor, though " +
					"less refined than dedicated CANDU or MOX rods. Usable only in CANDU II and CANDU " +
					"III reactors.")),
			maxQuantityPerTransportedProduct: new Quantity(3),
			isStorable: true,
			graphics: new CountableProductProto.Gfx(
				prefabPath: DUPIC_ROD_PREFAB_PATH,
				customIconPath: "Assets/FusionHorizon/ProductIcons/DUPICRod_Icon.png",
				packingMode: CountableProductStackingMode.Auto,
				allowPackingNoise: false),
			isWaste: false));

		const string DUPIC_SPENT_FUEL_PREFAB_PATH = "Assets/FusionHorizon/Model/DupicSpentFuel.prefab";

		// Burn output of a DUPIC Rod. Deliberately a distinct product from vanilla Spent
		// Fuel (see ModIDs.Products.DupicSpentFuel) so it can't be fed back into DUPIC
		// rod reprocessing; instead it feeds a Fast Breeder Reactor's breeding blanket.
		// radioactivity > 0 is what the vanilla Radioactive Waste Storage filters on
		// (IsStorable && Radioactivity > 0) — separate from RadioactiveWasteParam, an
		// optional param (see ResearchData.cs) that also makes a product decay into
		// another over time; not every radioactive product needs that second part.
		registrator.PrototypesDb.Add(new CountableProductProto(
			id: ModIDs.Products.DupicSpentFuel,
			strings: Proto.CreateStr(ModIDs.Products.DupicSpentFuel,
				ModTranslation.Get("product_unit.Product_dupic_spent_fuel.name", "DUPIC Spent Fuel"),
				ModTranslation.Get("product_unit.Product_dupic_spent_fuel.description",
					"Spent fuel discharged from a DUPIC Rod burned in a CANDU II or III reactor. Having " +
					"already been irradiated once in a light-water reactor before being refabricated into " +
					"a DUPIC Rod, its isotopic composition is too degraded for the simple mechanical OREOX " +
					"process to recycle a second time — it can no longer be reprocessed back into more " +
					"DUPIC Rods. It can still be reprocessed into Blanket Fuel for a Fast Breeder " +
					"Reactor's breeding blanket, same as regular Spent Fuel or Spent MOX.")),
			maxQuantityPerTransportedProduct: new Quantity(3),
			isStorable: true,
			radioactivity: 2,
			graphics: new CountableProductProto.Gfx(
				prefabPath: DUPIC_SPENT_FUEL_PREFAB_PATH,
				customIconPath: "Assets/FusionHorizon/ProductIcons/DupicSpentFuel_Icon.png",
				packingMode: CountableProductStackingMode.Auto,
				allowPackingNoise: false),
			isWaste: false));

		const string INDUSTRIAL_ISOTOPES_PREFAB_PATH = "Assets/FusionHorizon/Model/IndustrialIsotopes_Final.prefab";

		// Separated out of vanilla Fission Products (see Recipe_isotope_separation).
		// Radioactive (needs proper storage) but NOT given a RadioactiveWasteParam: it's
		// a stable product meant for active use, not something that should silently
		// decay into another product while sitting in storage.
		registrator.PrototypesDb.Add(new CountableProductProto(
			id: ModIDs.Products.IndustrialIsotopes,
			strings: Proto.CreateStr(ModIDs.Products.IndustrialIsotopes,
				ModTranslation.Get("product_unit.Product_industrial_isotopes.name", "Industrial Isotopes"),
				ModTranslation.Get("product_unit.Product_industrial_isotopes.description",
					"A refined mix of fission platinum-group metals (ruthenium, rhodium, palladium) and " +
					"sealed radioactive sources (caesium-137, strontium-90), separated out from Fission " +
					"Products. Stable and safe to store and transport as-is, unlike short-lived medical " +
					"isotopes such as technetium-99m. Used in advanced medical supply manufacturing.")),
			maxQuantityPerTransportedProduct: new Quantity(3),
			isStorable: true,
			radioactivity: 2,
			graphics: new CountableProductProto.Gfx(
				prefabPath: INDUSTRIAL_ISOTOPES_PREFAB_PATH,
				customIconPath: "Assets/FusionHorizon/ProductIcons/IndustrialIsotopes_Icon.png",
				packingMode: CountableProductStackingMode.Auto,
				allowPackingNoise: false),
			isWaste: false));

		const string DEPLETED_FISSION_PRODUCTS_PREFAB_PATH = "Assets/FusionHorizon/Model/DepletedFissionProducts_Final.prefab";

		// Residue of isotope separation. Deliberately a distinct product from vanilla
		// Fission Product (see ModIDs.Products.DepletedFissionProducts) so it can't be
		// re-fed into the same separation recipe. RadioactiveWasteParam is attached in
		// ResearchData.cs after registration, not here.
		registrator.PrototypesDb.Add(new CountableProductProto(
			id: ModIDs.Products.DepletedFissionProducts,
			strings: Proto.CreateStr(ModIDs.Products.DepletedFissionProducts,
				ModTranslation.Get("product_unit.Product_depleted_fission_products.name", "Depleted Fission Products"),
				ModTranslation.Get("product_unit.Product_depleted_fission_products.description",
					"The residual mix left over after Industrial Isotopes have been separated out of " +
					"Fission Products. Having lost its caesium-137/strontium-90 fraction — the isotopes " +
					"that dominate activity on a human timescale — it becomes safe to dispose of somewhat " +
					"sooner than untreated Fission Products, but must still be kept in radioactive waste " +
					"storage until it decays.")),
			maxQuantityPerTransportedProduct: new Quantity(3),
			isStorable: true,
			radioactivity: 6,
			graphics: new CountableProductProto.Gfx(
				prefabPath: DEPLETED_FISSION_PRODUCTS_PREFAB_PATH,
				customIconPath: "Assets/FusionHorizon/ProductIcons/DepletedFissionProducts_Icon.png",
				packingMode: CountableProductStackingMode.Auto,
				allowPackingNoise: false),
			isWaste: false));

		// Intermediate product bridging Plasma (a fluid) into recipes that otherwise only
		// take unit-item inputs — see Recipe_plasma_module_assembly.
		registrator.PrototypesDb.Add(new CountableProductProto(
			id: ModIDs.Products.PlasmaModule,
			strings: Proto.CreateStr(ModIDs.Products.PlasmaModule,
				ModTranslation.Get("product_unit.Product_plasma_module.name", "Plasma Module"),
				ModTranslation.Get("product_unit.Product_plasma_module.description",
					"A sealed glass-encased cartridge containing refined plasma, built to power advanced " +
					"medical equipment such as plasma scalpels and sterilization systems — plasma medicine " +
					"devices use sealed modules like this rather than raw plasma directly.")),
			maxQuantityPerTransportedProduct: new Quantity(3),
			isStorable: true,
			graphics: new CountableProductProto.Gfx(
				prefabPath: "Assets/FusionHorizon/Model/PlasmaModule_Final.prefab",
				customIconPath: "Assets/FusionHorizon/ProductIcons/PlasmaModule_Icon.png",
				packingMode: CountableProductStackingMode.Auto,
				allowPackingNoise: false),
			isWaste: false));

		// Top medical tier, produced by Recipe_medical_supplies_4_assembly.
		// MedicalSuppliesParam is attached in ResearchData.cs after registration, not
		// here — the vanilla Clinic (HospitalProto) scans every registered ProductProto
		// for that param at init time, so it picks this product up automatically once
		// the param exists; no changes to the Clinic itself are needed.
		registrator.PrototypesDb.Add(new CountableProductProto(
			id: ModIDs.Products.MedicalSupplies4,
			strings: Proto.CreateStr(ModIDs.Products.MedicalSupplies4,
				ModTranslation.Get("product_unit.Product_medical_supplies_4.name", "Medical Supplies IV"),
				ModTranslation.Get("product_unit.Product_medical_supplies_4.description",
					"The most advanced tier of medical supplies, combining Industrial Isotopes for " +
					"nuclear medicine (diagnostics, radiotherapy) with Plasma Modules for advanced " +
					"plasma-based medical equipment such as plasma scalpels and sterilization systems.")),
			maxQuantityPerTransportedProduct: new Quantity(3),
			isStorable: true,
			graphics: new CountableProductProto.Gfx(
				prefabPath: "Assets/FusionHorizon/Model/MedicalSupplies4_Final.prefab",
				customIconPath: "Assets/FusionHorizon/ProductIcons/MedicalSupplies4_Icon.png",
				packingMode: CountableProductStackingMode.Auto,
				allowPackingNoise: false),
			isWaste: false));
	}
}
