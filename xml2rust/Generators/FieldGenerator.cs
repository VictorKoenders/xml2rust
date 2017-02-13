using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using xml2rust.Data;

namespace xml2rust.Generators
{
	public class FieldGenerator : IGenerator
	{
		private readonly SimpleRegisterGenerator _registerGenerator;
		private readonly Field _field;

		public FieldGenerator(SimpleRegisterGenerator registerGenerator, Field field)
		{
			_registerGenerator = registerGenerator;
			_field = field;
		}

		public void Generate(GeneratorOutput output)
		{
			if (_registerGenerator.Register.Access != null && _field.Access != null)
			{
				Debug.Assert(
					_registerGenerator.Register.Access == _field.Access,
					"Register address is not the same as field access");
			}
			string access = _registerGenerator.Register.Access ?? _field.Access;
			output.AddComment(_registerGenerator.PeripheralGenerator.FileName, 1, _field.Description.ToSingleLine());
			output.AddComment(_registerGenerator.PeripheralGenerator.FileName, 1, "Access: {0}, Width: {1}, Offset: {2}", access, _field.BitWidth, _field.BitOffset);
			//output.AddLine(_registerGenerator.PeripheralGenerator.FileName, "\tpub const {0}_OFFSET: u32 = {1};", _field.Name.ToUpper(), _field.BitOffset);

			int bitWidth = int.Parse(_field.BitWidth);
			string type;
			if (bitWidth == 1) type = "bool";
			else if (bitWidth <= 8) type = "u8";
			else if (bitWidth <= 16) type = "u16";
			else if (bitWidth <= 32) type = "u32";
			else type = "u64";

			switch (access)
			{
				case "write-only":
					GenerateSetter(_field.Name.ToLower(), type, output);
					break;
				case "read-only":
					GenerateGetter(_field.Name.ToLower(), type, output);
					break;
				case "read-write":
					GenerateSetter("set_" + _field.Name.ToLower(), type, output);
					GenerateGetter("get_" + _field.Name.ToLower(), type, output);
					break;
				default:
					throw new NotImplementedException();
			}
		}

		private string GetAddress()
		{
			return string.Format("({0}u32 + {1}u32) as *mut u32", _registerGenerator.PeripheralGenerator.Peripheral.BaseAddress, _registerGenerator.Register.AddressOffset);
		}

		private void GenerateSetter(string name, string type, GeneratorOutput output)
		{
			var bitWidth = int.Parse(_field.BitWidth);
			bool checkWidth = bitWidth != 1 && bitWidth != 8 && bitWidth != 16 && bitWidth != 32;
			var mask = new string('1', bitWidth);

			output.AddComment(_registerGenerator.PeripheralGenerator.FileName, 1, "Set " + _field.Description.ToSingleLine());
			output.AddLine(_registerGenerator.PeripheralGenerator.FileName, "\tpub fn {0}(value: {1}) {{", name, type);
			if (checkWidth)
				output.AddLine(_registerGenerator.PeripheralGenerator.FileName, "\t\tdebug_assert!(value <= 0b{0}, \"{1} out of range\");", mask, name);

			output.AddLine(_registerGenerator.PeripheralGenerator.FileName, "\t\tlet value = value as u32;");
			if(_field.BitOffset != "0")
				output.AddLine(_registerGenerator.PeripheralGenerator.FileName, "\t\tlet value = value << {0};", _field.BitOffset);

			output.AddLine(_registerGenerator.PeripheralGenerator.FileName, "\t\tunsafe {{ ::core::ptr::write_volatile({0}, value) }};", GetAddress());
			output.AddLine(_registerGenerator.PeripheralGenerator.FileName, "\t}}");
		}
		private void GenerateGetter(string name, string type, GeneratorOutput output)
		{
			var bitWidth = int.Parse(_field.BitWidth);
			var mask = new string('1', bitWidth);

			if (name == "fn") name = _registerGenerator.Register.Name.ToLower() + "_fn";

			output.AddComment(_registerGenerator.PeripheralGenerator.FileName, 1, "Get " + _field.Description.ToSingleLine());
			output.AddLine(_registerGenerator.PeripheralGenerator.FileName, "\tpub fn {0}() -> {1} {{", name, type);
			output.AddLine(_registerGenerator.PeripheralGenerator.FileName, "\t\tlet value = unsafe {{ ::core::ptr::read_volatile({0}) }};", GetAddress());
			output.AddLine(_registerGenerator.PeripheralGenerator.FileName, "\t\tlet value = value & (0b{0} << {1});", mask, _field.BitOffset);
			output.AddLine(_registerGenerator.PeripheralGenerator.FileName, "\t\t" + (type == "bool" ? "value > 0" : "value as " + type));
			output.AddLine(_registerGenerator.PeripheralGenerator.FileName, "\t}}");
		}
	}
}