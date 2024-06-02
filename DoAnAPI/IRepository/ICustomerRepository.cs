using DoAnAPI.Models.Domain;
using DoAnAPI.Models.ViewModel;

namespace DoAnAPI.IRepository
{
    public interface ICustomerRepository
    {
        Task<Result<List<CustomerDTO>>> GetAll();
        Task<Result<CustomerDTO>> GetById(int id);
        Task<Result<CustomerDTO>> Add(CustomerDTO customer);
        Task<Result<Customer>> Update( Customer customer);
        Task<Result<bool>> Delete(int id);
    }
}
