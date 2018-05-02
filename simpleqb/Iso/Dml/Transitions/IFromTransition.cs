namespace simpleqb.Iso.Dml.Transitions
{
    public interface IFromTransition<Return> where Return : class
    {
        Return From { get; }
    }
}
