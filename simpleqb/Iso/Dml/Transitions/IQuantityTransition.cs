﻿using simpleqb.Iso.Dml.Syntaxes;

namespace simpleqb.Iso.Dml.Transitions
{
    public interface IQuantityTransition
    {
        ISetQuantifierSyntax Distinct { get; }

        IColumnTransition<IColumnSyntax> All { get; }
    }
}
