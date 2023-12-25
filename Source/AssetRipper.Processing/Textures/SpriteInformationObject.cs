﻿using AssetRipper.Assets;
using AssetRipper.Assets.Interfaces;
using AssetRipper.Assets.Metadata;
using AssetRipper.Primitives;
using AssetRipper.SourceGenerated.Classes.ClassID_213;
using AssetRipper.SourceGenerated.Classes.ClassID_28;
using AssetRipper.SourceGenerated.Classes.ClassID_687078895;
using AssetRipper.SourceGenerated.Interfaces;
using System.Diagnostics;

namespace AssetRipper.Processing.Textures;

public sealed class SpriteInformationObject : UnityObjectBase, IHasName
{
	public SpriteInformationObject(AssetInfo assetInfo, ITexture2D texture) : base(assetInfo)
	{
		Texture = texture;
	}

	public ITexture2D Texture { get; }
	public IReadOnlyDictionary<ISprite, ISpriteAtlas?> Sprites => dictionary;
	private readonly Dictionary<ISprite, ISpriteAtlas?> dictionary = new();
	string IHasNameString.NameString
	{
		get => Texture.NameString;
		set { }
	}
	Utf8String IHasName.Name
	{
		get => Texture.Name;
		set { }
	}

	internal void AddToDictionary(ISprite sprite, ISpriteAtlas? atlas)
	{
		if (dictionary.TryGetValue(sprite, out ISpriteAtlas? mappedAtlas))
		{
			if (mappedAtlas is null)
			{
				dictionary[sprite] = atlas;
			}
			else if (atlas is not null && atlas != mappedAtlas)
			{
				throw new Exception($"{nameof(atlas)} is not the same as {nameof(mappedAtlas)}");
			}
		}
		else
		{
			dictionary.Add(sprite, atlas);
		}
	}

	internal void SetMainAsset()
	{
		Debug.Assert(Texture.MainAsset is null);
		MainAsset = this;
		Texture.MainAsset = this;
		foreach ((ISprite sprite, ISpriteAtlas? atlas) in dictionary)
		{
			sprite.MainAsset = this;
			if (atlas is not null)
			{
				atlas.MainAsset = this;
			}
		}
	}
}
