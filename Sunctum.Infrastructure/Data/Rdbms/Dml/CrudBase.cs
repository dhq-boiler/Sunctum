

using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Sunctum.Infrastructure.Data.Rdbms.Dml
{
    public abstract class CrudBase : DmlBase, ICrud
    {
        public abstract IEnumerable<IRightValue> Parameters { get; }

        public void SetParameters(IDbCommand command)
        {
            ProcessParameters(command, Parameters);
        }

        private void ProcessParameters(IDbCommand command, IEnumerable<IRightValue> parameters)
        {
            foreach (var parameter in parameters)
            {
                if (parameter is PlaceholderRightValue)
                {
                    SetParameter(command, parameter as PlaceholderRightValue);
                }
                else if (parameter is MultiplePlaceholderRightValue)
                {
                    SetParameter(command, parameter as MultiplePlaceholderRightValue);
                }
                else if (parameter is SubqueryRightValue)
                {
                    SetParameter(command, parameter as SubqueryRightValue);
                }
            }
        }

        protected void SetParameter(IDbCommand command, PlaceholderRightValue parameter)
        {
            var p = command.CreateParameter();
            p.ParameterName = parameter.Name;
            p.Value = parameter.Values.First();
            command.Parameters.Add(p);
        }

        protected void SetParameter(IDbCommand command, MultiplePlaceholderRightValue parameter)
        {
            for (int i = 0; i < parameter.Values.Count(); ++i)
            {
                var p = command.CreateParameter();
                p.ParameterName = parameter.PlaceholderNameList.ElementAt(i);
                p.Value = parameter.Values[i];
                command.Parameters.Add(p);
            }
        }

        private void SetParameter(IDbCommand command, SubqueryRightValue subqueryRightValue)
        {
            var parameters = subqueryRightValue.Subquery.Parameters;
            ProcessParameters(command, parameters);
        }
    }
}
