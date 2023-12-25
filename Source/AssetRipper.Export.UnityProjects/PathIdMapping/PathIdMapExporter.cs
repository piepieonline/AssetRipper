using AssetRipper.Assets;
using AssetRipper.Assets.Bundles;
using AssetRipper.Assets.Collections;
using AssetRipper.Assets.Interfaces;
using AssetRipper.SourceGenerated.Classes.ClassID_27;
using AssetRipper.SourceGenerated.Classes.ClassID_43;
using AssetRipper.SourceGenerated.Classes.ClassID_49;
using AssetRipper.SourceGenerated.Classes.ClassID_83;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace AssetRipper.Export.UnityProjects.PathIdMapping;

public sealed class PathIdMapExporter : IPostExporter
{
	public void DoPostExport(Ripper ripper)
	{
		SerializedGameInfo gameInfo = new();
		GameBundle gameBundle = ripper.GameStructure.FileCollection;
		foreach (SerializedAssetCollection collection in gameBundle.FetchAssetCollections().OfType<SerializedAssetCollection>())
		{
			SerializedFileInfo fileInfo = new()
			{
				Name = collection.Name,
			};
			gameInfo.Files.Add(fileInfo);
			foreach (IUnityObjectBase asset in collection)
			{
				// if (asset is IMesh or ITexture or IAudioClip or ITextAsset)//Commonly useful asset types
				{
					fileInfo.Assets.Add(new()
					{
						Name = (asset as IHasNameString)?.NameString,
						Type = asset.ClassName,
						PathID = asset.PathID,
						GUID = asset.GUID.ToString()
					});
				}
			}
		}

		string outputDirectory = ripper.Settings.AuxiliaryFilesPath;
		
		Directory.CreateDirectory(outputDirectory);
		using FileStream pathIdMapStream = File.Create(Path.Combine(outputDirectory, "path_id_map.json"));
		JsonSerializer.Serialize(pathIdMapStream, gameInfo, SerializedGameInfoSerializerContext.Default.SerializedGameInfo);
		pathIdMapStream.Flush();

		var options = new JsonSerializerOptions(JsonSerializerDefaults.General);
		// JsonSerializerContext context = JsonSerializerContext.
		// JsonTypeInfo<Dictionary<string, string>> typeInfo = new JsonTypeInfo<Dictionary<string, string>>();
		// JsonConvert.SerializeObject();
		File.WriteAllText(Path.Combine(outputDirectory, "current_paths_new.json"), JsonSerializer.Serialize(UnityObjectBase.pathToGUID, options));
	}
}
