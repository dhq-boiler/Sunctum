﻿using simpleqb.Iso.Dml.Syntaxes;
using System.Collections.Generic;

namespace simpleqb.Iso.Dml.Transitions
{
    public interface IRowTransition
    {
        IRowSyntax Row(IEnumerable<object> values);

        IRowSyntax Row(params object[] values);

        IRowSyntax Rows(IEnumerable<IEnumerable<object>> rows);

        IRowSyntax Rows(params object[][] rows);
    }
}
