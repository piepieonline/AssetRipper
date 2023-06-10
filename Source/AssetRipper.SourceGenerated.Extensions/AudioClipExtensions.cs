﻿using AssetRipper.SourceGenerated.Classes.ClassID_83;
using AssetRipper.SourceGenerated.Enums;
using AssetRipper.SourceGenerated.NativeEnums.Fmod;

namespace AssetRipper.SourceGenerated.Extensions
{
	public static class AudioClipExtensions
	{
		public static byte[] GetAudioData(this IAudioClip audioClip)
		{
			if (audioClip.Has_AudioData_C83() && audioClip.AudioData_C83.Length > 0)
			{
				return audioClip.AudioData_C83;
			}
			else if (audioClip.Has_Resource_C83())
			{
				return audioClip.Resource_C83.GetContent(audioClip.Collection) ?? Array.Empty<byte>();
			}
			//else if (audioClip.StreamingInfo_C83 != null && audioClip.LoadType_C83 == (int)Classes.AudioClip.AudioClipLoadType.Streaming)
			//{
			//	return audioClip.StreamingInfo_C83.GetContent(audioClip.SerializedFile) ?? Array.Empty<byte>();
			//}
			else
			{
				return Array.Empty<byte>();
			}
		}

		public static bool CheckAssetIntegrity(this IAudioClip audioClip)
		{
			if (audioClip.Has_AudioData_C83() && audioClip.AudioData_C83.Length > 0)
			{
				return true;
			}
			else if (audioClip.Resource_C83 != null)
			{
				return audioClip.Resource_C83.CheckIntegrity(audioClip.Collection);
			}
			//else if (audioClip.StreamingInfo != null && audioClip.LoadType_C83 == (int)Classes.AudioClip.AudioClipLoadType.Streaming)
			//{
			//	return audioClip.StreamingInfo.CheckIntegrity(audioClip.SerializedFile);
			//}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// Only present when <see cref="IAudioClip.Has_Format_C83"/> is true.
		/// </summary>
		public static FmodSoundFormat GetSoundFormat(this IAudioClip audioClip) => (FmodSoundFormat)audioClip.Format_C83;

		/// <summary>
		/// Only present when <see cref="IAudioClip.Has_Type_C83"/> is true.
		/// </summary>
		public static FmodSoundType GetSoundType(this IAudioClip audioClip) => (FmodSoundType)audioClip.Type_C83;
	}
}
