using System;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.Entities
{
    public class Theme
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public List<ThemeData> Data { get; set; }
    }

    public class ThemeData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ColorCode { get; set; }
    }
}
