﻿namespace simpleqb.Iso.Dml.Transitions
{
    public interface IEqualToTransition<Result> where Result : class
    {
        Result EqualTo { get; }
    }
}
