﻿using AssetRipper.Assets;
using AssetRipper.Assets.Export;
using AssetRipper.Export.UnityProjects.Configuration;
using AssetRipper.Export.UnityProjects.Project.Collections;

namespace AssetRipper.Export.UnityProjects.Textures;

public sealed class TextureArrayAssetExportCollection : AssetExportCollection
{
	public TextureArrayAssetExportCollection(TextureArrayAssetExporter assetExporter, IUnityObjectBase asset) : base(assetExporter, asset)
	{
	}

	protected override string GetExportExtension(IUnityObjectBase asset)
	{
		return ((TextureArrayAssetExporter)AssetExporter).ImageExportFormat.GetFileExtension();
	}

	protected override IUnityObjectBase CreateImporter(IExportContainer container)
	{
		return ImporterFactory.GenerateTextureImporter(container, Asset);
	}
}
