using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using xml2rust.Data;

namespace xml2rust.Generators
{
	public class GroupedRegisterGenerator : RegisterGeneratorBase
	{
		private readonly List<IGrouping<string, Field>> _groups;

		public GroupedRegisterGenerator(PeripheralGenerator peripheralGenerator, Register register, List<IGrouping<string, Field>> groups) : base(peripheralGenerator, register)
		{
			_groups = groups;
		}

		public override void Generate(GeneratorOutput output)
		{
			output.AddComment(PeripheralGenerator.FileName, Register.Description.ToSingleLine());
			output.AddLine(PeripheralGenerator.FileName, "pub mod {0} {{", Register.Name.ToLower());

			foreach (IGrouping<string, Field> group in _groups)
			{
				List<Field> fields = group.ToList();
				Field firstField = fields[0];
				List<int> fieldOffsets = fields.Select(f => int.Parse(f.BitOffset)).ToList();
				fieldOffsets.Sort();
				int offset = fieldOffsets[0];
				int stepSize = fieldOffsets[1] - fieldOffsets[0];

				var access = Register.Access ?? firstField.Access;

				int bitWidth = int.Parse(firstField.BitWidth);
				string type;
				if (bitWidth == 1) type = "bool";
				else if (bitWidth <= 8) type = "u8";
				else if (bitWidth <= 16) type = "u16";
				else if (bitWidth <= 32) type = "u32";
				else type = "u64";

				switch (access)
				{
					case "write-only":
						GenerateSetter(group.Key.ToLower(), type, output, offset, stepSize, fields.Count, firstField);
						break;
					case "read-only":
						GenerateGetter(group.Key.ToLower(), type, output, offset, stepSize, fields.Count, firstField);
						break;
					case "read-write":
						GenerateSetter("set_" + group.Key.ToLower(), type, output, offset, stepSize, fields.Count, firstField);
						GenerateGetter("get_" + group.Key.ToLower(), type, output, offset, stepSize, fields.Count, firstField);
						break;
					default:
						throw new NotImplementedException();
				}
				
			}

			output.AddLine(PeripheralGenerator.FileName, "}}");
		}
		private string GetAddress()
		{
			return string.Format("({0}u32 + {1}u32) as *mut u32", PeripheralGenerator.Peripheral.BaseAddress, Register.AddressOffset);
		}

		private void GenerateGetter(string name, string type, GeneratorOutput output, int offset, int stepSize, int count, Field firstField)
		{
			var bitWidth = int.Parse(firstField.BitWidth);
			var mask = new string('1', bitWidth);

			output.AddComment(PeripheralGenerator.FileName, 1, "Get " + firstField.Description.ToSingleLine());
			output.AddLine(PeripheralGenerator.FileName, "\tpub fn {0}(index: u8) -> {1} {{", name, type);
			output.AddLine(PeripheralGenerator.FileName, "\t\tdebug_assert!(index < {0}, \"{1} out of range\");", count, name);
			output.AddLine(PeripheralGenerator.FileName, "\t\tlet value = unsafe {{ ::core::ptr::read_volatile({0}) }};", GetAddress());
			output.AddLine(PeripheralGenerator.FileName, "\t\tlet value = value & (0b{0} << ({1} + index * {2}));", mask, offset, stepSize);
			output.AddLine(PeripheralGenerator.FileName, "\t\t" + (type == "bool" ? "value > 0" : "value as " + type));
			output.AddLine(PeripheralGenerator.FileName, "\t}}");
		}

		private void GenerateSetter(string name, string type, GeneratorOutput output, int offset, int stepSize, int count, Field firstField)
		{
			var bitWidth = int.Parse(firstField.BitWidth);
			bool checkWidth = bitWidth != 1 && bitWidth != 8 && bitWidth != 16 && bitWidth != 32;
			var mask = new string('1', bitWidth);

			output.AddComment(PeripheralGenerator.FileName, 1, "Set " + firstField.Description.ToSingleLine());
			output.AddLine(PeripheralGenerator.FileName, "\tpub fn {0}(index: u8, value: {1}) {{", name, type);
			output.AddLine(PeripheralGenerator.FileName, "\t\tdebug_assert!(index < {0}, \"{1} out of range\");", count, name);
			if (checkWidth)
				output.AddLine(PeripheralGenerator.FileName, "\t\tdebug_assert!(value <= 0b{0}, \"{1} out of range\");", mask, name);

			output.AddLine(PeripheralGenerator.FileName, "\t\tlet value = value as u32;");
			output.AddLine(PeripheralGenerator.FileName, "\t\tlet value = value << ({0} + index * {1});", offset, stepSize);
			output.AddLine(PeripheralGenerator.FileName, "\t\tunsafe {{ ::core::ptr::write_volatile({0}, value) }};", GetAddress());
			output.AddLine(PeripheralGenerator.FileName, "\t}}");
		}
	}
}