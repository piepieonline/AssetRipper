using AssetRipper.Assets.Collections;
using AssetRipper.Assets.Export;
using AssetRipper.Assets.Export.Dependencies;
using AssetRipper.Assets.Export.Yaml;
using AssetRipper.Assets.Interfaces;
using AssetRipper.Assets.Metadata;
using AssetRipper.Primitives;
using AssetRipper.Yaml;
using System.Text.Json;
using System.Diagnostics;

namespace AssetRipper.Assets;

/// <summary>
/// The artificial base class for all generated Unity classes which inherit from Object.
/// </summary>
public abstract class UnityObjectBase : UnityAssetBase, IUnityObjectBase
{
	public static Dictionary<string, string> pathToGUID = null;
	public static Dictionary<AssetInfo, long> assetInfoToFileID = new Dictionary<AssetInfo, long>();
	
	protected UnityObjectBase(AssetInfo assetInfo)
	{
		if (pathToGUID == null)
		{
			try
			{
				pathToGUID = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(Path.Join(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "..", "/current_paths.json")));
			}
			catch
			{
				pathToGUID = new Dictionary<string, string>();
				var preColor = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"WARNING: No existing GUIDs found, this will break things. Abort? (y/n)");
				Console.ForegroundColor = preColor;
				if (Console.ReadLine() != "n") throw new Exception("Missing current_paths.json");
			}
		}

		AssetInfo = assetInfo;

		if (PathID != 0)
		{
			string guidKey;
			if (Collection.Name.StartsWith("cab"))
			{
				guidKey = $"addressables-{PathID}";
			}
			else
			{
				guidKey = $"{Collection.Name}-{PathID}";
			}
			
			if (pathToGUID.ContainsKey(guidKey))
			{
				GUID = UnityGuid.Parse(pathToGUID[guidKey]);

				if(pathToGUID.ContainsKey(guidKey + "-FileID"))
				{
					assetInfoToFileID[assetInfo] = long.Parse(pathToGUID[guidKey + "-FileID"]);
				}
			}
			else
			{
				var preColor = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine($"MISSING GUID: Generating GUID for {guidKey}");
				Console.ForegroundColor = preColor;
				// For addressables, predictable GUIDS
				// Otherwise, random at generation time
				GUID = guidKey.StartsWith("addressables") ? UnityGuid.Md5Hash(guidKey) : UnityGuid.NewGuid();
				pathToGUID[guidKey] = GUID.ToString();
			}
		}
		else
		{
			GUID = UnityGuid.NewGuid();
		}
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OriginalPathDetails? originalPathDetails;
	public AssetInfo AssetInfo { get; }
	public AssetCollection Collection => AssetInfo.Collection;
	public int ClassID => AssetInfo.ClassID;
	public long PathID => AssetInfo.PathID;
	public virtual string ClassName => GetType().Name;
	public UnityGuid GUID { get; set; }
	public IUnityObjectBase? MainAsset { get; set; }

	public YamlDocument ExportYamlDocument(IExportContainer container)
	{
		YamlDocument document = new();
		YamlMappingNode root = document.CreateMappingRoot();
		root.Tag = ClassID.ToString();
		root.Anchor = container.GetExportID(this).ToString();
		root.Add(ClassName, this.ExportYaml(container));
		return document;
	}

	public IEnumerable<(FieldName, PPtr<IUnityObjectBase>)> FetchDependencies()
	{
		return FetchDependencies((FieldName?)null);
	}

	/// <summary>
	/// Get the best name for this object.
	/// </summary>
	/// <remarks>
	/// In order of preference:<br/>
	/// 1. <see cref="IHasNameString.NameString"/><br/>
	/// 2. <see cref="OriginalName"/><br/>
	/// 3. <see cref="ClassName"/><br/>
	/// <see cref="OriginalName"/> has secondary preference because file importers can create assets with a different name from the file.
	/// </remarks>
	/// <returns>A nonempty string.</returns>
	public string GetBestName()
	{
		string? name = (this as IHasNameString)?.NameString;
		if (!string.IsNullOrEmpty(name))
		{
			return name;
		}
		else if (!string.IsNullOrEmpty(OriginalName))
		{
			return OriginalName;
		}
		else
		{
			return ClassName;
		}
	}

	/// <summary>
	/// The original path of the asset's file, relative to the project root.
	/// </summary>
	public string? OriginalPath
	{
		get => originalPathDetails?.ToString();
		set
		{
			if (value is null)
			{
				originalPathDetails = null;
			}
			else
			{
				originalPathDetails ??= new();
				originalPathDetails.Directory = Path.GetDirectoryName(value);
				originalPathDetails.Name = Path.GetFileNameWithoutExtension(value);
				originalPathDetails.Extension = RemovePeriod(Path.GetExtension(value));
			}
		}
	}

	/// <summary>
	/// The original directory containing the asset's file, relative to the project root.
	/// </summary>
	public string? OriginalDirectory
	{
		get => originalPathDetails?.Directory;
		set
		{
			if (originalPathDetails is not null)
			{
				originalPathDetails.Directory = value;
			}
			else if (value is not null)
			{
				originalPathDetails = new();
				originalPathDetails.Directory = value;
			}
		}
	}

	/// <summary>
	/// The original name of the asset's file.
	/// </summary>
	public string? OriginalName
	{
		get => originalPathDetails?.Name;
		set
		{
			if (originalPathDetails is not null)
			{
				originalPathDetails.Name = value;
			}
			else if (value is not null)
			{
				originalPathDetails = new();
				originalPathDetails.Name = value;
			}
		}
	}

	/// <summary>
	/// The original extension of the asset's file, not including the period.
	/// </summary>
	public string? OriginalExtension
	{
		get => originalPathDetails?.Extension;
		set
		{
			if (originalPathDetails is not null)
			{
				originalPathDetails.Extension = RemovePeriod(value);
			}
			else if (value is not null)
			{
				originalPathDetails = new();
				originalPathDetails.Extension = RemovePeriod(value);
			}
		}
	}

	/// <summary>
	/// The name of the asset bundle containing this asset.
	/// </summary>
	public string? AssetBundleName { get; set; }

	[return: NotNullIfNotNull(nameof(str))]
	private static string? RemovePeriod(string? str)
	{
		return string.IsNullOrEmpty(str) || str[0] != '.' ? str : str.Substring(1);
	}

	private sealed class OriginalPathDetails
	{
		public string? Directory { get; set; }
		public string? Name { get; set; }
		/// <summary>
		/// Not including the period
		/// </summary>
		public string? Extension { get; set; }
		public string NameWithExtension => string.IsNullOrEmpty(Extension) ? Name ?? "" : $"{Name}.{Extension}";

		public override string? ToString()
		{
			string result = Directory is null
				? NameWithExtension
				: Path.Combine(Directory, NameWithExtension);
			return string.IsNullOrEmpty(result) ? null : result;
		}
	}
}
