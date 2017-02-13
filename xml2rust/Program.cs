using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using xml2rust.Generators;

namespace xml2rust
{
	internal static class Program
	{
		public static void Main()
		{
			Console.WriteLine("Parsing...");
			Parser parser = new Parser("Resources/STM32F303x.svd");
			Generator generator = new Generator(parser);
			GeneratorOutput output = new GeneratorOutput();
			generator.Generate(output);

			Console.WriteLine("Generating...");
			WriteGeneratedFiles(output.Files);
			WriteResourceFiles();
			Console.WriteLine("Compiling...");
			TestCompile();
			Console.WriteLine("Press enter to continue ...");
			Console.ReadLine();
		}

		private static void TestCompile()
		{
			Process process = new Process();
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.RedirectStandardOutput = true;

			process.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
			process.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data);

			process.StartInfo.UseShellExecute = false;
			process.StartInfo.EnvironmentVariables.Add("RUST_BACKTRACE", "1");
			process.StartInfo.WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "peripheral");
			process.StartInfo.FileName = "xargo.exe";
			process.StartInfo.Arguments = "build --verbose --target thumbv7em-none-eabihf";
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.Start();
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
			
			process.WaitForExit();
			
		}

		private static void WriteResourceFiles()
		{
			WriteResourceToFile("xml2rust.Resources.Cargo.toml", "peripheral/Cargo.toml");
		}

		private static void WriteResourceToFile(string resourceName, string file)
		{
			string path = Path.GetDirectoryName(Path.GetFullPath(file));
			if (path != null && !Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			if (File.Exists(file)) File.Delete(file);
			using (Stream inputStream = Assembly.GetEntryAssembly().GetManifestResourceStream(resourceName))
			using (FileStream outputStream = File.Create(file))
			{
				inputStream?.CopyTo(outputStream);
			}
		}

		private static void WriteGeneratedFiles(Dictionary<string, string> files)
		{
			foreach (KeyValuePair<string, string> file in files)
			{
				string filename = Path.Combine("peripheral", "src", file.Key);
				string path = Path.GetDirectoryName(Path.GetFullPath(filename));
				if (path != null && !Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}

				if (File.Exists(filename)) File.Delete(filename);
				File.WriteAllText(filename, file.Value);
			}
		}
	}
}
