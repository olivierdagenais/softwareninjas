using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareNinjas.Core.TestDoubles.Stubs
{
    internal class ParsedParameter
    {
        private readonly string _type;
        private readonly string _name;

        public ParsedParameter(string type, string name)
        {
            _type = type;
            _name = name;
        }

        public string Type
        {
            get
            {
                return _type;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public override string ToString()
        {
            return String.Format("{0} {1}", _type, _name);
        }
    }
}
