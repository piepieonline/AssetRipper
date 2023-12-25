﻿using AssetRipper.Assets.Metadata;
using AssetRipper.Numerics;
using AssetRipper.SourceGenerated.Classes.ClassID_1;
using AssetRipper.SourceGenerated.Classes.ClassID_4;
using AssetRipper.SourceGenerated.Subclasses.PPtr_Transform;

namespace AssetRipper.SourceGenerated.Extensions
{
	public static class TransformExtensions
	{
		private const char PathSeparator = '/';

		public static Transformation ToTransformation(this ITransform transform)
		{
			return Transformation.Create(transform.LocalPosition_C4.CastToStruct(), transform.LocalRotation_C4.CastToStruct(), transform.LocalScale_C4.CastToStruct());
		}

		public static Transformation ToInverseTransformation(this ITransform transform)
		{
			return Transformation.CreateInverse(transform.LocalPosition_C4.CastToStruct(), transform.LocalRotation_C4.CastToStruct(), transform.LocalScale_C4.CastToStruct());
		}

		public static string GetRootPath(this ITransform transform)
		{
			string pre = string.Empty;
			if (!transform.Father_C4.IsNull())
			{
				pre = transform.Father_C4.GetAsset(transform.Collection).GetRootPath() + PathSeparator;
			}
			return pre + transform.GetGameObject().NameString;
		}

		/// <summary>
		/// Initialize an injected Transform with some sensible default values.
		/// </summary>
		/// <remarks>
		/// Since this Transform is assumed to have no <see cref="ITransform.Father_C4"/>, its <see cref="ITransform.RootOrder_C4"/> is zero.
		/// </remarks>
		/// <param name="transform"></param>
		public static void InitializeDefault(this ITransform transform)
		{
			transform.LocalPosition_C4.SetZero();
			transform.LocalRotation_C4.SetIdentity();
			transform.LocalScale_C4.SetOne();
			transform.RootOrder_C4 = 0;
			transform.LocalEulerAnglesHint_C4?.SetZero();
		}

		public static ITransform? FindChild(this ITransform transform, string path)
		{
			if (path.Length == 0)
			{
				return transform;
			}
			return transform.FindChild(path, 0);
		}

		private static ITransform? FindChild(this ITransform transform, string path, int startIndex)
		{
			int separatorIndex = path.IndexOf(PathSeparator, startIndex);
			string childName = separatorIndex == -1 ?
				path.Substring(startIndex, path.Length - startIndex) :
				path.Substring(startIndex, separatorIndex - startIndex);
			foreach (ITransform child in transform.GetChildren())
			{
				IGameObject childGO = child.GetGameObject();
				if (childGO.NameString == childName)
				{
					return separatorIndex == -1 ? child : child.FindChild(path, separatorIndex + 1);
				}
			}
			return default;
		}

		public static IEnumerable<ITransform> GetChildren(this ITransform transform)
		{
			foreach (IPPtr_Transform childPtr in transform.Children_C4)
			{
				yield return childPtr.GetAsset(transform.Collection);
			}
		}

		/// <summary>
		/// Find the sibling index (aka the root order) of the transform
		/// </summary>
		/// <param name="transform">The relevant transform</param>
		/// <returns>The sibling index of the transform</returns>
		/// <exception cref="Exception">if the transform cannot be found among the father's children</exception>
		public static int CalculateRootOrder(this ITransform transform)
		{
			ITransform? father = transform.Father_C4P;
			if (father is null)
			{
				return 0;
			}
			for (int i = 0; i < father.Children_C4P.Count; i++)
			{
				if (father.Children_C4P[i] == transform)
				{
					return i;
				}
			}
			throw new Exception("Transform hasn't been found among father's children");
		}
	}
}
