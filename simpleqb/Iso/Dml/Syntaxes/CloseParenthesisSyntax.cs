﻿using simpleqb.Core;

namespace simpleqb.Iso.Dml.Syntaxes
{
    internal class CloseParenthesisSyntax : SyntaxBase, ISql, INoMarginLeftSyntax
    {
        internal CloseParenthesisSyntax(SyntaxBase syntaxBase)
            : base(syntaxBase)
        { }

        public string ToSql()
        {
            return Relay.RelayQuery(this);
        }

        public override string Represent()
        {
            return ")";
        }
    }
}
