namespace com.bloomberg.emsx.samples
{

    public class SchemaFieldDefinition
    {

        public string name = "";
        public string status = "";
        public string type = "";
        public int min = 0;
        public int max = 0;
        public string description = "";

        public SchemaFieldDefinition(string name)
        {
            this.name = name;
        }

        public bool isStatic()
        {

            if (description.IndexOf("Static") > -1)
            {
                return true;
            }
            else return false;
        }

        public bool isOrderField()
        {
            if ((description.IndexOf("Order") > -1) || (description.IndexOf("O,R") > -1))
            {
                return true;
            }
            else return false;
        }

        public bool isRouteField()
        {
            if ((description.IndexOf("Route") > -1) || (description.IndexOf("O,R") > -1))
            {
                return true;
            }
            else return false;
        }

        public bool isSpecialField()
        {
            if (description.IndexOf("Special") > -1)
            {
                return true;
            }
            else return false;
        }

    }
}