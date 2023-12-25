﻿using AssetRipper.Assets;
using AssetRipper.Assets.Collections;
using AssetRipper.Assets.Export;
using AssetRipper.Assets.Metadata;
using AssetRipper.Import.Logging;
using AssetRipper.IO.Files;
using AssetRipper.IO.Files.SerializedFiles;

namespace AssetRipper.Export.UnityProjects
{
	public class FailExportCollection : IExportCollection
	{
		public FailExportCollection(IAssetExporter assetExporter, IUnityObjectBase asset)
		{
			AssetExporter = assetExporter ?? throw new ArgumentNullException(nameof(assetExporter));
			m_asset = asset ?? throw new ArgumentNullException(nameof(asset));
		}

		public bool Export(IExportContainer container, string projectDirectory)
		{
			Logger.Log(LogType.Warning, LogCategory.Export, $"Unable to export asset {Name}");
			return false;
		}

		public bool IsContains(IUnityObjectBase asset)
		{
			return asset == m_asset;
		}

		public long GetExportID(IUnityObjectBase asset)
		{
			if (asset == m_asset)
			{
				return ExportIdHandler.GetMainExportID(m_asset);
			}
			throw new ArgumentException(null, nameof(asset));
		}

		public UnityGuid GetExportGUID(IUnityObjectBase _)
		{
			throw new NotSupportedException();
		}

		public MetaPtr CreateExportPointer(IUnityObjectBase asset, bool isLocal)
		{
			if (isLocal)
			{
				throw new ArgumentException(null, nameof(isLocal));
			}

			long exportId = GetExportID(asset);
			AssetType type = AssetExporter.ToExportType(asset);
			return new MetaPtr(exportId, UnityGuid.MissingReference, type);
		}

		public IAssetExporter AssetExporter { get; }
		public AssetCollection File => m_asset.Collection;
		public TransferInstructionFlags Flags => File.Flags;
		public IEnumerable<IUnityObjectBase> Assets
		{
			get { yield return m_asset; }
		}
		public string Name => m_asset.GetBestName();

		private readonly IUnityObjectBase m_asset;
	}
}
