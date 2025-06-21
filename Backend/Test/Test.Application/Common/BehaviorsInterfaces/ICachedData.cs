namespace Test.Application.Common.BehaviorsInterfaces
{
    public interface ICachedData
    {
        string Key { get; }

        public TimeSpan LifeTime { get; }
    }
}
