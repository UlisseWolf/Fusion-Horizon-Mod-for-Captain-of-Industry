using ResNodeID = Mafi.Core.Research.ResearchNodeProto.ID;

namespace FusionHorizon;

// ResearchNodeProto.ID for every custom research node this mod registers (see
// ResearchData.cs). A node can require both custom and vanilla research nodes as
// parents, letting a mod graft its own tech tree branches onto the base game's.
public static partial class ModIDs {

	public static partial class Research {

		public static readonly ResNodeID CanduReactor     = new("Research_Candu_Reactor");
		public static readonly ResNodeID CanduReactor3     = new("Research_Candu_Reactor_3");
		public static readonly ResNodeID DupicRod          = new("Research_Dupic_Rod");
		public static readonly ResNodeID FusionReactor     = new("Research_Fusion_Reactor");
		public static readonly ResNodeID PlasmaIncinerator = new("Research_Plasma_Inceneritor");
		public static readonly ResNodeID IsotopeSeparation = new("Research_Isotope_Separation");
		public static readonly ResNodeID MedicalSupplies4  = new("Research_Medical_Supplies_4");

	}

}
