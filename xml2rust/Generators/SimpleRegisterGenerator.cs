using System.Collections.Generic;
using System.Linq;
using xml2rust.Data;

namespace xml2rust.Generators
{
	public class SimpleRegisterGenerator : RegisterGeneratorBase { 

		private readonly List<FieldGenerator> FieldGenerators;

		public SimpleRegisterGenerator(PeripheralGenerator peripheralGenerator, Register register) : base(peripheralGenerator, register)
		{
			FieldGenerators = register.Fields?.Field.Select(f => new FieldGenerator(this, f)).ToList() ?? new List<FieldGenerator>();
		}

		public override void Generate(GeneratorOutput output)
		{
			output.AddComment(PeripheralGenerator.FileName, Register.Description.ToSingleLine());
			output.AddLine(PeripheralGenerator.FileName, "pub mod {0} {{", Register.Name.ToLower());
			foreach (FieldGenerator field in FieldGenerators)
			{
				field.Generate(output);
			}
			output.AddLine(PeripheralGenerator.FileName, "}}");
		}
	}
}