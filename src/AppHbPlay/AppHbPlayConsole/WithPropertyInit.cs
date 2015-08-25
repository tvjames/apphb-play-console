namespace AppHbPlayConsole
{
    class WithPropertyInit
    {
        public string FirstProp { get; } = "First Prop Value";
        public string SecondProp { get; } = "Second Prop Value";

        public string ConstructorSetProp { get; }

        public WithPropertyInit()
        {
            this.ConstructorSetProp = $"Using string interpolation {Program.Random.Next()}";
        }

        public string NameOfProp() => nameof(this.ConstructorSetProp);
    }
}