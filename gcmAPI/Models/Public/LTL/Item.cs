
namespace gcmAPI.Models.Public.LTL
{
    public class Item
    {
        public string commodity, type;
        public string description, tag;

        public bool hazmat;
        public double? length, width, height, freightClass, weight;
        public int units, pieces;
    }
}