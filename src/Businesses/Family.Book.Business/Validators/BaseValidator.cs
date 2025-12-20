using FamilyBook.Domain.Validators;

namespace FamilyBook.Business.Validators;

public abstract class BaseValidator<T> : IValidator<T> where T : class
{
    protected BaseValidator(T instance)
    {
        Validate(instance);
    }

    public abstract void Validate(T instance);
}