using System.Linq;
using System.Reflection;
using Mafi;
using Mafi.Collections.ImmutableCollections;
using Mafi.Core.Entities.Static.Layout;
using Mafi.Core.Ports.Io;

namespace FusionHorizon.Extensions;

// Adds ports to an already-built LayoutEntityProto via reflection. There is no public
// builder API for appending ports after construction (LayoutEntityProto.Layout has a
// private setter), so this reads/writes the private fields directly — the same
// reflection-based approach other DLL mods use when a builder doesn't expose something
// the underlying proto actually supports.
internal static class LayoutEntityExtensions {

	public static void AddExtraPorts(this LayoutEntityProto proto, params IoPortTemplate[] extraPorts) {
		EntityLayout layout = proto.Layout;

		var newPorts = layout.Ports.AddRange(extraPorts);

		var newLayout = new EntityLayout(
			sourceLayoutStr: layout.SourceLayoutStr,
			tiles: layout.LayoutTiles,
			vertices: layout.TerrainVertices,
			ports: newPorts,
			layoutParams: layout.LayoutParams,
			collapseVerticesThreshold: layout.CollapseVerticesThreshold,
			originTile: layout.OriginTile,
			sizeOverride: (layout.CoreMin, layout.CoreMax, layout.LayoutSize));

		proto.SetProperty(nameof(LayoutEntityProto.Layout), newLayout);
		proto.SetField(nameof(LayoutEntityProto.InputPorts), newPorts.Where(p => p.Type == IoPortType.Input).ToImmutableArray());
		proto.SetField(nameof(LayoutEntityProto.OutputPorts), newPorts.Where(p => p.Type == IoPortType.Output).ToImmutableArray());
	}

	// Generic reflection helpers: walk the type hierarchy to find a private field, or a
	// property (writable directly, or via its auto-generated backing field otherwise).
	private static void SetField<T>(this T obj, string fieldName, object? value) where T : class {
		var type = obj.GetType();
		while (type is not null) {
			var field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			if (field is not null) {
				field.SetValue(obj, value);
				return;
			}
			type = type.BaseType;
		}
		throw new System.InvalidOperationException($"Field '{fieldName}' not found on '{obj.GetType().FullName}'.");
	}

	private static void SetProperty<T>(this T obj, string propName, object value) where T : class {
		var type = obj.GetType();
		var propInfo = type.GetProperty(propName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			?? throw new System.InvalidOperationException($"Property '{propName}' not found.");

		if (propInfo.CanWrite) {
			propInfo.SetValue(obj, value, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, null, null);
			return;
		}

		while (type != null) {
			var backingField = type.GetField($"<{propName}>k__BackingField", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (backingField != null) {
				backingField.SetValue(obj, value);
				return;
			}
			type = type.BaseType;
		}
		throw new System.InvalidOperationException($"Backing field of property '{propName}' not found.");
	}
}
