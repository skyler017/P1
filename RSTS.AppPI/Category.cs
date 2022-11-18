namespace RSTS.AppPI
{
    public class Category
    {
        public int Categoryid { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return "ID: " + Categoryid + " Name: " + CategoryName + " Desc: " + Description;
        }
    }
}
