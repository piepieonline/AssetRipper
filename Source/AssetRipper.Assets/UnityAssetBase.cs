﻿using AssetRipper.Assets.Cloning;
using AssetRipper.Assets.Export;
using AssetRipper.Assets.Export.Dependencies;
using AssetRipper.Assets.Interfaces;
using AssetRipper.Assets.IO.Reading;
using AssetRipper.Assets.IO.Serialization;
using AssetRipper.Assets.IO.Writing;
using AssetRipper.Assets.Metadata;
using AssetRipper.IO.Endian;
using AssetRipper.Yaml;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;

namespace AssetRipper.Assets;

/// <summary>
/// The artificial base class for all generated Unity classes
/// </summary>
public abstract class UnityAssetBase : IUnityAssetBase, IAssetReadable
{
	public virtual void ReadEditor(AssetReader reader) => throw MethodNotSupported();

	public virtual void ReadRelease(AssetReader reader) => throw MethodNotSupported();

	public virtual void ReadEditor(ref EndianSpanReader reader) => throw MethodNotSupported();

	public virtual void ReadRelease(ref EndianSpanReader reader) => throw MethodNotSupported();

	public virtual void WriteEditor(AssetWriter writer) => throw MethodNotSupported();

	public virtual void WriteRelease(AssetWriter writer) => throw MethodNotSupported();

	public virtual YamlNode ExportYamlEditor(IExportContainer container) => throw MethodNotSupported();

	public virtual YamlNode ExportYamlRelease(IExportContainer container) => throw MethodNotSupported();

	public virtual IEnumerable<PPtr<IUnityObjectBase>> FetchDependencies(DependencyContext context)
	{
		return Enumerable.Empty<PPtr<IUnityObjectBase>>();
	}

	public virtual IEnumerable<(FieldName, PPtr<IUnityObjectBase>)> FetchDependencies(FieldName? parent)
	{
		return Enumerable.Empty<(FieldName, PPtr<IUnityObjectBase>)>();
	}

	public override string ToString()
	{
		string? name = (this as IHasNameString)?.NameString;
		return string.IsNullOrEmpty(name) ? GetType().Name : name;
	}

	public virtual void Reset() => throw MethodNotSupported();

	public virtual void CopyValues(IUnityAssetBase? source, PPtrConverter converter)
	{
	}

	public virtual JsonNode SerializeAllFields(IUnityAssetSerializer serializer, SerializationOptions options) => throw MethodNotSupported();

	public virtual JsonNode SerializeReleaseFields(IUnityAssetSerializer serializer, SerializationOptions options) => throw MethodNotSupported();

	public virtual JsonNode SerializeEditorFields(IUnityAssetSerializer serializer, SerializationOptions options) => throw MethodNotSupported();

	public virtual void Deserialize(JsonNode node, IUnityAssetDeserializer deserializer, DeserializationOptions options) => throw MethodNotSupported();

	private Exception MethodNotSupported([CallerMemberName] string? methodName = null)
	{
		return new NotSupportedException($"{methodName} is not supported for {GetType().FullName}");
	}
}
