using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using xml2rust.Data;

namespace xml2rust.Generators
{
	public class PeripheralGenerator : IGenerator
	{
		public readonly Peripheral Peripheral;
		private readonly List<RegisterGeneratorBase> RegisterGenerators;
		private readonly List<InterruptGenerator> InterruptGenerators;

		public string FileName;

		public PeripheralGenerator(Peripheral peripheral)
		{
			Peripheral = peripheral;
			RegisterGenerators = peripheral.Registers?.Register.Select(r => RegisterGeneratorBase.GetGenerator(this, r)).ToList() ?? new List<RegisterGeneratorBase>();
			InterruptGenerators = peripheral.Interrupt.Select(r => new InterruptGenerator(this, r)).ToList();
		}

		public void Generate(GeneratorOutput output)
		{
			FileName = Peripheral.Name;
			if (Peripheral.GroupName != null && Peripheral.GroupName != Peripheral.Name)
			{
				string dir = Peripheral.GroupName;
				if (FileName.StartsWith(Peripheral.GroupName))
				{
					string remainder = FileName.Substring(Peripheral.GroupName.Length);
					if (remainder.IsValidFilename())
					{
						FileName = remainder;
					}
				}
				FileName = dir + "/" + FileName;
			}
			FileName = FileName.ToLower() + ".rs";

			output.Add(FileName, "");

			foreach (RegisterGeneratorBase register in RegisterGenerators) register.Generate(output);
			foreach (InterruptGenerator interrupt in InterruptGenerators) interrupt.Generate(output);

			using (MemoryStream memoryStream = new MemoryStream())
			{
				new XmlSerializer(typeof(Peripheral)).Serialize(memoryStream, Peripheral);

				memoryStream.Seek(0, SeekOrigin.Begin);
				output.AddLine(FileName, "/*");
				output.AddLine(FileName, new StreamReader(memoryStream).ReadToEnd());
				output.AddLine(FileName, "*/");
			}
		}
	}
}