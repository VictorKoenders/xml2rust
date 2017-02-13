using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace xml2rust.Generators
{
	public class GeneratorOutput
	{
		public readonly Dictionary<string, string> Files = new Dictionary<string, string>();

		public void Add(string file, string content, params object[] args)
		{
			if (!Files.ContainsKey(file))
			{
				Files.Add(file, "");
			}
			content = content.Replace("\t", "    ");
			Files[file] += string.Format(content, args);
		}
		public void AddLine(string file, string content, params object[] args)
		{
			if (!Files.ContainsKey(file))
			{
				Files.Add(file, "");
			}
			content = content.Replace("\t", "    ");
			Files[file] += string.Format(content, args) + Environment.NewLine;
		}

		public void AddComment(string file, int indent, string content, params object[] args)
		{
			AddLine(file, new string('\t', indent) + "/// " + content, args);
		}

		public void AddComment(string file, string content, params object[] args)
		{
			AddLine(file, "/// " + content, args);
		}

		public List<string> GetDirectories()
		{
			return Files.Select(f => Path.GetDirectoryName(f.Key)).Distinct().ToList();
		}

		public List<string> GetDirectoriesInDirectory(string dir)
		{
			if (dir == "") dir = ".";
			string baseDir = Path.GetFullPath(dir);
			List<string> foundDirectories = new List<string>();
			foreach (KeyValuePair<string, string> file in Files)
			{
				string directory = Path.GetDirectoryName(file.Key);
				if (directory != null && Path.GetFullPath(Path.Combine(directory, "..")) == baseDir)
				{
					if (!foundDirectories.Contains(directory)) foundDirectories.Add(directory);
				}
			}
			return foundDirectories;
		}
		public List<string> GetFilesInDirectory(string dir)
		{
			return (
				from f in Files
				let file = f.Key
				let path = Path.GetDirectoryName(file)
				where path == dir
				select file
			).ToList();
		}
	}
}