using xml2rust.Data;

namespace xml2rust.Generators
{
	public class InterruptGenerator : IGenerator
	{
		private readonly PeripheralGenerator _peripheralGenerator;
		private readonly Interrupt _interrupt;

		public InterruptGenerator(PeripheralGenerator peripheralGenerator, Interrupt interrupt)
		{
			_peripheralGenerator = peripheralGenerator;
			_interrupt = interrupt;
		}

		public void Generate(GeneratorOutput output)
		{
			output.AddComment(_peripheralGenerator.FileName, _interrupt.Description.ToSingleLine());
			output.AddLine(_peripheralGenerator.FileName, "pub const INTERRUPT_{0}: u32 = {1};", _interrupt.Name.ToUpper(), _interrupt.Value);
		}
	}
}