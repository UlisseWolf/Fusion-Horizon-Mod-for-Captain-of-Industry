using FusionHorizon.Data;
using Mafi;
using Mafi.Collections;
using Mafi.Core.Game;
using Mafi.Core.Mods;
using Mafi.Core.Prototypes;

namespace FusionHorizon;

// Entry point for a Captain of Industry DLL mod. Every mod needs exactly one class
// implementing IMod; its full type name (namespace + class) must match
// "primary_mod_class_name" in manifest.json, and the built assembly name must match
// "primary_dlls".
public sealed class FusionHorizonMod : IMod {

	public bool IsUiOnly => false;

	public Option<IConfig> ModConfig => default;

	public ModManifest Manifest { get; private set; }

	public ModJsonConfig JsonConfig { get; }

	public FusionHorizonMod(ModManifest manifest) {
		Manifest = manifest;
		JsonConfig = new(this);

		Log.Info($"{manifest.DisplayName} v{manifest.Version}");
	}

	// Called once at startup to register everything the mod adds (products, recipes,
	// machines, research). Each RegisterData<T>() call runs one IModData class's
	// RegisterData(ProtoRegistrator) method. Order matters here: later classes can look
	// up protos registered by earlier ones via registrator.PrototypesDb.GetOrThrow(...),
	// so register in dependency order (products/machines before the recipes that bind to
	// them, research last since it references products/recipes/machines).
	public void RegisterPrototypes(ProtoRegistrator registrator) {
		Log.Info("[Fusion Horizon] Registering prototypes");

		ModTranslation.Initialize(Manifest.RootDirectoryPath);

		registrator.RegisterData<ProductData>();
		registrator.RegisterData<MachineData>();
		registrator.RegisterData<RecipesData>();
		registrator.RegisterData<ResearchData>();
	}

	// Remaining IMod lifecycle hooks. Unused here, but every mod must implement them:
	// RegisterDependencies for DI container bindings, EarlyInit/Initialize for runtime
	// setup, MigrateJsonConfig for save-compatible config upgrades, Dispose for cleanup.
	public void RegisterDependencies(DependencyResolverBuilder depBuilder, ProtosDb protosDb, bool gameWasLoaded) { }

	public void EarlyInit(DependencyResolver resolver) { }

	public void Initialize(DependencyResolver resolver, bool gameWasLoaded) { }

	public void MigrateJsonConfig(VersionSlim savedVersion, Dict<string, object> savedValues) { }

	public void Dispose() { }
}
