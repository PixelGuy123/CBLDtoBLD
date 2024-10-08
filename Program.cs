﻿using PlusLevelFormat;
using System.Diagnostics;

namespace CBLDtoBLD;

using Console = System.Console;

static class BLDConverter
{
	static void Main(string[] args)
	{		
		if (args.Length == 0)
		{
			Console.WriteLine("Please, input the path to the .CBLD file you want to convert.");
			while (args.Length == 0)
			{
				string? input = Console.ReadLine();
				if (!string.IsNullOrEmpty(input) && ValidFile(input))
					args = [input];
				else
				{
					Console.Clear();
					Console.WriteLine("Please, input the path to the .CBLD file you want to convert.");
				}
			}
		}
		Console.Clear();
		for (int i = 0; i < args.Length; i++)
		{
			string file = args[i];
			Console.WriteLine($"== Loading file: {Path.GetFileName(file)} ==\n");
			

			if (!ValidFile(file))
			{
				LogAtColor(ConsoleColor.Red, $"Invalid file detected ({file}). Please input a .cbld file next time.");
				continue;
			}
			file = Path.GetFullPath(file);
			
			Stopwatch w = Stopwatch.StartNew();
			try
			{
				Console.WriteLine("Reading level..");
				Level level;
				using (var reader = new BinaryReader(File.OpenRead(file)))
					level = LevelExtensions.ReadLevel(reader);


				Console.WriteLine("Converting level...");
				var dir = Path.GetDirectoryName(file);

				if (string.IsNullOrEmpty(dir))
					throw new DirectoryNotFoundException("Directory for the provided path has not been found.");

				string fname = Path.Combine(dir, Path.GetFileNameWithoutExtension(file) + ".bld");

				using (var writer = new BinaryWriter(File.OpenWrite(fname)))
					level.ConvertToEditor().SaveIntoStream(writer);


				w.Stop();
				LogAtColor(ConsoleColor.Green, $"CBLD file converted as {Path.GetFileName(fname)}");
				Console.WriteLine("Time taken: " + w.ElapsedMilliseconds + "ms");
			}
			catch (Exception e)
			{
				Console.BackgroundColor = ConsoleColor.Red;
				Console.WriteLine($"Failed to load file ({file}). Please, make sure the file you\'re using is not corrupted or contain invalid data.");
				Console.WriteLine($"Printing exception...\n{e}");
			}
		}

		Console.WriteLine("====\nPress any key to quit...");
		Console.Read();
	}

	static bool ValidFile(string path) =>
		File.Exists(path) && Path.GetExtension(path) == ".cbld";

	static void LogAtColor(ConsoleColor color, string content)
	{
		var c = Console.BackgroundColor;
		Console.BackgroundColor = color;
		Console.WriteLine(content);
		Console.BackgroundColor = c;
	}
}