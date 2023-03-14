namespace Gwen.Net.RichText
{
    public class LineBreakPart : Part
    {
        public override string[] Split(ref Font splitFont)
        {
            return new[]
            {
                "\n"
            };
        }
    }
}
