using xml2rust.Data;

namespace xml2rust.Generators
{
	public class CachedStateRegisterGenerator : RegisterGeneratorBase
	{
		public CachedStateRegisterGenerator(PeripheralGenerator peripheralGenerator, Register register) : base(peripheralGenerator, register)
		{
		}

		public override void Generate(GeneratorOutput output)
		{
			Field firstField = Register.Fields.Field[0];

			int bitWidth = int.Parse(firstField.BitWidth);
			string type;
			if (bitWidth == 1) type = "bool";
			else if (bitWidth <= 8) type = "u8";
			else if (bitWidth <= 16) type = "u16";
			else if (bitWidth <= 32) type = "u32";
			else type = "u64";

			if (Register.Name == "MODER")
			{
				type = "::ModerType";
			}

			output.AddComment(PeripheralGenerator.FileName, Register.Description.ToSingleLine());
			output.AddLine(PeripheralGenerator.FileName, "pub mod {0} {{", Register.Name.ToLower());

			// Create readonly cache struct
			output.AddLine(PeripheralGenerator.FileName, "\tpub struct ReadonlyCache {{");

			foreach (Field field in Register.Fields.Field)
			{
				output.AddComment(PeripheralGenerator.FileName, 2, field.Description.ToSingleLine());
				output.AddLine(PeripheralGenerator.FileName, "\t\tpub {0}: {1},", field.Name.ToLower(), type);
			}

			output.AddLine(PeripheralGenerator.FileName, "\t}}");

			// Create cache struct
			output.AddLine(PeripheralGenerator.FileName, "\tpub struct Cache {{");

			foreach (Field field in Register.Fields.Field)
			{
				output.AddComment(PeripheralGenerator.FileName, 2, field.Description.ToSingleLine());
				output.AddLine(PeripheralGenerator.FileName, "\t\tpub {0}: {1},", field.Name.ToLower(), type);
			}

			output.AddLine(PeripheralGenerator.FileName, "\t}}");

			// Create readonly getter
			output.AddLine(PeripheralGenerator.FileName, "\tpub fn load() -> ReadonlyCache {{");
			output.AddLine(PeripheralGenerator.FileName, "\t\tlet value = unsafe {{ ::core::ptr::read_volatile(({0}u32 + {1}u32) as *mut u32) }};", PeripheralGenerator.Peripheral.BaseAddress, Register.AddressOffset);
			output.AddLine(PeripheralGenerator.FileName, "\t\tReadonlyCache {{", Register.Name.ToCamelCase());
			foreach (Field field in Register.Fields.Field)
			{
				string mask = "0b" + new string('1', bitWidth);

				string comparison = type == "bool" ? "> 0" : type == "::ModerType" ? ".into()" : "as " + type;
				output.AddLine(PeripheralGenerator.FileName, "\t\t\t{0}: ((value >> {2}) & {1}) {3},", field.Name.ToLower(), mask, field.BitOffset, comparison);
			}
			output.AddLine(PeripheralGenerator.FileName, "\t\t}}");
			output.AddLine(PeripheralGenerator.FileName, "\t}}", Register.Name.ToUpper());

			// Create getter
			output.AddLine(PeripheralGenerator.FileName, "\tpub fn modify() -> Cache {{");
			output.AddLine(PeripheralGenerator.FileName, "\t\tlet value = unsafe {{ ::core::ptr::read_volatile(({0}u32 + {1}u32) as *mut u32) }};", PeripheralGenerator.Peripheral.BaseAddress, Register.AddressOffset);
			output.AddLine(PeripheralGenerator.FileName, "\t\tCache {{", Register.Name.ToCamelCase());
			foreach (Field field in Register.Fields.Field)
			{
				string mask = "0b" + new string('1', bitWidth);

				string comparison = type == "bool" ? "> 0" : type == "::ModerType" ? ".into()" : "as " + type;
				output.AddLine(PeripheralGenerator.FileName, "\t\t\t{0}: ((value >> {2}) & {1}) {3},", field.Name.ToLower(), mask, field.BitOffset, comparison);
			}
			output.AddLine(PeripheralGenerator.FileName, "\t\t}}");
			output.AddLine(PeripheralGenerator.FileName, "\t}}", Register.Name.ToUpper());

			// Create save function on struct
			output.AddLine(PeripheralGenerator.FileName, "\timpl Cache {{");
			output.AddLine(PeripheralGenerator.FileName, "\t\tpub fn save(self) {{");
			output.AddLine(PeripheralGenerator.FileName, "\t\t\t// This will call Cache::drop defined below", Register.Name.ToCamelCase());
			output.AddLine(PeripheralGenerator.FileName, "\t\t}}");
			output.AddLine(PeripheralGenerator.FileName, "\t}}");

			// Create drop function on struct
			output.AddLine(PeripheralGenerator.FileName, "\timpl ::core::ops::Drop for Cache {{");
			output.AddLine(PeripheralGenerator.FileName, "\t\tfn drop(&mut self) {{");
			output.AddLine(PeripheralGenerator.FileName, "\t\t\tlet value = 0");
			foreach (Field field in Register.Fields.Field)
			{
				output.AddLine(PeripheralGenerator.FileName, "\t\t\t\t| ((self.{0} as u32) << {1})", field.Name.ToLower(), field.BitOffset);
			}
			output.AddLine(PeripheralGenerator.FileName, "\t\t\t;", Register.Name.ToUpper());
			output.AddLine(PeripheralGenerator.FileName, "\t\t\tunsafe {{ ::core::ptr::write_volatile(({0}u32 + {1}u32) as *mut u32, value) }};", PeripheralGenerator.Peripheral.BaseAddress, Register.AddressOffset);
			
			output.AddLine(PeripheralGenerator.FileName, "\t\t}}", Register.Name.ToUpper());
			output.AddLine(PeripheralGenerator.FileName, "\t}}", Register.Name.ToUpper());
			output.AddLine(PeripheralGenerator.FileName, "}}", Register.Name.ToUpper());

		}
	}
}