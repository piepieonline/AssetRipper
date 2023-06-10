﻿using AssetRipper.Assets;
using AssetRipper.Assets.Bundles;
using AssetRipper.Assets.Cloning;
using AssetRipper.Assets.Collections;
using AssetRipper.Assets.Metadata;
using AssetRipper.SourceGenerated.Classes.ClassID_213;
using AssetRipper.SourceGenerated.Classes.ClassID_28;
using AssetRipper.SourceGenerated.Classes.ClassID_687078895;
using AssetRipper.SourceGenerated.Extensions;
using AssetRipper.SourceGenerated.Subclasses.SecondarySpriteTexture;
using AssetRipper.SourceGenerated.Subclasses.SpriteAtlasData;
using AssetRipper.SourceGenerated.Subclasses.SpriteRenderData;
using System.Drawing;
using System.Numerics;

namespace AssetRipper.Processing.Textures
{
	public sealed class SpriteProcessor : IAssetProcessor
	{
		public void Process(GameBundle gameBundle, UnityVersion projectVersion)
		{
			ObjectFactory factory = new ObjectFactory(gameBundle, projectVersion);
			foreach (IUnityObjectBase asset in gameBundle.FetchAssets())
			{
				if (asset is ITexture2D texture)
				{
					if (texture.MainAsset is null)
					{
						factory.GetOrCreate(texture);
					}
				}
				else if (asset is ISprite sprite)
				{
					ITexture2D? spriteTexture = sprite.TryGetTexture();
					if (spriteTexture is not null)
					{
						SpriteInformationObject spriteInformationObject = factory.GetOrCreate(spriteTexture);
						ISpriteAtlas? atlas = sprite.SpriteAtlas_C213P;
						spriteInformationObject.AddToDictionary(sprite, atlas);
					}

					ProcessSprite(sprite);
				}
				else if (asset is ISpriteAtlas atlas && atlas.RenderDataMap_C687078895.Count > 0)
				{
					foreach (ISprite packedSprite in atlas.PackedSprites_C687078895P.WhereNotNull())
					{
						if (TryGetPackedSpriteTexture(atlas, packedSprite, out ITexture2D? spriteTexture))
						{
							SpriteInformationObject spriteInformationObject = factory.GetOrCreate(spriteTexture);
							spriteInformationObject.AddToDictionary(packedSprite, atlas);
						}
					}
				}
			}
			foreach (SpriteInformationObject asset in factory.Assets)
			{
				asset.SetMainAsset();
			}
		}

		private static bool TryGetPackedSpriteTexture(ISpriteAtlas atlas, ISprite packedSprite, [NotNullWhen(true)] out ITexture2D? spriteTexture)
		{
			if (packedSprite.Has_RenderDataKey_C213() && atlas.RenderDataMap_C687078895.TryGetValue(packedSprite.RenderDataKey_C213, out ISpriteAtlasData? atlasData))
			{
				spriteTexture = atlasData.Texture.TryGetAsset(atlas.Collection);
			}
			else
			{
				spriteTexture = null;
			}
			return spriteTexture is not null;
		}

		private static void ProcessSprite(ISprite sprite)
		{
			// SpriteAtlas is a new feature since Unity 2017.
			// For older versions of Unity, SpriteAtlas doesn't exist,
			// and the correct metadata of Sprite is stored in the m_RD field.
			// Otherwise, if a SpriteAtlas reference is serialized into this sprite,
			// we must recover the m_RD field of the sprite from the SpriteAtlas.
			ISpriteAtlas? atlas = sprite.SpriteAtlas_C213P;
			if (atlas is not null && sprite.Has_SpriteAtlas_C213())
			{
				if (sprite.Has_RenderDataKey_C213() &&
					atlas.RenderDataMap_C687078895.TryGetValue(sprite.RenderDataKey_C213, out ISpriteAtlasData? spriteData))
				{
					PPtrConverter converter = new PPtrConverter(atlas, sprite);
					ISpriteRenderData m_RD = sprite.RD_C213;
					m_RD.Texture.CopyValues(spriteData.Texture, converter);
					if (m_RD.Has_AlphaTexture())
					{
						m_RD.AlphaTexture.CopyValues(spriteData.AlphaTexture, converter);
					}
					m_RD.TextureRect.CopyValues(spriteData.TextureRect);
					if (m_RD.Has_AtlasRectOffset() && spriteData.Has_AtlasRectOffset())
					{
						m_RD.AtlasRectOffset.CopyValues(spriteData.AtlasRectOffset);
					}
					m_RD.SettingsRaw = spriteData.SettingsRaw;
					if (m_RD.Has_UvTransform())
					{
						m_RD.UvTransform.CopyValues(spriteData.UvTransform);
					}
					m_RD.DownscaleMultiplier = spriteData.DownscaleMultiplier;
					if (m_RD.Has_SecondaryTextures() && spriteData.Has_SecondaryTextures())
					{
						m_RD.SecondaryTextures.Clear();
						foreach (SecondarySpriteTexture spt in spriteData.SecondaryTextures)
						{
							SecondarySpriteTexture newSpt = m_RD.SecondaryTextures.AddNew();
							newSpt.Name.CopyValues(spt.Name);
							newSpt.Texture.CopyValues(spt.Texture, converter);
						}
					}
				}

				// Must clear the reference to SpriteAtlas, since Unity Editor will crash trying to pack an already-packed sprite otherwise.
				sprite.SpriteAtlas_C213P = null;
				sprite.AtlasTags_C213?.Clear();
			}

			// Some sprite properties must be recalculated with regard to SpriteAtlas. See the comments inside the following method.
			sprite.GetSpriteCoordinatesInAtlas(atlas, out RectangleF rect, out Vector2 pivot, out Vector4 border);
			sprite.Rect_C213.CopyValues(rect);
			sprite.Pivot_C213?.CopyValues(pivot);
			sprite.Border_C213?.CopyValues(border);

			// Calculate and overwrite Offset. It is the offset in pixels of the pivot to the center of Rect.
			sprite.Offset_C213.X = (pivot.X - 0.5f) * rect.Width;
			sprite.Offset_C213.Y = (pivot.Y - 0.5f) * rect.Height;

			// Calculate and overwrite TextureRectOffset. It is the offset in pixels of m_RD.TextureRect to Rect.
			sprite.RD_C213.TextureRectOffset.X = sprite.RD_C213.TextureRect.X - rect.X;
			sprite.RD_C213.TextureRectOffset.Y = sprite.RD_C213.TextureRect.Y - rect.Y;
		}

		private readonly struct ObjectFactory
		{
			private readonly ProcessedAssetCollection processedCollection;
			private readonly Dictionary<ITexture2D, SpriteInformationObject> dictionary = new();

			public IEnumerable<SpriteInformationObject> Assets => dictionary.Values;

			public ObjectFactory(GameBundle gameBundle, UnityVersion projectVersion)
			{
				processedCollection = gameBundle.AddNewProcessedCollection("Sprite Data Storage", projectVersion);
			}

			public SpriteInformationObject GetOrCreate(ITexture2D texture)
			{
				if (!dictionary.TryGetValue(texture, out SpriteInformationObject? result))
				{
					result = MakeSpriteInformationObject(texture);
					dictionary.Add(texture, result);
				}
				return result;
			}

			SpriteInformationObject MakeSpriteInformationObject(ITexture2D texture)
			{
				return processedCollection.CreateAsset(-1, texture, static (assetInfo, texture) => new SpriteInformationObject(assetInfo, texture));
			}
		}
	}
}
