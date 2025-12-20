namespace FamilyBook.Domain.Models;

public abstract class BaseModel
{
    public Guid Id { get; }

    protected BaseModel(Guid id)
    {
        Id = id;
    }
}