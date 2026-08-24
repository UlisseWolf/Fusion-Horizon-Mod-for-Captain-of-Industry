using RecipeID = Mafi.Core.Factory.Recipes.RecipeProto.ID;

namespace FusionHorizon;

// RecipeProto.ID for every custom recipe this mod registers (see RecipesData.cs). A
// recipe's ID is independent from the machine(s) it runs on — the same recipe can be
// bound to multiple machine tiers via .BindTo(...), and one machine can host several
// different recipes.
public static partial class ModIDs {

	public static partial class Recipes {

		public static readonly RecipeID CanduRod               = new("Recipe_Candu_Rod");
		public static readonly RecipeID HeavyWater              = new("Recipe_heavy_water");
		public static readonly RecipeID PrimarySteamGenerator   = new("Recipe_primary_steam_generator");
		public static readonly RecipeID PrimarySteamGeneratorSp = new("Recipe_primary_steam_generator_sp");
		public static readonly RecipeID DupicRodReprocessing    = new("Recipe_dupic_rod_reprocessing");
		public static readonly RecipeID DupicSpentFuelToBlanket = new("Recipe_dupic_spent_fuel_to_blanket");

		public static readonly RecipeID IsotopeSeparation        = new("Recipe_isotope_separation");
		public static readonly RecipeID PlasmaModuleAssembly     = new("Recipe_plasma_module_assembly");
		public static readonly RecipeID MedicalSupplies4Assembly = new("Recipe_medical_supplies_4_assembly");

		public static readonly RecipeID TritiumSeparation = new("Recipe_tritium_separation");

		public static readonly RecipeID Deuterium                   = new("Recipe_deuterium");
		public static readonly RecipeID Tritium                     = new("Recipe_tritium");
		public static readonly RecipeID DtFuel                      = new("Recipe_dtfuel");
		public static readonly RecipeID PrimarySteamGeneratorHelium = new("Recipe_primary_steam_generator_helium");
		public static readonly RecipeID RefinedPlasma               = new("Recipe_refinedplasma");
		public static readonly RecipeID Plasma                      = new("Recipe_plasma");

		public static readonly RecipeID PlasmaTrash          = new("Recipe_plasma_trash");
		public static readonly RecipeID PlasmaTrashCompact   = new("Recipe_plasma_trash_compact");
		public static readonly RecipeID HeliumDischargedT1   = new("Helium_discharged_T1");
		public static readonly RecipeID HeliumDischargedT2   = new("Helium_discharged_T2");

		public static readonly RecipeID Bioethanol       = new("Recipe_Bioethanol");
		public static readonly RecipeID OrganicPesticide = new("Recipe_Organic_pesticide");

	}

}
