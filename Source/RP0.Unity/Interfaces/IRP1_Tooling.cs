namespace RP0.Unity.Interfaces
{
    public interface IRP1_Tooling
    {
        string partName { get; }
        float partToolingCost { get; }
        float partUntooledCost { get; }
        float partTooledCost { get; }
    }
}
