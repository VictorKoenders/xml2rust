using System.Collections.Generic;
using System.IO;
using System.Linq;
using xml2rust.Data;

namespace xml2rust.Generators
{
	public class DeviceGenerator : IGenerator
	{
		private readonly Device _device;
		private readonly List<PeripheralGenerator> PeripheralGenerators;

		public DeviceGenerator(Parser parser)
		{
			_device = parser.Device;
			PeripheralGenerators = _device.Peripherals.Peripheral.Select(p => new PeripheralGenerator(p)).ToList();
		}

		public void Generate(GeneratorOutput output)
		{
			foreach (PeripheralGenerator generator in PeripheralGenerators)
			{
				generator.Generate(output);
			}
			output.AddLine("lib.rs", "#![no_std]");
			//output.AddLine("lib.rs", "#![allow(non_snake_case, non_camel_case_types)]");
			//output.AddLine("lib.rs", "#![feature(asm)]");
			output.AddLine("lib.rs", "#![deny(warnings)]");

			output.AddLine("lib.rs", "#[derive(Copy, Clone)]");
			output.AddLine("lib.rs", "pub enum ModerType {{");
			output.AddLine("lib.rs", "\tInputMode = 0b00,");
			output.AddLine("lib.rs", "\tOutputMode = 0b01,");
			output.AddLine("lib.rs", "\tAlternateMode = 0b10,");
			output.AddLine("lib.rs", "\tAnalogMode = 0b11,");
			output.AddLine("lib.rs", "}}");
			output.AddLine("lib.rs", "impl ::core::convert::From<u32> for ModerType {{");
			output.AddLine("lib.rs", "\tfn from(val: u32) -> ModerType {{");
			output.AddLine("lib.rs", "\t\tmatch val {{");
			output.AddLine("lib.rs", "\t\t\t0b00 => ModerType::InputMode,");
			output.AddLine("lib.rs", "\t\t\t0b01 => ModerType::OutputMode,");
			output.AddLine("lib.rs", "\t\t\t0b10 => ModerType::AlternateMode,");
			output.AddLine("lib.rs", "\t\t\t0b11 => ModerType::AnalogMode,");
			output.AddLine("lib.rs", "\t\t\tx => panic!(\"ModerType::From out of range: {{}}\", x)");
			output.AddLine("lib.rs", "\t\t}}");
			output.AddLine("lib.rs", "\t}}");
			output.AddLine("lib.rs", "}}");


			output.AddLine("lib.rs", "");
			output.AddLine("lib.rs", "#[no_mangle]");
			output.AddLine("lib.rs", "#[export_name = \"__aeabi_memcpy\"]");
			output.AddLine("lib.rs", "pub unsafe extern fn memcpy(dest: *mut u8, src: *const u8, n: usize) -> *mut u8 {{");
			output.AddLine("lib.rs", "\tlet mut i = 0;");
			output.AddLine("lib.rs", "\twhile i < n {{");
			output.AddLine("lib.rs", "\t\t*dest.offset(i as isize) = *src.offset(i as isize);");
			output.AddLine("lib.rs", "\t\ti += 1;");
			output.AddLine("lib.rs", "\t}}");
			output.AddLine("lib.rs", "\treturn dest");
			output.AddLine("lib.rs", "}}");

			output.AddLine("lib.rs", "");
			output.AddLine("lib.rs", "#[no_mangle]");
			output.AddLine("lib.rs", "#[export_name = \"__aeabi_memclr4\"]");
			output.AddLine("lib.rs", "pub unsafe extern fn memclr4(dest: *mut u8, n: usize) -> *mut u8 {{");
			output.AddLine("lib.rs", "\tlet mut i = 0;");
			output.AddLine("lib.rs", "\twhile i < n {{");
			output.AddLine("lib.rs", "\t\t*dest.offset(i as isize) = 0;");
			output.AddLine("lib.rs", "\t\ti += 1;");
			output.AddLine("lib.rs", "\t}}");
			output.AddLine("lib.rs", "\treturn dest");
			output.AddLine("lib.rs", "}}");

			output.AddComment("lib.rs", _device.Name.ToSingleLine());

			foreach (string directory in output.GetDirectories())
			{
				IEnumerable<string> dirs = output.GetDirectoriesInDirectory(directory).Concat(output.GetFilesInDirectory(directory));

				string libPath = Path.Combine(directory, directory == "" ? "lib.rs" : "mod.rs");
				foreach (string file in dirs.Where(f => f != libPath))
				{
					string filename = Path.GetFileNameWithoutExtension(file);
					if (!string.IsNullOrEmpty(filename))
					{
						output.AddLine(libPath, "pub mod {0};", filename);
					}
				}
			}

		}
	}
}