using ProductID = Mafi.Core.Products.ProductProto.ID;

namespace FusionHorizon;

// ProductProto.ID for every custom product this mod registers (see ProductData.cs).
// A mod's own product IDs are just strings wrapped in a strongly-typed ID struct — pick
// any string not already used by vanilla or another mod. Grouping them here, instead of
// inline string literals, keeps every id typo-proof and easy to find/reuse across files.
public static partial class ModIDs {

	public static partial class Products {

		public static readonly ProductID CanduRod       = new("Product_Candu_rod");
		public static readonly ProductID HeavyWater     = new("Product_heavy_water");
		public static readonly ProductID HeavyWaterHigh = new("Product_heavy_water_high");
		public static readonly ProductID DupicRod       = new("Product_dupic_rod");
		public static readonly ProductID DupicSpentFuel = new("Product_dupic_spent_fuel");

		public static readonly ProductID IndustrialIsotopes      = new("Product_industrial_isotopes");
		public static readonly ProductID DepletedFissionProducts = new("Product_depleted_fission_products");
		public static readonly ProductID PlasmaModule             = new("Product_plasma_module");
		public static readonly ProductID MedicalSupplies4          = new("Product_medical_supplies_4");

		public static readonly ProductID HeavyWaterHighTritium = new("Product_heavy_water_high_tritium");
		public static readonly ProductID HeavyWaterSp          = new("Product_heavy_water_sp");

		public static readonly ProductID Deuterium     = new("Product_deuterium");
		public static readonly ProductID Helium        = new("Product_helium");
		public static readonly ProductID HeliumHot     = new("Product_helium_hot");
		public static readonly ProductID Tritium       = new("Product_tritium");
		public static readonly ProductID DtFuel        = new("Product_dtfuel");
		public static readonly ProductID PlasmaDirty   = new("Product_plasmadirty");
		public static readonly ProductID PlasmaRefined = new("Product_plasmarefined");
		public static readonly ProductID Plasma        = new("Product_plasma");

	}

}
