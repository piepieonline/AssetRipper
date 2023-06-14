﻿using AssetRipper.Assets;
using AssetRipper.Assets.Export;
using AssetRipper.Export.UnityProjects.Project.Collections;
using AssetRipper.Export.UnityProjects.Project.Exporters;
using AssetRipper.Primitives;
using AssetRipper.SourceGenerated.Classes.ClassID_117;
using AssetRipper.SourceGenerated.Classes.ClassID_187;
using AssetRipper.SourceGenerated.Classes.ClassID_188;
using AssetRipper.SourceGenerated.Classes.ClassID_28;
using AssetRipper.SourceGenerated.Classes.ClassID_43;
using AssetRipper.SourceGenerated.Classes.ClassID_89;
using AssetRipper.SourceGenerated.Extensions;

namespace AssetRipper.Export.UnityProjects
{
	public sealed class YamlStreamedAssetExportCollection : AssetExportCollection
	{
		public YamlStreamedAssetExportCollection(IAssetExporter assetExporter, IUnityObjectBase asset) : base(assetExporter, asset)
		{
		}

		protected override bool ExportInner(IExportContainer container, string filePath, string dirPath)
		{
			//Possible improvement:
			//
			//The code for all these is very similar.
			//An interface could be added to them during source generation in order to avoid this switch expression.

			return Asset switch
			{
				IMesh mesh => ExportMesh(container, filePath, dirPath, mesh),
				ITexture2D texture2D => ExportTexture2D(container, filePath, dirPath, texture2D),
				ITexture3D texture3D => ExportTexture3D(container, filePath, dirPath, texture3D),
				ITexture2DArray texture2DArray => ExportTexture2DArray(container, filePath, dirPath, texture2DArray),
				ICubemapArray cubemapArray => ExportCubemapArray(container, filePath, dirPath, cubemapArray),
				_ => false,
			};
		}

		private bool ExportMesh(IExportContainer container, string filePath, string dirPath, IMesh mesh)
		{
			bool result;
			if (mesh.Has_StreamData_C43())
			{
				ulong offset = mesh.StreamData_C43.GetOffset();
				Utf8String path = mesh.StreamData_C43.Path;
				uint size = mesh.StreamData_C43.Size;
				if (mesh.VertexData_C43 is not null && mesh.VertexData_C43.Data.Length == 0 && mesh.StreamData_C43.IsSet())
				{
					mesh.VertexData_C43.Data = mesh.StreamData_C43.GetContent(mesh.Collection);
					mesh.StreamData_C43.ClearValues();
					result = base.ExportInner(container, filePath, dirPath);
					mesh.VertexData_C43.Data = Array.Empty<byte>();
				}
				else
				{
					mesh.StreamData_C43.ClearValues();
					result = base.ExportInner(container, filePath, dirPath);
				}
				mesh.StreamData_C43.SetOffset(offset);
				mesh.StreamData_C43.Path = path;
				mesh.StreamData_C43.Size = size;
			}
			else
			{
				result = base.ExportInner(container, filePath, dirPath);
			}

			return result;
		}

		/// <summary>
		/// Also handles <see cref="ICubemap"/> by inheritance
		/// </summary>
		private bool ExportTexture2D(IExportContainer container, string filePath, string dirPath, ITexture2D texture)
		{
			bool result;
			if (texture.Has_StreamData_C28())
			{
				ulong offset = texture.StreamData_C28.GetOffset();
				Utf8String path = texture.StreamData_C28.Path;
				uint size = texture.StreamData_C28.Size;
				if (texture.ImageData_C28.Length == 0 && texture.StreamData_C28.IsSet())
				{
					texture.ImageData_C28 = texture.StreamData_C28.GetContent(texture.Collection);
					texture.StreamData_C28.ClearValues();
					result = base.ExportInner(container, filePath, dirPath);
					texture.ImageData_C28 = Array.Empty<byte>();
				}
				else
				{
					texture.StreamData_C28.ClearValues();
					result = base.ExportInner(container, filePath, dirPath);
				}
				texture.StreamData_C28.SetOffset(offset);
				texture.StreamData_C28.Path = path;
				texture.StreamData_C28.Size = size;
			}
			else
			{
				result = base.ExportInner(container, filePath, dirPath);
			}

			return result;
		}

		private bool ExportTexture3D(IExportContainer container, string filePath, string dirPath, ITexture3D texture)
		{
			bool result;
			if (texture.Has_StreamData_C117())
			{
				ulong offset = texture.StreamData_C117.GetOffset();
				Utf8String path = texture.StreamData_C117.Path;
				uint size = texture.StreamData_C117.Size;
				if (texture.ImageData_C117.Length == 0 && texture.StreamData_C117.IsSet())
				{
					texture.ImageData_C117 = texture.StreamData_C117.GetContent(texture.Collection);
					texture.StreamData_C117.ClearValues();
					result = base.ExportInner(container, filePath, dirPath);
					texture.ImageData_C117 = Array.Empty<byte>();
				}
				else
				{
					texture.StreamData_C117.ClearValues();
					result = base.ExportInner(container, filePath, dirPath);
				}
				texture.StreamData_C117.SetOffset(offset);
				texture.StreamData_C117.Path = path;
				texture.StreamData_C117.Size = size;
			}
			else
			{
				result = base.ExportInner(container, filePath, dirPath);
			}

			return result;
		}

		private bool ExportTexture2DArray(IExportContainer container, string filePath, string dirPath, ITexture2DArray texture)
		{
			bool result;
			if (texture.Has_StreamData_C187())
			{
				ulong offset = texture.StreamData_C187.GetOffset();
				Utf8String path = texture.StreamData_C187.Path;
				uint size = texture.StreamData_C187.Size;
				if (texture.ImageData_C187.Length == 0 && texture.StreamData_C187.IsSet())
				{
					texture.ImageData_C187 = texture.StreamData_C187.GetContent(texture.Collection);
					texture.StreamData_C187.ClearValues();
					result = base.ExportInner(container, filePath, dirPath);
					texture.ImageData_C187 = Array.Empty<byte>();
				}
				else
				{
					texture.StreamData_C187.ClearValues();
					result = base.ExportInner(container, filePath, dirPath);
				}
				texture.StreamData_C187.SetOffset(offset);
				texture.StreamData_C187.Path = path;
				texture.StreamData_C187.Size = size;
			}
			else
			{
				result = base.ExportInner(container, filePath, dirPath);
			}

			return result;
		}

		private bool ExportCubemapArray(IExportContainer container, string filePath, string dirPath, ICubemapArray texture)
		{
			bool result;
			if (texture.Has_StreamData_C188())
			{
				ulong offset = texture.StreamData_C188.GetOffset();
				Utf8String path = texture.StreamData_C188.Path;
				uint size = texture.StreamData_C188.Size;
				if (texture.ImageData_C188.Length == 0 && texture.StreamData_C188.IsSet())
				{
					texture.ImageData_C188 = texture.StreamData_C188.GetContent(texture.Collection);
					texture.StreamData_C188.ClearValues();
					result = base.ExportInner(container, filePath, dirPath);
					texture.ImageData_C188 = Array.Empty<byte>();
				}
				else
				{
					texture.StreamData_C188.ClearValues();
					result = base.ExportInner(container, filePath, dirPath);
				}
				texture.StreamData_C188.SetOffset(offset);
				texture.StreamData_C188.Path = path;
				texture.StreamData_C188.Size = size;
			}
			else
			{
				result = base.ExportInner(container, filePath, dirPath);
			}

			return result;
		}
	}
}
