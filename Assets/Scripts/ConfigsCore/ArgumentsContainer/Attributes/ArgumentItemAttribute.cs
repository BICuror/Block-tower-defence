namespace Configs
{
    public class ArgumentItemAttribute : System.Attribute
    {
        public string FieldName;

        public ArgumentItemAttribute(string name)
        {
            FieldName = name;
        }
    }
}