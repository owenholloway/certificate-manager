using System.ComponentModel.DataAnnotations;
using CertificateManager.Interfaces.Stores;

namespace CertificateManager.Features.Stores
{
    public class Entity<TId> : IEntity<TId> where TId: struct
    {
        [Key]
        public virtual TId Id { get; protected set; }

        protected Entity(){}
        
        protected Entity(TId id)
        {
            if (Equals(id,default(TId)))
            {
                throw new ArgumentException("The identifier cannot be default.", paramName: nameof(id));
            }

            // ReSharper disable once VirtualMemberCallInConstructor
            Id = id;
        }

        public override bool Equals(object? otherObject)
        {
            if (otherObject is Entity<TId> entity && !Equals(Id, default(TId)))
            {
                return Equals(entity);
            }
            // ReSharper disable once BaseObjectEqualsIsObjectEquals
            return base.Equals(otherObject);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        private bool Equals(Entity<TId>? other)
        {
            return other != null && Id.Equals(other.Id);
        }
        
    }
    
}