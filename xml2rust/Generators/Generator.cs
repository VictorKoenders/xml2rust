using System.Linq;
using xml2rust.Data;

namespace xml2rust.Generators
{
	public class Generator : IGenerator
	{
		private readonly DeviceGenerator DeviceGenerator;

		public Generator(Parser parser)
		{
			foreach (Peripheral peripheral in parser.Device.Peripherals.Peripheral)
			{
				if (!string.IsNullOrEmpty(peripheral.DerivedFrom))
				{
					Derive(peripheral, parser.Device.Peripherals.Peripheral.First(p => p.Name == peripheral.DerivedFrom));
				}
			}

			DeviceGenerator = new DeviceGenerator(parser);
		}

		private static void Derive(Peripheral peripheral, Peripheral parent)
		{
			peripheral.AddressBlock = peripheral.AddressBlock ?? parent.AddressBlock;
			peripheral.BaseAddress = peripheral.BaseAddress ?? parent.BaseAddress;
			peripheral.DerivedFrom = peripheral.DerivedFrom ?? parent.DerivedFrom;
			peripheral.Description = peripheral.Description ?? parent.Description;
			peripheral.GroupName = peripheral.GroupName ?? parent.GroupName;
			peripheral.Interrupt = peripheral.Interrupt ?? parent.Interrupt;
			peripheral.Name = peripheral.Name ?? parent.Name;
			peripheral.Registers = peripheral.Registers ?? parent.Registers;
		}

		public void Generate(GeneratorOutput output)
		{
			DeviceGenerator.Generate(output);
		}
	}
}