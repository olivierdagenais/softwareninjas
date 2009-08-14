using System;
using System.Collections.Generic;
using System.Reflection;

namespace SoftwareNinjas.Core.TestDoubles.Stubs
{
    internal class ParsedMethod : IEnumerable<ParsedParameter>
    {
        private readonly string _name;
        private readonly IList<ParsedParameter> _parameters;

        public ParsedMethod(string name)
        {
            _name = name;
            _parameters = new List<ParsedParameter>();
        }

        public void Add(ParsedParameter parameter)
        {
            _parameters.Add(parameter);
        }

        public void Add(string parameterType, string parameterName)
        {
            _parameters.Add(new ParsedParameter(parameterType, parameterName));
        }

        public IList<ParsedParameter> Parameters
        {
            get
            {
                return _parameters;
            }
        }

        public IEnumerable<ParsedParameter> CallableParameters
        {
            get
            {
                bool isFirst = true;
                Func<ParsedParameter, bool> skipFirst = (pp) =>
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        return false;
                    }
                    return true;
                };
                return _parameters.Filter(skipFirst);
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public bool HasArguments
        {
            get
            {
                return _parameters.Count > 1;
            }
        }

        public bool HasReturnType
        {
            get
            {
                return _parameters[0].Type != "void";
            }
        }

        public override string ToString()
        {
            return String.Format("{0} ({1})", _name, _parameters.Join(", "));
        }

        #region IEnumerable<ParsedParameter> Members

        public IEnumerator<ParsedParameter> GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        #endregion
    }
}
