namespace FamilyBook.Domain.Validators;

public interface IValidator<T> where T : class
{
    void Validate(T instance);
}