﻿using simpleqb.Iso.Dml.Syntaxes;

namespace simpleqb.Iso.Dml.Transitions
{
    public interface IWhereColumnTransition<R, R1, R2> where R : class
                                                       where R1 : class
                                                       where R2 : class
    {
        ISearchCondition<R, R1, R2> Column(string name);

        ISearchCondition<R, R1, R2> Column(string tableAlias, string name);
    }
}
