using MachineID = Mafi.Core.Factory.Machines.MachineProto.ID;
using ReactorID = Mafi.Core.Entities.Static.StaticEntityProto.ID;

namespace FusionHorizon;

// Central place for every strongly-typed proto ID this mod defines, split by category
// across ModIDs.*.cs partial-class files (Machines/Products/Recipes/Research). Keeping
// IDs here instead of inline string literals avoids typos across files and makes every
// custom proto this mod owns easy to find in one place.
public static partial class ModIDs {

	public static partial class Machines {

		// MachineProto.ID: ordinary machines built via MachineProtoBuilder in MachineData.cs.
		public static readonly MachineID HeatExchangerCandu   = new("Heat_exchanger_candu");
		public static readonly MachineID IncinerationPlantT2  = new("Incineration_plant_T2");
		public static readonly MachineID TritiumSeparator     = new("Tritium_separator");

		// StaticEntityProto.ID: nuclear reactors use NuclearReactorProto, which derives from
		// StaticEntityProto rather than MachineProto, so they need this separate ID type and
		// their own registration path (see RegisterCanduReactors/RegisterFusionReactor in
		// MachineData.cs) instead of MachineProtoBuilder.
		public static readonly ReactorID CanduReactorT1   = new("Candu_Reactor_T1_build");
		public static readonly ReactorID CanduReactorT2   = new("Candu_Reactor_T2_Build");
		public static readonly ReactorID CanduReactorT3   = new("Candu_Reactor_T3_build");
		public static readonly ReactorID FusionReactorT1  = new("Fusion_Reactor_t1");

	}

}
