namespace Configs
{
    public class ArgumentAttribute : System.Attribute
    {
        public string FieldName;

        public ArgumentAttribute(string name)
        {
            FieldName = name;
        }
    }
}