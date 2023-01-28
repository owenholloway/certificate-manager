namespace CertificateManager.Interfaces.Stores
{
    public interface IEntity<out TId>
    {
        TId Id { get; }
    }
}