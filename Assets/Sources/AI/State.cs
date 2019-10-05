namespace LDJAM45
{
    public abstract class State
    {
        public abstract void Begin();
        public abstract CrewState Update();
        public abstract void End();
    }
}