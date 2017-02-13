using System.Collections.Generic;
using System.Linq;
using xml2rust.Data;

namespace xml2rust.Generators
{
	public abstract class RegisterGeneratorBase : IGenerator
	{
		public readonly Register Register;
		public readonly PeripheralGenerator PeripheralGenerator;
		public abstract void Generate(GeneratorOutput output);

		public RegisterGeneratorBase(PeripheralGenerator peripheralGenerator, Register register)
		{
			PeripheralGenerator = peripheralGenerator;
			Register = register;
		}

		public static RegisterGeneratorBase GetGenerator(PeripheralGenerator peripheralGenerator, Register register)
		{
			if (register.Fields != null && (register.Access == "read-write" || register.Fields.Field.All(f => f.Access == "read-write")))
			{
				return new CachedStateRegisterGenerator(peripheralGenerator, register);
			}
			List<IGrouping<string, Field>> groups = register.Fields?.Field.GroupBy(f => StringUtils.NumericRegex.Replace(f.Name, "")).ToList();
			if (groups?.Count <= 2 && groups[0].Count() > 1)
			{
				return new GroupedRegisterGenerator(peripheralGenerator, register, groups);
			}
			return new SimpleRegisterGenerator(peripheralGenerator, register);
		}
	}
}