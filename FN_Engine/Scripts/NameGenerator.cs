namespace FN_Engine
{
    public class NameGenerator
    {
        public string BaseName;
        int Counter = -1;

        public NameGenerator(string BaseName)
        {
            this.BaseName = BaseName;
        }

        public string GenerateName()
        {
            Counter++;
            return BaseName + " " + Counter.ToString();
        }
    }
}
