using DoAnAPI.IRepository;
using DoAnAPI.Models.Domain;
using DoAnAPI.Models.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace DoAnAPI.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DatabaseContext _context;

        public CustomerRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<Result<List<CustomerDTO>>> GetAll()
        {
            var listCustomer = new List<CustomerDTO>(); 
            var customers = await _context.Customers.ToListAsync();
            foreach(var customer in customers)
            {
                var customerDTO = new CustomerDTO
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Email = customer.Email,
                    Phone = customer.Phone
                };
                listCustomer.Add(customerDTO);
            };
            return Result<List<CustomerDTO>>.Success(listCustomer);
        }

        public async Task<Result<CustomerDTO>> GetById(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            var customerDTO = new CustomerDTO();
            if (customer == null)
            {
                return Result<CustomerDTO>.Fail("Customer not found.");
            }
            else
            {
                customerDTO.Id = customer.Id;
                customerDTO.Name = customer.Name;
                customerDTO.Email = customer.Email;
                customerDTO.Phone = customer.Phone;

            }
            return Result<CustomerDTO>.Success(customerDTO);
        }

        public async Task<Result<CustomerDTO>> Add(CustomerDTO customerDTO)
        {
            var customer = new Customer();
            customer.Name = customerDTO.Name;
            customer.Phone = customerDTO.Phone;
            customer.Email = customerDTO.Email;

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            customerDTO.Id=customer.Id;
            return Result<CustomerDTO>.Success(customerDTO);
        }

        public async Task<Result<Customer>> Update(Customer customer)
        {
            // Kiểm tra xem có tồn tại khách hàng với id được cung cấp trong đối tượng customer không
            var existingCustomer = await _context.Customers.FindAsync(customer.Id);
            if (existingCustomer == null)
                return Result<Customer>.Fail("Customer not found.");

            // Cập nhật thông tin của khách hàng hiện có với thông tin mới từ đối tượng customer
            existingCustomer.Name = customer.Name;
            existingCustomer.Email = customer.Email;
            existingCustomer.Phone = customer.Phone;

            try
            {
                // Lưu các thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();
                // Trả về kết quả thành công và thông tin của khách hàng đã được cập nhật
                return Result<Customer>.Success(existingCustomer);
            }
            catch (DbUpdateConcurrencyException)
            {
                // Xử lý ngoại lệ đồng thời khi có ngoại lệ về sự không nhất quán trong cơ sở dữ liệu
                // (trường hợp khách hàng đã bị xóa hoặc cập nhật bởi người dùng khác)
                if (!CustomerExists(customer.Id))
                    return Result<Customer>.Fail("Customer not found.");
                else
                    throw; // Ném lại ngoại lệ nếu không phải là trường hợp trên
            }
        }


        public async Task<Result<bool>> Delete(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return Result<bool>.Fail("Customer not found.");

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return Result<bool>.Success(true);
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
