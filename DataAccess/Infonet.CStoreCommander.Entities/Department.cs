namespace Infonet.CStoreCommander.Entities
{
    public class Department
    {
        public string Dept { get; set; }

        public string DeptName { get; set; }

        public int CountDetail { get; set; }

        public decimal Sales { get; set; }
    }

    public class Dept
    {
        public string DeptCode { get; set; }

        public string DeptName { get; set; }

        public int EODDetail { get; set; }

        public int EODGroup { get; set; }

        public int CountDetail { get; set; }
    }

    public class SubDept
    {
        public string Sub_Dept { get; set; }

        public string Sub_Name { get; set; }
    }
}
