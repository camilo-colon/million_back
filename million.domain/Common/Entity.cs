namespace million.domain.common;

public abstract class Entity
{
    public Guid Id { get; private init; } 
    
    protected Entity(Guid id)
    {
        Id = id;
    }

    protected Entity() { }

}