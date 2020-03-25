namespace Infonet.CStoreCommander.Entities
{
    public class Policy
    {
        public string ClassName { get; set; }

        public string PolicyName { get; set; }
        
        public bool Implemented { get; set; }
        
	    public string Value { get; set; }
	
        public string VarType { get; set; }
    }

    public class PolicyCanbe
    {
        public string PolicyName { get; set; }

        public int Sequence { get; set; }

        public string CanBe { get; set; }
    }

    public class PolicySet
    {
        public string PolicyName { get; set; }

        public string Level { get; set; }

        public string Value { get; set; }

        public string Set { get; set; }
    }
    
    
}
