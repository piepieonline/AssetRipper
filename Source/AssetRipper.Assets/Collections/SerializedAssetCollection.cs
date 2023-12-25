﻿using AssetRipper.Assets.Bundles;
using AssetRipper.Assets.IO;
using AssetRipper.Assets.Metadata;
using AssetRipper.IO.Files.SerializedFiles;
using AssetRipper.IO.Files.SerializedFiles.Parser;

namespace AssetRipper.Assets.Collections;

/// <summary>
/// A collection of assets read from a <see cref="SerializedFile"/>.
/// </summary>
public sealed class SerializedAssetCollection : AssetCollection
{
	private FileIdentifier[]? DependencyIdentifiers { get; set; }

	private SerializedAssetCollection(Bundle bundle) : base(bundle)
	{
	}

	/// <inheritdoc/>
	protected override bool IsCompatibleDependency(AssetCollection dependency)
	{
		return dependency is SerializedAssetCollection or ProcessedAssetCollection;
	}

	internal void InitializeDependencyList()
	{
		if (Dependencies.Count > 1)
		{
			throw new Exception("Dependency list has already been initialized.");
		}
		if (DependencyIdentifiers is not null)
		{
			for (int i = 0; i < DependencyIdentifiers.Length; i++)
			{
				FileIdentifier identifier = DependencyIdentifiers[i];
				SetDependency(i + 1, Bundle.ResolveCollection(identifier));
			}
			DependencyIdentifiers = null;
		}
	}

	/// <summary>
	/// Creates a <see cref="SerializedAssetCollection"/> from a <see cref="SerializedFile"/>.
	/// </summary>
	/// <remarks>
	/// The new <see cref="SerializedAssetCollection"/> is automatically added to the <paramref name="bundle"/>.
	/// </remarks>
	/// <param name="bundle">The <see cref="Bundle"/> to add this collection to.</param>
	/// <param name="file">The <see cref="SerializedFile"/> from which to make this collection.</param>
	/// <param name="factory">A factory for creating assets.</param>
	/// <param name="defaultVersion">The default version to use if the file does not have a version, ie the version has been stripped.</param>
	/// <returns>The new collection.</returns>
	internal static SerializedAssetCollection FromSerializedFile(Bundle bundle, SerializedFile file, AssetFactoryBase factory, UnityVersion defaultVersion = default)
	{
		SerializedAssetCollection collection = new SerializedAssetCollection(bundle)
		{
			Name = file.NameFixed,
			Version = file.Version.IsEqual(0, 0, 0) ? defaultVersion : file.Version,
			Platform = file.Platform,
			Flags = file.Flags,
			EndianType = file.EndianType,
		};
		FileIdentifier[] fileDependencies = file.Metadata.Externals;
		if (fileDependencies.Length > 0)
		{
			collection.DependencyIdentifiers = new FileIdentifier[fileDependencies.Length];
			Array.Copy(fileDependencies, collection.DependencyIdentifiers, fileDependencies.Length);
		}
		ReadData(collection, file, factory);
		return collection;
	}

	private static void ReadData(SerializedAssetCollection collection, SerializedFile file, AssetFactoryBase factory)
	{
		for (int i = 0; i < file.Metadata.Object.Length; i++)
		{
			ObjectInfo objectInfo = file.Metadata.Object[i];
			SerializedType? type = objectInfo.GetSerializedType(file.Metadata.Types);
			int classID = objectInfo.TypeID < 0 ? 114 : objectInfo.TypeID;
			AssetInfo assetInfo = new AssetInfo(collection, objectInfo.FileID, classID);
			IUnityObjectBase? asset = factory.ReadAsset(assetInfo, objectInfo.ObjectData, type);
			if (asset is not null)
			{
				collection.AddAsset(asset);
			}
		}
	}
}
