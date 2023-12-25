﻿using AsmResolver.DotNet;
using AssetRipper.Import.Logging;
using AssetRipper.Import.Structure.Assembly.Managers;
using AssetRipper.IO.Files.Utils;

namespace AssetRipper.Export.UnityProjects.Scripts
{
	public class DllPostExporter : IPostExporter
	{
		public void DoPostExport(Ripper ripper)
		{
			string outputDirectory = Path.Combine(ripper.Settings.AuxiliaryFilesPath, "GameAssemblies");

			Logger.Info(LogCategory.Export, "Saving game assemblies...");
			IAssemblyManager assemblyManager = ripper.GameStructure.AssemblyManager;
			AssemblyDefinition[] assemblies = assemblyManager.GetAssemblies().ToArray();
			if (assemblies.Length != 0)
			{
				Directory.CreateDirectory(outputDirectory);
				foreach (AssemblyDefinition assembly in assemblies)
				{
					string filepath = Path.Combine(outputDirectory, FilenameUtils.AddAssemblyFileExtension(assembly.Name!));
					assemblyManager.SaveAssembly(assembly, filepath);
				}
			}
		}
	}
}
